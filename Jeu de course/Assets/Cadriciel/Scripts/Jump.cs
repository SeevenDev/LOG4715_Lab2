using UnityEngine;
using System.Collections;

public class Jump : MonoBehaviour
{
	// ==========================================
	// == Attributs
	// ==========================================

	[SerializeField]
	private float _jumpForce = 20.0f;

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
		if (!grounded) {  // Si le joueur est en l'air
			if (Input.GetAxis("Horizontal") > 0) {
				transform.Rotate(0.0f, 5.0f, 0.0f);
			}
			if (Input.GetAxis("Horizontal") < 0) {
				transform.Rotate(0.0f, -5.0f, 0.0f);
			}
			if (Input.GetAxis("Vertical") > 0) {
				transform.Rotate(5.0f, 0.0f, 0.0f);
			}
			if (Input.GetAxis("Vertical") < 0) {
				transform.Rotate(-5.0f, 0.0f, 0.0f);
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
