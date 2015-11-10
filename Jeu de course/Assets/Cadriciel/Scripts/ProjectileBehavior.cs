using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 
 * Source rebonds : http://answers.unity3d.com/questions/352609/how-can-i-reflect-a-projectile.html
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
		HOMING_DEVICE, 	// rouge
		TO_THE_TOP		// bleu
	};

	// ==========================================
	// == Attributs
	// ==========================================

	// Mode :
	private Mode mode;

	// Explosion :
	private GameObject explosion;
	private float rayonExplosion, forceExplosion, forceSoulevante, vitesse;
	private int _projectileDamage;

	// Vitesse :
	private Vector3 force;
	private Vector3 oldVelocity;

	// Path following :
	private Transform[] _path_p, _path_v;
	private float reachDist;
	private int currentPoint = 0;

	// Vert :
	private int nbRebonds, nbMaxRebonds;

	// Rouge/Bleu :
	private float turn, maxDistance;
	private Transform cible, premiereVoiture;
	private bool cibleAcquise = false;
	private float maxAngle;
	private LayerMask voituresLayer;

	// Autres :
	private CheckpointManager cp_manager;
	private List<string> joueursIgnores;
	private Material[] wall_materials;

	// ==========================================
	// == Start
	// ==========================================

	void Start () 
	{
		explosion = Resources.Load ("Fireworks") as GameObject;

		// Le CheckpointManager :
		GameObject game_manager = GameObject.Find ("Game Manager") as GameObject;
		this.cp_manager = game_manager.GetComponent<CheckpointManager> ();

		// Joueurs ignorés par fusée bleue :
		joueursIgnores = new List<string> (new string[]{"Joueur 1", "Joueur 2"});

		// Materials des murs et obstacles :
		wall_materials = new Material[] {
			Resources.Load ("Wall_1") as Material,
			Resources.Load ("Wall_2") as Material,
			Resources.Load ("Wall_3") as Material
		};
	}

	// ==========================================
	// == Update
	// ==========================================

	void Update()
	{
		Rigidbody rb = transform.rigidbody;

		// === Carapace Rouge ===

		if (mode == Mode.HOMING_DEVICE) 
		{
			// --- Choix de la cible ---

			if (!trouverCible ()) {
				cible = _path_p [currentPoint];

				// Déterminer si on a atteint le waypoint courant :
				if ((_path_p [currentPoint].position - transform.position).magnitude < reachDist) {
					// Cibler le waypoint suivant (en boucle) :
					currentPoint = (currentPoint + 1) % _path_p.Length;
				}
			}

			// --- Poursuite de la cible / du waypoint ---

			// Déterminer la rotation de la carapace (ce qui influe sa direction) :
			Quaternion rotationCible = Quaternion.LookRotation (cible.position - transform.position);
			rb.MoveRotation (Quaternion.RotateTowards (transform.rotation, rotationCible, turn));

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

		// === Fusée Bleue ===

		else if (mode == Mode.TO_THE_TOP) 
		{
			// --- Choix de la cible ---

			premiereVoiture = cp_manager.getCarAtPosition(0, joueursIgnores).transform;

			// --- Follow path ---

			cible = _path_p [currentPoint];
			
			// Déterminer si on a atteint le waypoint courant :
			if ((_path_p [currentPoint].position - transform.position).magnitude < reachDist) {
				// Cibler le waypoint suivant (en boucle) :
				currentPoint = (currentPoint + 1) % _path_p.Length;
			}
			
			// --- Poursuite de la cible / du waypoint ---

			// Déterminer si le premier est assez proche pour le poursuivre :

			float distPremier = (premiereVoiture.position - transform.position).magnitude;

			Vector3 directionPremier = premiereVoiture.position - transform.position;
			Vector3 directionProjectile = transform.forward;
			float anglePremier = Vector3.Angle(directionPremier, directionProjectile);

			bool obstacle = Physics.Raycast(transform.position, directionPremier, distPremier, voituresLayer);
			if (distPremier <= maxDistance
			    && anglePremier <= maxAngle
			    && !obstacle) 
			{
				cible = premiereVoiture;
			}
			
			// Déterminer la rotation de la carapace (ce qui influe sa direction) :
			Quaternion rotationCible = Quaternion.LookRotation (cible.position - transform.position);
			rb.MoveRotation (Quaternion.RotateTowards (transform.rotation, rotationCible, turn));
			
			// Vitesse constante :
			rb.velocity = transform.forward * vitesse;
			rb.AddForce (Physics.gravity);
		}
	}

	// ==========================================
	// == Setters
	// ==========================================
	
	public void setExplosion (float forceExplosion, float rayonExplosion, float forceSoulevante, int damage)
	{
		this.rayonExplosion = rayonExplosion * transform.localScale.x;
		this.forceExplosion = forceExplosion;
		this.forceSoulevante = forceSoulevante;
		this._projectileDamage = damage;
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

	public void setHoming (float turn, float maxDist, float maxAngle, LayerMask voituresLayer)
	{
		this.turn = turn;
		this.maxDistance = maxDist;
		this.maxAngle = maxAngle;
		this.voituresLayer = voituresLayer;
	}

	public void setPath (Transform[] path, int currentWaypoint, float reachDist)
	{
		this._path_p = path;
		this.currentPoint = currentWaypoint;
		this.reachDist = reachDist;
	}

	public void setPathVehicules (Transform[] _path_v)
	{
		this._path_v = _path_v;
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

			// --- Carapace Bleue : ne pas exploser si ce n'est pas la première voiture ---

			bool exploser = true;
			if (mode == Mode.TO_THE_TOP) {
				exploser = (trans == premiereVoiture);
			}

			if (exploser) {
				// Détruire la carapace :
				Destroy(gameObject);
			}

			// --- Effet visuel de l'explosion ---

			GameObject fx = Instantiate (explosion) as GameObject;
			fx.transform.position = positionFinale;
			Destroy (fx, 1);

			// --- Exploser sur la voiture touchée ---
			
			string nomVoiture = collision.gameObject.name;
			Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

			if (rb != null)
			{
				rb.AddExplosionForce(forceExplosion, positionFinale, rayonExplosion, forceSoulevante, ForceMode.Impulse);

				// Ajout de dégats au voitures touchées :
				collision.gameObject.GetComponent<Vie>().addDamage(_projectileDamage);
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

			// --- Desctruction des murs internes et des obstacles ---

			if (trans.parent.name == "Inner wall" || trans.parent.name == "Obstacles")
			{
				MeshRenderer rend = trans.GetComponent<MeshRenderer>();

				// Déterminer le Material courant :
				for (int i = 0 ; i < wall_materials.Length ; i++)
				{
					if (rend.material.name.StartsWith(wall_materials[i].name))
					{
						// Si c'est le dernier Material : détruire l'objet
						if (i == wall_materials.Length - 1) {
							Destroy(collision.gameObject);
							break;
						}

						// Sinon, passer au Material suivant :
						rend.material = Instantiate(wall_materials[i+1]) as Material;
						break;
					}
				}
			}
		}
	}
}
