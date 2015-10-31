// Tiré de http://answers.unity3d.com/questions/15414/how-can-i-make-an-on-screen-speedometer.html

using UnityEngine;
using System.Collections;

public class Speedometer : MonoBehaviour {

	public Texture2D texture = null;  // texture de l'aiguille de vitesse
	public float angle = 0;  // angle de l'aiguille de vitesse
	public Vector2 size = new Vector2(128, 128);
	Vector2 pos = new Vector2(0, 0);
	Rect rect;
	Vector2 pivot;
	
	void Start () {
		Update ();
	}

	void Update () {
		pos = new Vector2(transform.localPosition.x, transform.localPosition.y);
		rect = new Rect(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, size.x, size.y);
		pivot = new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
	}

	void OnGUI() {
		if (Application.isEditor) { Update(); }
		Matrix4x4 matrixBackup = GUI.matrix;
		GUIUtility.RotateAroundPivot(angle, pivot);
		GUI.DrawTexture(rect, texture);
		GUI.matrix = matrixBackup;
	}
}
