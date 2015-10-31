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

	[SerializeField]
	private float rayonExplosion = 1.0f;
	
	[SerializeField]
	private float forceExplosion = 2.0f;

	[SerializeField]
	private float forceSoulevante = 2.0f;

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
			projectile.transform.position = transform.position + transform.forward * 3;

			StartCoroutine(carapaceVerteCorout(projectile));
		}
	}

	IEnumerator carapaceVerteCorout(GameObject projectile)
	{
		Vector3 force = transform.forward * forceVerte;

		ProjectileCollider explosion = projectile.AddComponent<ProjectileCollider>() as ProjectileCollider;
		explosion.setExplosion (forceExplosion, rayonExplosion, forceSoulevante);

		// Force initiale :
		projectile.rigidbody.AddForce (force);

		while (projectile != null) 
		{
			// Faire bouger le projectile :
			/*while (projectile.GetComponent<Rigidbody>().velocity.magnitude < force.magnitude) 
			{
				projectile.GetComponent<Rigidbody> ().velocity += force / 30;
				yield return true;
			}*/

			Rigidbody rb = projectile.rigidbody;
			rb.velocity = new Vector3(force.x, rb.velocity.y, force.z);
			rb.AddForce(Physics.gravity);

			yield return true;
		}
	}
}
