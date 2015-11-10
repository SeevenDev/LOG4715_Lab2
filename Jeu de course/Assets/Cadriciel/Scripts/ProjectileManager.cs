using UnityEngine;
using System.Collections;

/**
 * 
 * Tuto projectile : https://www.youtube.com/watch?v=DEtZUeVY9qk
 * Tuto projectile explosif : https://www.youtube.com/watch?v=J9ErQDWR44k
 **/
public class ProjectileManager : MonoBehaviour 
{
	// ==========================================
	// == Attributs
	// ==========================================

	// Objets projectiles :
	private GameObject carapaceVerte;
	private GameObject carapaceRouge;
	private GameObject carapaceBleue;

	private Transform[] _path_v, _path_p;
	private int currentWaypoint = 0;
	[SerializeField] private float reachDist = 100.0f;

	// Vitesses :
	[SerializeField]
	private float vitesseVerte = 100.0f;
	[SerializeField]
	private float vitesseRouge = 80.0f;
	[SerializeField]
	private float vitesseBleue = 70.0f;

	// Explosion :
	[SerializeField]
	private float rayonExplosion = 1.0f;
	[SerializeField]
	private float forceExplosion = 2.0f;
	[SerializeField]
	private float forceSoulevante = 2.0f;

	// Dégâts :
	[SerializeField]
	private int degatsVert = 15;
	[SerializeField]
	private int degatsRouge = 10;
	[SerializeField]
	private int degatsBleu = 30;

	// Carapace Verte :
	[SerializeField]
	private int nbMaxRebonds = 3;

	// Carapace rouge :
	[SerializeField] private float turn = 20.0f;
	[SerializeField] private float distMaxCible = 80.0f;
	[SerializeField] private float maxAngle = 90.0f;
	[SerializeField] private LayerMask voituresLayer;

	// ==========================================
	// == Start
	// ==========================================

	void Start () 
	{
		carapaceVerte = Resources.Load ("CarapaceVerte") as GameObject;
		carapaceRouge = Resources.Load ("CarapaceRouge") as GameObject;
		carapaceBleue = Resources.Load ("CarapaceBleue") as GameObject;


		// Chargement du path pour les projectiles :
		GameObject p = GameObject.Find ("Path P");
		this._path_p = new Transform[p.transform.childCount];
		for (int i = 0; i < p.transform.childCount; i++) {
			this._path_p[i] = p.transform.GetChild(i);
		}

		// Chargement du path pour la voiture 
		// (correspond avec le Path P mais adapté pour la trajectoire du joueur) :
		p = GameObject.Find ("Path V");
		this._path_v = new Transform[p.transform.childCount];
		for (int i = 0; i < p.transform.childCount; i++) {
			this._path_v[i] = p.transform.GetChild(i);
		}
	}
	
	// ==========================================
	// == Update
	// ==========================================

	void Update () 
	{
		// === Utilisation d'un projectile ===

		GameObject projectile;

		// --- Mise à jour de la position sur le path ---

		// Vérifier à quel waypoint on est :
		for (int i = 0; i < _path_v.Length; i ++) {
			if ((_path_v[i].position - transform.position).magnitude < reachDist) {
				// Cibler le waypoint suivant (en boucle) :
				currentWaypoint = (i+1) % _path_v.Length;
			}
		}

		// --- Carapace Verte ---

		if (Input.GetButtonDown ("Fire1")) 
		{
			// Instanciation de l'objet Carapace Verte :
			projectile = Instantiate (carapaceVerte) as GameObject;
			projectile.rigidbody.useGravity = true;
			
			// Positionnement initial du projectile : 
			projectile.transform.position = transform.position + transform.forward * 4.5f;

			// Ajout du composant ProjectileCollider pour gérer les collisions et les déplacements :

			Vector3 vitesseInitiale = transform.forward * vitesseVerte;
			
			ProjectileBehavior collider = projectile.AddComponent<ProjectileBehavior> () as ProjectileBehavior;
			collider.setExplosion (forceExplosion, rayonExplosion, forceSoulevante, degatsVert);
			collider.setVelocity (vitesseVerte, vitesseInitiale);
			collider.setMode (ProjectileBehavior.Mode.BOUNCING);
			collider.setNbMaxRebonds (nbMaxRebonds);
		}

		// --- Carapace Rouge ---

		else if (Input.GetButtonDown ("Fire2")) 
		{
			// Instanciation de l'objet Carapace Rouge :
			projectile = Instantiate (carapaceRouge) as GameObject;
			projectile.rigidbody.useGravity = true;
			
			// Positionnement initial du projectile : 
			projectile.transform.position = transform.position + transform.forward * 4.5f;
			projectile.transform.forward = transform.forward;

			// Ajout du composant ProjectileCollider pour gérer les collisions et les déplacements :
			
			Vector3 vitesseInitiale = transform.forward * vitesseRouge;
			
			ProjectileBehavior collider = projectile.AddComponent<ProjectileBehavior> () as ProjectileBehavior;
			collider.setExplosion (forceExplosion, rayonExplosion, forceSoulevante, degatsRouge);
			collider.setVelocity (vitesseRouge, vitesseInitiale);
			collider.setHoming(turn, distMaxCible, maxAngle, voituresLayer);
			collider.setPath(_path_p, currentWaypoint, reachDist);
			collider.setMode (ProjectileBehavior.Mode.HOMING_DEVICE);
		}

		// --- Carapace Bleue ---

		else if (Input.GetButtonDown ("Fire3")) 
		{
			// Instanciation de l'objet Carapace Rouge :
			projectile = Instantiate (carapaceBleue) as GameObject;
			projectile.rigidbody.useGravity = true;
			
			// Positionnement initial du projectile : 
			projectile.transform.position = transform.position + transform.forward * 10;
			projectile.transform.forward = transform.forward;
			
			// Ajout du composant ProjectileCollider pour gérer les collisions et les déplacements :
			
			Vector3 vitesseInitiale = transform.forward * vitesseBleue;
			
			ProjectileBehavior collider = projectile.AddComponent<ProjectileBehavior> () as ProjectileBehavior;
			collider.setExplosion (forceExplosion, rayonExplosion, forceSoulevante, degatsBleu);
			collider.setVelocity (vitesseBleue, vitesseInitiale);
			collider.setHoming(turn, distMaxCible, maxAngle, voituresLayer);
			collider.setPath(_path_p, currentWaypoint, reachDist);
			collider.setPathVehicules(_path_v);
			collider.setMode (ProjectileBehavior.Mode.TO_THE_TOP);
		}
	}
}
