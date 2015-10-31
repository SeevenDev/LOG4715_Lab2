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

	private GameObject carapaceVerte;

	[SerializeField]
	private float vitesseVerte = 100.0f;

	[SerializeField]
	private float rayonExplosion = 1.0f;
	
	[SerializeField]
	private float forceExplosion = 2.0f;

	[SerializeField]
	private float forceSoulevante = 2.0f;

	[SerializeField]
	private int nbMaxRebonds = 3;

	// ==========================================
	// == Start
	// ==========================================

	void Start () 
	{
		carapaceVerte = Resources.Load ("CarapaceVerte") as GameObject;
	}
	
	// ==========================================
	// == Update
	// ==========================================

	void Update () 
	{
		// === Utilisation d'un projectile ===

		// --- Carapace Verte ---

		if (Input.GetMouseButtonDown (0)) 
		{
			// Instanciation de l'objet Carapace Verte :
			GameObject projectile = Instantiate(carapaceVerte) as GameObject;
			projectile.rigidbody.useGravity = true;
			
			// Positionnement initial du projectile : 
			projectile.transform.position = transform.position + transform.forward * 4;

			// Ajout du composant ProjectileCollider pour gérer les collisions et les déplacements :

			Vector3 vitesseInitiale = transform.forward * vitesseVerte;
			
			ProjectileCollider collider = projectile.AddComponent<ProjectileCollider>() as ProjectileCollider;
			collider.setExplosion (forceExplosion, rayonExplosion, forceSoulevante);
			collider.setVelocity (vitesseVerte, vitesseInitiale);
			collider.setNbMaxRebonds(nbMaxRebonds);
		}
	}
}
