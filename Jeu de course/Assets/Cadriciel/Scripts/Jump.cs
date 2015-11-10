using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour
{
	// ==========================================
	// == Attributs
	// ==========================================

	[SerializeField]
	private float _jumpForce = 20.0f;

	[SerializeField]
	private float _airControlRotation = 3.0f;

	private bool grounded;

	// ==========================================
	// == FixedUpdate
	// ==========================================

	void FixedUpdate ()
	{
		// === Sauter ===

		if (Input.GetButtonDown("Jump") && grounded)
		{
			rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
			rigidbody.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
		}

		// === Air Control ===

		if (!grounded)
		{  
			// Tourner horizontalement (comme un virage à plat) :
			if (Input.GetAxis("VirageAerien") > 0) {
				transform.Rotate(0.0f, _airControlRotation, 0.0f);
			}
			if (Input.GetAxis("VirageAerien") < 0) {
				transform.Rotate(0.0f, -_airControlRotation, 0.0f);
			}

			// Flip vertical (salto avant/arrière) :
			if (Input.GetAxis("Vertical") > 0) {
				transform.Rotate(_airControlRotation, 0.0f, 0.0f);
			}
			if (Input.GetAxis("Vertical") < 0) {
				transform.Rotate(-_airControlRotation, 0.0f, 0.0f);
			}

			// Tonneau gauche/droite :
			if (Input.GetAxis("Horizontal") > 0)
			{
				transform.Rotate(0.0f, 0.0f, -_airControlRotation);
			}
			if (Input.GetAxis("Horizontal") < 0)
			{
				transform.Rotate(0.0f, 0.0f, _airControlRotation);
			}
		}
    }

	// ==========================================
	// == Vérifier si le joueur est au sol
	// ==========================================

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "Track")
		{
			grounded = true;
		}
	}

	void OnCollisionExit(Collision col)
	{
		if (col.gameObject.name == "Track")
		{
			grounded = false;
		}
	}
}
