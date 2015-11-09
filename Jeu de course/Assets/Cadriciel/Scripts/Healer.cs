using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class Healer : MonoBehaviour
{
	// ==========================================
	// == Attributs
	// ==========================================

	[SerializeField]
	private int _healAmount = 30;

	[SerializeField]
	private int _respawnTimeMs = 30000;

	private Stopwatch _count;
	private Vector3 rotationsPerMinute = new Vector3(10.0f, 10.0f, 10.0f);

	// ==========================================
	// == Start
	// ==========================================

	void Start () {
		_count = new Stopwatch();
	}

	// ==========================================
	// == Update
	// ==========================================

	void Update ()
	{
		// === Faire réapparaitre le bloc si besoin ===

		if (!renderer.enabled && _count.ElapsedMilliseconds > _respawnTimeMs)
		{
			renderer.enabled = true;
			_count.Stop();
			_count.Reset();
		}

		// === Faire tourner le bloc ===

		if (renderer.enabled)
		{
			transform.Rotate(
				(float)(6.0 * rotationsPerMinute.x * Time.deltaTime),
				(float)(6.0 * rotationsPerMinute.y * Time.deltaTime),
				(float)(6.0 * rotationsPerMinute.z * Time.deltaTime)
			);
		}
	}

	// ==========================================
	// == Trigger
	// ==========================================

	void OnTriggerEnter(Collider col)
	{
		if (renderer.enabled && col.transform.root.name == "Cars")
		{
			// Destruction du bloc :
			renderer.enabled = false;
			_count.Start();

			// Soin du véhicule :
			col.transform.parent.parent.GetComponent<Vie>().addHealth(_healAmount);
		}
	}
}
