using UnityEngine;
using System.Collections;

/**
 * 
 * Tuto : https://www.youtube.com/watch?v=DEtZUeVY9qk
 **/
public class ProjectileManager : MonoBehaviour 
{
	// ==========================================
	// == Attributs
	// ==========================================

	private GameObject carapaceVerte;

	[SerializeField]
	private float forceVerte = 100.0f;

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

			// Positionnement initial du projectile : 
			projectile.transform.position = transform.position + transform.forward * 5;

			// Faire bouger le projectile :
			projectile.GetComponent<Rigidbody>().velocity = transform.forward * forceVerte;
		}
	}
}
