using UnityEngine;
using System.Collections;

/**
 * Mécanique 1 : Style
 **/

public class StyleManager : MonoBehaviour 
{
	// ==========================================
	// == Attributs
	// ==========================================

	int _points;

	[SerializeField]
	private GUIText _totalAfficheur;

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
	}

	void updateStyle() {
		_totalAfficheur.text = _points.ToString() + " pts";
	}
	
	// ==========================================
	// == Afficher les nouveaux points de style
	// ==========================================

	public void printStyle(int points) 
	{
		_points += points;
		StartCoroutine(printStyleCoroutine(points));
	}

	IEnumerator printStyleCoroutine(int points) 
	{
		_info.text = "+" + points.ToString () + " !";
		yield return new WaitForSeconds(_infoDuration);
		_info.text = "";
		updateStyle ();
	}
}
