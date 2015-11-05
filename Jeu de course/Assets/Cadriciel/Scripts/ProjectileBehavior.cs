﻿using UnityEngine;
using System.Collections;

/**
 * 
 * Souce rebonds : http://answers.unity3d.com/questions/352609/how-can-i-reflect-a-projectile.html
 * Tuto tete chercheuse : https://www.youtube.com/watch?v=feTek1j1Beo
 * Tuto path following : https://www.youtube.com/watch?v=fvdRKS8x0aM
 **/
public class ProjectileBehavior : MonoBehaviour 
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

	// Path following :
	private Transform[] _path;
	private float reachDist;
	private int currentPoint = 0;

	// Vert :
	private int nbRebonds, nbMaxRebonds;

	// Rouge :
	private float turn, retard, maxDistance;
	private Transform cible;
	private bool cibleAcquise = false;
	private float maxAngle;
	private LayerMask voituresLayer;

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

		// === Carapace Rouge ===

		if (this.mode == Mode.HOMING_DEVICE ) 
		{
			// --- Choix de la cible ---

			if (!trouverCible()) {
				cible = _path[currentPoint];

				// Déterminer si on a atteint le waypoint courant :
				if ((_path[currentPoint].position - transform.position).magnitude < reachDist) {
					// Cibler le waypoint suivant (en boucle) :
					currentPoint = (currentPoint + 1) % _path.Length;
				}
			}

			// Debug.Log ("Aiming : " + cible);

			// --- Poursuite de la cible / du waypoint ---

			// Déterminer la rotation de la carapace (ce qui influe sa direction) :
			Quaternion rotationCible = Quaternion.LookRotation(cible.position - transform.position);
			rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rotationCible, turn));

			// Vitesse constante :
			rb.velocity = transform.forward * vitesse;
			rb.AddForce (Physics.gravity);
		} 

		// === Carapace Verte ===

		else if (mode == Mode.BOUNCING) 
		{
			oldVelocity = rb.velocity;

			rb.velocity = new Vector3 (force.x, rb.velocity.y, force.z);
			rb.AddForce (Physics.gravity);
		}
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

	public void setHoming (float turn, float retard, float maxDist, float maxAngle, LayerMask voituresLayer)
	{
		this.turn = turn;
		this.retard = retard;
		this.maxDistance = maxDist;
		this.maxAngle = maxAngle;
		this.voituresLayer = voituresLayer;
	}

	public void setPath (Transform[] path, int currentWaypoint, float reachDist)
	{
		this._path = path;
		this.currentPoint = currentWaypoint;
		this.reachDist = reachDist;
	}

	// ==========================================
	// == Carapace Rouge
	// ==========================================

	bool trouverCible() 
	{
		float distance = maxDistance;

		// Choix de la cible :
		GameObject[] joueurs = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < joueurs.Length; i++) 
		{
			if (joueurs[i].transform.name != "Joueur 1") // on ne veut pas se tirer dessus !
			{
				float diff = (joueurs[i].transform.position - transform.position).sqrMagnitude;
				Vector3 directionCible = joueurs[i].transform.position - transform.position;
				Vector3 directionProjectile = transform.forward;

				// On acquière la cible si elle est assez proche, "à l'avant" (angle < 90°) 
				// et s'il n'y a pas d'obstacle entre le projectile et la voiture
				if (diff < distance 
				    && Vector3.Angle (directionCible, directionProjectile) <= maxAngle
				    && ! Physics.Raycast(transform.position, directionCible, diff, voituresLayer)
				    ) 
				{
					distance = diff;
					cible = joueurs[i].transform;

					Debug.Log ("Cible : " + joueurs[i].transform.name);

					return true;
				}
			}
		}
		return false;
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
							// Debug.Log ("Explosion sur " + nomVoitureProche);
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
			// --- Carapace Verte : rebondir sur le mur ---

			if (mode == Mode.BOUNCING)
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

			// --- Carapace Rouge : exploser ---

			else if (mode == Mode.HOMING_DEVICE)
			{
				// Position finale de la carapace :
				Vector3 positionFinale = transform.position;
				
				// Détruire la carapace :
				Destroy (gameObject);
				
				// Effet visuel de l'explosion : 
				GameObject fx = Instantiate (explosion) as GameObject;
				fx.transform.position = positionFinale;
				Destroy (fx, 1);
			}
		}
	}

	// ==========================================
	// == Path
	// ==========================================

	void OnDrawGizmos() 
	{
		for (int i = 0; i < _path.Length; i++) {
			if (_path[i] != null) {
				Gizmos.DrawSphere(_path[i].position, reachDist);
			}
		}
	}

}
