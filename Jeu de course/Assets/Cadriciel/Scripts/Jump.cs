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
