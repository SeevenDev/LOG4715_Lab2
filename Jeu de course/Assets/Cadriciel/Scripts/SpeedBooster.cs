using UnityEngine;
using System.Collections;

public class SpeedBooster : MonoBehaviour {

	[SerializeField]
	private float boostForce = 1000;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter (Collision col)
	{
		if(col.gameObject.transform.parent.name == "Cars")
		{
			GameObject joueur = col.gameObject;
			Vector3 originalSpeed = joueur.transform.rigidbody.velocity;
			joueur.transform.rigidbody.AddForce(joueur.transform.forward * boostForce);
		}
	}
	
}