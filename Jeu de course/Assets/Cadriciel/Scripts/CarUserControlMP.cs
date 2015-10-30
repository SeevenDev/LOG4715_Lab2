using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarUserControlMP : MonoBehaviour
{
	// the car controller we want to use
	private CarController car;

	[SerializeField]
	private string vertical = "Vertical";

	[SerializeField]
	private string horizontal = "Horizontal";

	private StyleManager styleManager;
	private bool _isCrashing = false;
	
	void Awake ()
	{
		Debug.Log ("Awake in CarUserControlMP");
		// get the car controller
		car = GetComponent<CarController>();
		styleManager = GetComponent<StyleManager>();
	}
	
	void FixedUpdate()
	{
		// pass the input to the car!
		#if CROSS_PLATFORM_INPUT
		float h = CrossPlatformInput.GetAxis(horizontal);
		float v = CrossPlatformInput.GetAxis(vertical);
		#else
		float h = Input.GetAxis(horizontal);
		float v = Input.GetAxis(vertical);
		#endif
		car.Move(h,v);
	}

	// ==========================================
	// == Collision handlers
	// ==========================================

	void OnCollisionEnter(Collision theCollision)
	{
		if (theCollision.transform.root.name == "Cars")
		{
			_isCrashing = true;
			styleManager.logStyle (20, "Crash !");
		} 
	}
	void OnCollisionExit(Collision theCollision)
	{
		if (theCollision.transform.root.name == "Cars")
		{
			_isCrashing = false;
		} 
	}
}
