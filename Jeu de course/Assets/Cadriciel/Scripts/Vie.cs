using UnityEngine;
using System.Collections;
using System;

/**
 *
 * Source placement barre de vie : http://forum.unity3d.com/threads/health-bar-over-enemy.26014/
 */
public class Vie : MonoBehaviour
{
	// ==========================================
	// == Attributs
	// ==========================================

	// Vie :
	[SerializeField]
	private int _maxVie = 100;
	private int _vie;

	// Ralentissement :
	[SerializeField]
	private int _seuilRalentissement = 30;
	[SerializeField]
	private float _pourcentageRalentissement = 0.30f;

	// Barre de vie :
	private Vector2 _ajustement = new Vector2(0.0f, 50.0f);
	private Vector2 _tailleBarre = new Vector2(60, 20);
	// private GUITexture _barre;

	// Joueur 1 :
	private GameObject _joueur1;
	private float _distMax = 45.0f, // distance d'affichage
		_angleMax = 200.0f; // angle d'affichage max

	// ==========================================
	// == Start
	// ==========================================

	void Start ()
	{
		// Vie :
		_vie = _maxVie;

		// Barre de vie : 
		_ajustement.x = _tailleBarre.x / 2;
		// _barre = Resources.Load("Healthbar") as GUITexture;

		// Joueur 1 :
		_joueur1 = GameObject.Find("Joueur 1");
	}

	// ==========================================
	// == Update
	// ==========================================

	void Update ()
	{
		// === Vérifier si la voiture doit être ralentie ou non ===

		if (_vie <= _seuilRalentissement) {
			gameObject.GetComponent<CarController>().setSloweringFactor(_pourcentageRalentissement);
		} else {
			gameObject.GetComponent<CarController>().setSloweringFactor(1);
		}
	}

	// ==========================================
	// == Afficher la barre de vie
	// ==========================================

	void OnGUI()
	{
		// === Si la voiture est assez proche du Joueur 1 ===

		float distance = (transform.position - _joueur1.transform.position).magnitude;
        bool assezProche = distance <= _distMax;

		// === Si la voiture est à l'avant du Joueur 1 ===

		Vector3 dirVoiture = transform.position - _joueur1.transform.position;
		float angle = Vector3.Angle(_joueur1.transform.forward, dirVoiture);
		bool devant = angle <= _angleMax/2;
        
		// === Afficher la vie ===

		if (assezProche && devant)
		{
			Vector2 targetPos = Camera.main.WorldToScreenPoint(transform.position);
			GUI.Box(
				new Rect (targetPos.x - _ajustement.x, Screen.height - targetPos.y - _ajustement.y, _tailleBarre.x, _tailleBarre.y), 
				_vie + "/" + _maxVie
			);
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
	}

	public void addHealth(int h)
	{
		if (_vie + h < _maxVie)
			_vie += h;
		else
			_vie = _maxVie;
	}
}
