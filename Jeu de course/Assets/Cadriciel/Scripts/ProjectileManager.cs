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

			StartCoroutine(carapaceVerteCorout(projectile));
		}
	}

	IEnumerator carapaceVerteCorout(GameObject projectile)
	{
		Vector3 force = transform.forward * forceVerte;

		projectile.AddComponent<ProjectileCollider>();

		while (projectile != null) 
		{
			// Faire bouger le projectile :
			/*while (projectile.GetComponent<Rigidbody>().velocity.magnitude < force.magnitude) 
			{
				projectile.GetComponent<Rigidbody> ().velocity += force / 30;
				yield return true;
			}*/

			projectile.GetComponent<Rigidbody>().velocity = force;
			yield return true;
		}
	}
}
