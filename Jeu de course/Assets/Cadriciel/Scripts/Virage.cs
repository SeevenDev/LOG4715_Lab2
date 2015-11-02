using UnityEngine;
using System.Collections;

public class Virage : MonoBehaviour 
{
	[SerializeField]
	private GUIText virage;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		virage.text = getIndication ();
	}
	
	public string getIndication() {
		var x = transform.position.x;
		var z = transform.position.z;
		
		//Premier virage
		if (x > -20 && x < 20 && z > 160 && z < 190) {
			return "1.LEFT";
		}
		//Deuxième virage
		else if(x > -250 && x < -130 && z > 20 && z < 100){
			return "2.RIGHT";
		}
		//Troisième virage
		else if(x > -305 && x < -260 && z > 67 && z < 120){
			return "3.LEFT";
		}
		//Quatrième virage
		else if(x > -400 && x < -335 && z > -30 && z < 5){
			return "4.LEFT";
		}
		//Cinquième virage
		else if(x > -255 && x < -200 && z > -130 && z < 60){
			return "5.LEFT";
		}
		//Sixième virage
		else if(x > -78 && x < -33 && z > -95 && z < -50){
			return "6.LEFT";
		}
		//Pas de virage
		else{
			return "";
		}
	}
}
