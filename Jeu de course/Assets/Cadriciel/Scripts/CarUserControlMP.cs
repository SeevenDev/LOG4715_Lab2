using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CarController))]
public class CarUserControlMP : MonoBehaviour
{
	struct Frolage
	{
		public bool _entree, _crash;

		public Frolage(bool entree, bool crash) {
			_entree = entree;
			_crash = crash;
		}
	}

	// ==========================================
	// == Attributs
	// ==========================================

	// the car controller we want to use
	private CarController car;

	[SerializeField]
	private string vertical = "Vertical";

	[SerializeField]
	private string horizontal = "Horizontal";

	private StyleManager styleManager;

	[SerializeField]
	private float rayonDeFrolage = 1.0f;

	[SerializeField]
	private bool afficherRayonDeFrolage = false;

	private Hashtable voituresEnFrolage = new Hashtable();

	[SerializeField]
	private GUIText speedometer;

	[SerializeField]
	private Speedometer speedNeedle;

	[SerializeField]
	private int _degatCrash = 10;

	// ==========================================
	// == Awake
	// ==========================================

	void Awake ()
	{
		// get the car controller
		car = GetComponent<CarController>();

		styleManager = GetComponent<StyleManager>();
	}

	// ==========================================
	// == FixedUpdate
	// ==========================================

	void FixedUpdate()
	{
		// pass the input to the car!
		#if CROSS_PLATFORM_INPUT
		float h = CrossPlatformInput.GetAxis(horizontal);
		float v = CrossPlatformInput.GetAxis(vertical);
		#else
		float h = Input.GetAxis(horizontal);
		float v = Input.GetAxis(vertical);
		#endif
		car.Move(h,v);

		// === Vérifier si l'on frole une voiture ===

		// --- Début du frolage ---

		HashSet<string> voituresEnFrolageCurrentFrame = new HashSet<string>();

		Collider[] collidersProches = Physics.OverlapSphere (car.transform.position, rayonDeFrolage);
		for (int i = 0; i < collidersProches.Length; i++) 
		{
			Transform objetProche = collidersProches[i].transform;
			if (objetProche.root.name == "Cars") 
			{
				string nomVoitureProche = objetProche.parent.parent.name;
				if (nomVoitureProche != "Joueur 1" && nomVoitureProche != "Cars") 
				{
					if (!voituresEnFrolage.Contains (nomVoitureProche)) {
						// Si la voiture entre en frolage pour la première frame :
						voituresEnFrolage.Add (nomVoitureProche, new Frolage());
					}
					voituresEnFrolageCurrentFrame.Add(nomVoitureProche);
				}
			}
		}

		// --- Fin du frolage ---

		// Copie du HashSet pour ne aps le modifier pendant qu'on l'itère :
		string[] voituresEnFrolageArray = new string[voituresEnFrolage.Count];
		voituresEnFrolage.Keys.CopyTo (voituresEnFrolageArray, 0);

		// Pour chaque voiture que le joueur a commencé à froler, 
		// vérifier si on la frole toujours pendant cette frame :
		for(int i = 0; i < voituresEnFrolageArray.Length; i++) 
		{
			if (!voituresEnFrolageCurrentFrame.Contains(voituresEnFrolageArray[i])) 
			{
				// Le joueur ne frole plus la voiture à partir de cette frame...
				if (! ((Frolage)voituresEnFrolage[voituresEnFrolageArray[i]])._crash) 
				{
					// S'il n'y a eu aucun crash : on comptabilise la figure
					styleManager.logStyle(40, "Frolage");
				}
				// et supprime la voiture de la Hashtable :
				voituresEnFrolage.Remove(voituresEnFrolageArray[i]);
			}
		}

		// --- Actualisation du compteur de vitesse ---
		var speed = rigidbody.velocity.magnitude;
		speedometer.text = speed.ToString("F2") + " km/h";
		speedNeedle.angle = speed * 1.4f;
	}

	void OnDrawGizmos() 
	{
		if (!Application.isPlaying) return;

		if (afficherRayonDeFrolage) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (car.transform.position, rayonDeFrolage);
		}
	}

	// ==========================================
	// == Collision handlers
	// ==========================================

	void OnCollisionEnter(Collision theCollision)
	{
		string rootName = theCollision.transform.root.name;
		string parentName = theCollision.transform.parent.name;
		if (rootName == "Cars")
		{
			// On comptabilise la figure
			// et on enlève la voiture du HashSet de frolage :
			string name = theCollision.transform.name;
			styleManager.logStyle (20, "Crash !");
			voituresEnFrolage[name] = new Frolage(true, true);

			// On enlève des dégâts aux 2 voitures :
			gameObject.GetComponent<Vie>().addDamage(_degatCrash);
			theCollision.gameObject.GetComponent<Vie>().addDamage(_degatCrash);
		}

		else if (parentName == "Inner wall"
				 || parentName== "Outer wall"
				 || parentName== "Obstacles"
				 || parentName == "Goal Posts")
		{
			// On enlève des dégâts au joueur  :
			gameObject.GetComponent<Vie>().addDamage(_degatCrash);
		}
	}
	void OnCollisionExit(Collision theCollision)
	{
		if (theCollision.transform.root.name == "Cars")
		{
			// _isCrashing = false;
		} 
	}
}
