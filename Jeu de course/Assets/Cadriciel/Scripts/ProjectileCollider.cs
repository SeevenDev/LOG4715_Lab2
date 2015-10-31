using UnityEngine;
using System.Collections;

/**
 * 
 * Souce rebonds : http://answers.unity3d.com/questions/352609/how-can-i-reflect-a-projectile.html 
 **/
public class ProjectileCollider : MonoBehaviour 
{
	private GameObject explosion;

	private float rayonExplosion, forceExplosion, forceSoulevante, vitesse;
	private Vector3 force;

	private Vector3 oldVelocity;

	private int nbRebonds, nbMaxRebonds;

	// Use this for initialization
	void Start () 
	{
		explosion = Resources.Load ("Fireworks") as GameObject;
	}

	void Update()
	{
		Rigidbody rb = transform.rigidbody;

		oldVelocity = rb.velocity;

		rb.velocity = new Vector3(force.x, rb.velocity.y, force.z);
		rb.AddForce(Physics.gravity);
	}

	public void setExplosion (float forceExplosion, float rayonExplosion, float forceSoulevante)
	{
		this.rayonExplosion = rayonExplosion;
		this.forceExplosion = forceExplosion;
		this.forceSoulevante = forceSoulevante;
	}

	public void setVelocity (float vitesse, Vector3 forceInitiale)
	{
		this.vitesse = vitesse;
		this.force = forceInitiale;
	}

	public void setNbMaxRebonds(int nb)
	{
		this.nbMaxRebonds = nb;
	}
	
	void OnCollisionEnter(Collision collision)
	{
		Transform trans = collision.transform;
		if (trans.root.name == "Cars") {
			// === Collision avec une voiture ===

			// Position finale de la carapace :
			Vector3 positionFinale = transform.position;

			// Détruire la carapace :
			Destroy (gameObject);

			// Effet visuel de l'explosion : 
			GameObject fx = Instantiate (explosion) as GameObject;
			fx.transform.position = positionFinale;
			Destroy (fx, 1);

			// Ajouter une force d'explosion aux voitures proches :
			Collider[] collidersProches = Physics.OverlapSphere (positionFinale, rayonExplosion);
			for (int i = 0; i < collidersProches.Length; i++) {
				Transform objetProche = collidersProches [i].transform;
				if (objetProche.root.name == "Cars") {
					string nomVoitureProche = objetProche.parent.parent.name;
					if (nomVoitureProche != "Cars") {
						Rigidbody rb = objetProche.parent.parent.GetComponent<Rigidbody> ();
						if (rb != null) {
							rb.AddExplosionForce (forceExplosion, positionFinale, rayonExplosion, forceSoulevante, ForceMode.Impulse);
							Debug.Log ("Explosion sur " + nomVoitureProche);
						}
					}
				}
			}
		} 
		else if (trans != null && trans.parent != null && trans.parent.name != null
		         && (trans.parent.name == "Inner wall" 
		         || trans.parent.name == "Outer wall"
		         || trans.parent.name == "Obstacles"))
		{
			// === Collision avec un mur ===

			// Détruire le projectile s'il a rebondi suffisamment de fois :
			if (nbRebonds++ > nbMaxRebonds) 
			{
				// Effet visuel de l'explosion : 
				GameObject fx = Instantiate (explosion) as GameObject;
				fx.transform.position = transform.position;
				Destroy (fx, 1);

				Destroy (gameObject);

				return;
			}

			// Get the point of contact :
			ContactPoint contactPoint = collision.contacts[0];

			// reflect our old velocity off the contact point's normal vector
			Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contactPoint.normal);

			// assign the reflected velocity back to the rigidbody
			force = reflectedVelocity;
		}
	}
}
