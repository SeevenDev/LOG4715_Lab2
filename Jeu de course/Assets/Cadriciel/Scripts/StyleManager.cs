using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

using Debug = UnityEngine.Debug;

/**
 * Mécanique 1 : Style
 **/

public class StyleManager : MonoBehaviour 
{
	// ==========================================
	// == Structure du log des styles
	// ==========================================

	struct Log
	{
		public string _description;
		public int _points;
		public Stopwatch _timer;

		public Log(string desc, int points) 
		{
			_description = desc;
			_points = points;
			_timer = new Stopwatch();
			_timer.Start();
		}

		public string getLine()
		{
			return _description + " +" + _points.ToString () + " pts";
		}
	}

	// ==========================================
	// == Attributs
	// ==========================================

	int _points;

	Queue _log = new Queue();

	[SerializeField]
	private GUIText _totalAfficheur; // afficheur du total des points de style

	[SerializeField]
	private GUIText _info; // afficheur des nouveaux poinst de style

	[SerializeField]
	private float _infoDuration = 1.0f; // temps d'affichage des nouveaux poins de style en secondes

	// ==========================================
	// == Start
	// ==========================================

	void Start()
	{
		Debug.Log ("Start de StyleManager");
		_points = 0;
		updateStyle ();

		// StartCoroutine (testCorout());
	}

	/* IEnumerator testCorout() {
		logStyle (100, "Beau début");

		yield return new WaitForSeconds (1.0f);
		logStyle (300, "Cascade LOURDE");

		yield return new WaitForSeconds (1.0f);
		logStyle (1000, "Frolage");
	} */

	void updateStyle() {
		_totalAfficheur.text = _points.ToString() + " pts";
	}
	
	// ==========================================
	// == Afficher les nouveaux points de style
	// ==========================================

	void Update() 
	{
		if (_log.Count > 0) {
			// === Retirer toutes les lignes expirées (affichées suffisament longtemps) ===

			bool allGood;
			do {
				allGood = true;
				// Vérifier le premier élément de la Queue :
				long timespan = ((Log)_log.Peek ())._timer.ElapsedMilliseconds;
				if (timespan > _infoDuration*1000)
				{
					// On supprime la ligne expirée de la Queue :
					Log currentLine = (Log)_log.Dequeue ();
					allGood = false;

					// On comptabilise les points et on met à jour le total :
					_points += currentLine._points;
					updateStyle ();
				}
			} while (!allGood && _log.Count > 0);

			// === Lire le _log des styles ===

			string lines = "";
			object[] logArray = _log.ToArray ();

			for (int i = 0; i < _log.Count; i++) {
				lines += ((Log)logArray [i]).getLine () + (i < _log.Count - 1 ? "\n" : ""); 
				// (ajout d'un "\n" s'il y a encore des lignes à afficher après)
			}

			// Enfin afficher les lignes de style :
			printStyle (lines);
		}
	}

	public void logStyle(int points, string description) 
	{
		// Ajouter le style dans le log :
		_log.Enqueue (new Log(description, points));
	}

	private void printStyle(string lines) {
		_info.text = lines;
	}
}
