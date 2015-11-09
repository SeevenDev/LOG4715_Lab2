using UnityEngine;
using System.Collections;

public class NitroGauge : MonoBehaviour {

	[SerializeField]
	private float progress = 100;

	[SerializeField]
	private float nitroForce = 50;

	[SerializeField]
	private GUITexture _nitro;
	private GameObject _fireModel;
	private GameObject _fireObj;

	// Use this for initialization
	void Start () {
		this.enabled = false;
		StartCoroutine (enableNitro());

		_fireModel = Resources.Load("Fire1") as GameObject;
		_fireObj = Instantiate(_fireModel) as GameObject;
		setFireRenderer(false);
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

		if (Input.GetButton("Nitro") && progress > 10)
		{
			transform.rigidbody.AddForce (transform.forward * nitroForce);
			progress -= 3;

			setFireRenderer(true);
            _fireObj.transform.position = transform.position;
		}
		else
		{
			setFireRenderer(false);
		}

		if (Input.GetButton("Nitro") && progress <= 10) {
			progress -= 2;
		}


		progress += 0.5f;

		_nitro.pixelInset = new Rect(_nitro.pixelInset.x, _nitro.pixelInset.y, progress, _nitro.pixelInset.height);
	}

	void setFireRenderer(bool mode)
	{
		_fireObj.transform.Find("OuterCore").renderer.enabled = mode;
		_fireObj.transform.Find("InnerCore").renderer.enabled = mode;
		_fireObj.transform.Find("smoke").renderer.enabled = mode;
	}
}

