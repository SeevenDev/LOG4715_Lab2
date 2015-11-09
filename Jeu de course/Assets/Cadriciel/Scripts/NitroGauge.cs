using UnityEngine;
using System.Collections;

public class NitroGauge : MonoBehaviour {

	[SerializeField]
	private float progress = 100;

	[SerializeField]
	private float nitroForce = 50;

	[SerializeField]
	private GUITexture _nitro;

	// Use this for initialization
	void Start () {
		this.enabled = false;
		StartCoroutine (enableNitro());
	}

	IEnumerator enableNitro(){
		yield return new WaitForSeconds(3);
		this.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {

		if (progress >= 100)
			progress = 100;

		if (progress <= 0)
			progress = 0;

		if (Input.GetKey ("x") && progress > 10) {
			transform.rigidbody.AddForce (transform.forward * nitroForce);
			progress -= 3;
		}

		if (Input.GetKey ("x") && progress <= 10) {
			progress -= 2;
		}


		progress += 0.5f;

		_nitro.pixelInset = new Rect(_nitro.pixelInset.x, _nitro.pixelInset.y, progress, _nitro.pixelInset.height);
	}

}

