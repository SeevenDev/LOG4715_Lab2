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
			Debug.Log(joueur.name);
			Vector3 originalSpeed = joueur.transform.rigidbody.velocity;
			Debug.Log(originalSpeed);
			joueur.transform.rigidbody.AddForce(joueur.transform.forward * boostForce);
			//StartCoroutine(TemporarySpeedUp(col, originalSpeed));
		}
	}

//	IEnumerator TemporarySpeedUp(Collision col ,Vector3 originalSpeed){
//		yield return new WaitForSeconds(4);
//
//		Vector3 vitesseJoueur = col.gameObject.transform.rigidbody.velocity;
//		GameObject joueur = col.gameObject;
//
//		//while(vitesseJoueur.z > originalSpeed.z)
//		//	joueur.transform.rigidbody.AddForce( - joueur.transform.forward * boostForce/10);
//		joueur.transform.rigidbody.velocity = originalSpeed;
//	}
	
}