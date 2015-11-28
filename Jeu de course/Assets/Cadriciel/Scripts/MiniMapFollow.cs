using UnityEngine;
using System.Collections;

public class MiniMapFollow : MonoBehaviour {

	public Transform joueur;

	//http://docs.unity3d.com/ScriptReference/MonoBehaviour.LateUpdate.html
	//***Actualisation de la position de la camera suivant le joueur
	void LateUpdate(){
		transform.position = new Vector3 (
			joueur.position.x, 
			transform.position.y,
			joueur.position.z
		);
	}
}
