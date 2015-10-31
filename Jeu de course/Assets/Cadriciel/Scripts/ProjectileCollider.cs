using UnityEngine;
using System.Collections;

public class ProjectileCollider : MonoBehaviour 
{
	private GameObject explosion;

	// Use this for initialization
	void Start () 
	{
		explosion = Resources.Load ("Fireworks") as GameObject;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		string rootName = collision.transform.root.name;
		if (rootName == "Cars") 
		{
			// Effet visuel de l'explosion : 
			GameObject fx = Instantiate (explosion) as GameObject;
			fx.transform.position = transform.position; 
			Destroy (fx, 1);

			// Détruire la carapace :
			Destroy (gameObject, 1);
		}
	}
}
