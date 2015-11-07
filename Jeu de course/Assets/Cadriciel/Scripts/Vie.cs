using UnityEngine;
using System.Collections;

public class Vie : MonoBehaviour
{
	// ==========================================
	// == Attributs
	// ==========================================

	private int _maxVie = 100, _vie;
	private int _seuilRalentissement = 30;
	private float _pourcentageRalentissement = 0.30f;
	private GUITexture _barre;

	// ==========================================
	// == Start
	// ==========================================

	void Start ()
	{
		_vie = _maxVie;
		_barre = Instantiate(Resources.Load("healthbar")) as GUITexture;
	}

	// ==========================================
	// == Update
	// ==========================================

	void Update ()
	{
		// === Afficher la barre de vie ===

		// http://forum.unity3d.com/threads/health-bar-over-enemy.26014/

		// === Vérifier si la voiture doit être ralentie ou non ===

		if (_vie <= _seuilRalentissement) {
			gameObject.GetComponent<CarController>().setSloweringFactor(_pourcentageRalentissement);
		} else {
			gameObject.GetComponent<CarController>().setSloweringFactor(1);
		}
	}

	// ==========================================
	// == Messages
	// ==========================================

	public void addDamage(int dmg)
	{
		if (_vie - dmg > 0)
			_vie -= dmg;
		else
			_vie = 0;

		Debug.Log("-" + dmg + " : " + _vie + " hp pour " + gameObject.name);
	}

	public void addHealth(int h)
	{
		if (_vie + h < _maxVie)
			_vie += h;
		else
			_vie = _maxVie;

		Debug.Log("+" + h + " : " + _vie + " hp pour " + gameObject.name);
	}
}
