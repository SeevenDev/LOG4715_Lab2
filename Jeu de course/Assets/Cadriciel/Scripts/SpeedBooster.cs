using UnityEngine;
using System.Collections;

public class SpeedBooster : MonoBehaviour {

	[SerializeField]
	private float boostForce = 100.0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter (Collider col)
	{
		if (col.attachedRigidbody != null && col.attachedRigidbody.tag == "Player")
		{
			Transform joueur = col.attachedRigidbody.transform;
			joueur.rigidbody.AddForce(joueur.forward * boostForce, ForceMode.Impulse);
		}
	}
	
}