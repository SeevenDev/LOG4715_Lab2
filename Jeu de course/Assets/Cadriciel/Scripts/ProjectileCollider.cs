using UnityEngine;
using System.Collections;

/**
 * 
 * Souce rebonds : http://answers.unity3d.com/questions/352609/how-can-i-reflect-a-projectile.html 
 **/
public class ProjectileCollider : MonoBehaviour 
{
	// ==========================================
	// == Modes de projectiles
	// ==========================================

	public enum Mode {
		BOUNCING, 		// vert
		HOMING_DEVICE 	// rouge
	};

	// ==========================================
	// == Attributs
	// ==========================================

	// Mode :
	private Mode mode;

	// Explosion :
	private GameObject explosion;
	private float rayonExplosion, forceExplosion, forceSoulevante, vitesse;

	// Vitesse :
	private Vector3 force;
	private Vector3 oldVelocity;

	// Vert :
	private int nbRebonds, nbMaxRebonds;


	// ==========================================
	// == Start
	// ==========================================

	void Start () 
	{
		explosion = Resources.Load ("Fireworks") as GameObject;
	}

	// ==========================================
	// == Update
	// ==========================================

	void Update()
	{
		Rigidbody rb = transform.rigidbody;

		oldVelocity = rb.velocity;

		rb.velocity = new Vector3(force.x, rb.velocity.y, force.z);
		rb.AddForce(Physics.gravity);
	}

	// ==========================================
	// == Setters
	// ==========================================
	
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

	public void setMode(Mode m)
	{
		this.mode = m;
	}

	public void setNbMaxRebonds(int nb)
	{
		this.nbMaxRebonds = nb;
	}

	// ==========================================
	// == Sur une collision
	// ==========================================

	void OnCollisionEnter(Collision collision)
	{
		Transform trans = collision.transform;

		// === Collision avec une voiture ===

		if (trans.root.name == "Cars") 
		{
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

		// === Collision avec un mur ===

		else if (trans != null && trans.parent != null && trans.parent.name != null
		         && (trans.parent.name == "Inner wall" 
		         || trans.parent.name == "Outer wall"
		         || trans.parent.name == "Obstacles"))
		{
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
