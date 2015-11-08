using UnityEngine;
using System.Collections;

public class NitroGauge : MonoBehaviour {

	[SerializeField]
	private float progress = 100;

	[SerializeField]
	private float nitroForce = 100;

	[SerializeField]
	private GUITexture _nitro;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		Debug.Log (progress);

		if (progress >= 100)
			progress = 100;

		if (progress <= 0)
			progress = 0;

		if (Input.GetKey ("x") && progress > 50) {
			transform.rigidbody.AddForce (transform.forward * nitroForce);
			progress -= 5;
		}

		if (Input.GetKey ("x") && progress <= 50) {
			progress -= 2;
		}


		progress += 0.5f;

		_nitro.pixelInset = new Rect(_nitro.pixelInset.x, _nitro.pixelInset.y, progress, _nitro.pixelInset.height);
	}

}

