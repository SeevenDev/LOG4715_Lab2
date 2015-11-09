using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class SpeedBooster : MonoBehaviour
{
	[SerializeField]
	private float boostForce = 100.0f;

	private GameObject _fireModel, _fireObj;

	private Transform _joueur;
	private Stopwatch _watch;
	[SerializeField] private long _fireDurationMs = 2000;

	// Use this for initialization
	void Start ()
	{
		_fireModel = Resources.Load("Fire1") as GameObject;
		_watch = new Stopwatch();
		enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		_fireObj.transform.position = _joueur.position;

		if (_watch.ElapsedMilliseconds >= _fireDurationMs)
		{
			Destroy(_fireObj);
			_watch.Stop();
			_watch.Reset();
			enabled = false;
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (col.attachedRigidbody != null && col.attachedRigidbody.tag == "Player")
		{
			// Ajouter une impulsion au joueur :

			_joueur = col.attachedRigidbody.transform;
			_joueur.rigidbody.AddForce(_joueur.forward * boostForce, ForceMode.Impulse);

			// Ajouter une courte traînée de feu derrière le joueur :
			_fireObj = Instantiate(_fireModel) as GameObject;
			enabled = true;
			_watch.Start();
		}
	}
}