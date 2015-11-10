using UnityEngine;
using System.Collections;

public class OutOfBounds : MonoBehaviour
{
	private Transform[] _path_v;

	// Use this for initialization
	void Start ()
	{
		// Chargement du path pour la voiture :
		GameObject p = GameObject.Find("Path V");
		this._path_v = new Transform[p.transform.childCount];
		for (int i = 0; i < p.transform.childCount; i++)
		{
			this._path_v[i] = p.transform.GetChild(i);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col)
	{
		// === Voiture : replacer au Waypoint le plus proche ===

		if (col.attachedRigidbody != null && col.attachedRigidbody.tag == "Player")
		{
			Transform voiture = col.transform.parent.parent.transform;

			// --- Trouver le WP le plus proche ---

			float minDistWp = Mathf.Infinity;
			Transform waypoint = null;

			for (int wp = 0; wp < _path_v.Length; wp++)
			{
				float distWp_tmp = (voiture.position - _path_v[wp].transform.position).magnitude;

				if (distWp_tmp < minDistWp)
				{
					minDistWp = distWp_tmp;
					waypoint = _path_v[wp].transform;
				}
			}

			// --- Replacer la voiture ---

			// Position :
			voiture.position = new Vector3(waypoint.position.x, waypoint.position.y + 10, waypoint.position.z);
			// Orientation :
			voiture.rotation = Quaternion.identity;
			voiture.forward = waypoint.forward;
			// Inertie :
			voiture.rigidbody.velocity = Vector3.zero;
			voiture.rigidbody.angularVelocity = Vector3.zero;
		}

		// === Carapace : détruire ===

		else if (col.gameObject.transform.name.StartsWith("Carapace"))
		{
			Destroy(col.gameObject);
		}

	}
}
