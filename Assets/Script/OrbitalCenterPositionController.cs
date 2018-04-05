using UnityEngine;
using System.Collections;

public class OrbitalCenterPositionController : MonoBehaviour {

	//This script controls the position of the orbital center which can be moved around at will

	//++++++++++++++++// MOUSE CONTROL //++++++++++++++++//
	//Mouse wheel controls distance from camera
	//Mouse movement controls viewing angle


	public Camera cam;



	Transform cameraTransform;
	Transform orbitalObjectTransform;

	public float sensMouseX = 0.003f;
	public float sensMouseY = 0.003f;
	public float sensASDW_X = 0.05f;
	public float sensASDW_Y = 0.05f;
	public float sensitivityZ = 0.08f;

	public float boundaryZ0 = 1f;
	public float boundaryZ1 = 100f;

	public bool asdwControlOn = true;
	public bool oculusMouseControlOn = true;
	public bool invertScrollWheel = true;

	float lastMouseX;
	float lastMouseY;

	private float robRad;


	//float lastMouseScrollWheelPos;


	void Start () {

		orbitalObjectTransform = GetComponent<Transform> ();

		lastMouseX = Input.mousePosition.x;
		lastMouseY = Input.mousePosition.y;

	
	}

	float inv;
	
	void Update () {

		if (invertScrollWheel) {
			inv = -1f;
		} else {
			inv = 1f;
		}

		SmoothMouse ();
		OculusMouseControl ();

		lastMouseX = Input.mousePosition.x;
		lastMouseY = Input.mousePosition.y;
	
	}




	void OculusMouseControl (){

		//set up relevant vectors
		robRad = Vector3.Magnitude (cam.transform.position - orbitalObjectTransform.position);

		Vector3 fromOrbToCamera = cam.transform.position - orbitalObjectTransform.position;

		Vector3 orbCamPos = cam.transform.InverseTransformPoint (orbitalObjectTransform.transform.position);

		Vector3 orbCamViewPort = cam.WorldToViewportPoint (orbitalObjectTransform.position);

		Vector3 xRadial = - Vector3.Magnitude (fromOrbToCamera) * Vector3.Normalize (Vector3.Cross (fromOrbToCamera, cam.transform.up));
		Vector3 yRadial = - Vector3.Magnitude (fromOrbToCamera) * Vector3.Normalize (Vector3.Cross (fromOrbToCamera, cam.transform.right));



		//control Z distance away from camera of orb (using mouse scroll wheel)
		if (orbCamPos.z < boundaryZ1 ||
			(orbCamPos.z >= boundaryZ1 && inv * Input.mouseScrollDelta.y > 0f)){
			orbitalObjectTransform.position += fromOrbToCamera * inv * Input.mouseScrollDelta.y * sensitivityZ;
		}

		if (orbCamPos.z < 0.1f) {
			orbitalObjectTransform.position =  cam.transform.position + cam.transform.forward * 0.1f;
		}


		//control XY using ASDW keys
		if (asdwControlOn) {
			if (Input.GetKey (KeyCode.A) &&
			   orbCamViewPort.x > 0f) {
				orbitalObjectTransform.position += xRadial * sensASDW_X * 5f;
			}
			if (Input.GetKey (KeyCode.D) &&
			   orbCamViewPort.x < 1f) {
				orbitalObjectTransform.position -= xRadial * sensASDW_X * 5f;
			}
			if (Input.GetKey (KeyCode.W) &&
			   orbCamViewPort.y < 1f) {
				orbitalObjectTransform.position += yRadial * sensASDW_Y * 5f;
			}
			if (Input.GetKey (KeyCode.S) &&
			   orbCamViewPort.y > 0f) {
				orbitalObjectTransform.position -= yRadial * sensASDW_Y * 5f;
			}

		}

		//control XY using mouse
		//float mouseDx = Input.mousePosition.x - lastMouseX;
		//float mouseDy = -(Input.mousePosition.y - lastMouseY);

		//control XY using mouse
		float mouseDx = _smoothMouse.x;
		float mouseDy = -_smoothMouse.y;

		if (oculusMouseControlOn) {
			if (orbCamViewPort.x > 0f && mouseDx < 0) {
				orbitalObjectTransform.position -= xRadial * sensMouseX * mouseDx;
			}
			if (orbCamViewPort.x < 1f && mouseDx > 0) {
				orbitalObjectTransform.position -= xRadial * sensMouseX * mouseDx;
			}
			if (orbCamViewPort.y < 1f && mouseDy < 0) {
				orbitalObjectTransform.position -= yRadial * sensMouseY * mouseDy;
			}
			if (orbCamViewPort.y > 0f && mouseDy > 0) {
				orbitalObjectTransform.position -= yRadial * sensMouseY * mouseDy;
			}

		}



	}

	Vector2 _mouseAbsolute;
	Vector2 _smoothMouse;
	
	//public Vector2 clampInDegrees = new Vector2(360, 180);
	public bool lockCursor;
	public Vector2 sensitivity = new Vector2(2, 2);
	public Vector2 smoothing = new Vector2(3, 3);
	public Vector2 targetDirection;
	public Vector2 targetCharacterDirection;
	
	void SmoothMouse (){

		// Ensure the cursor is always locked when set
		Screen.lockCursor = lockCursor;
	
	
		// Get raw mouse input for a cleaner reading on more sensitive mice.
		var mouseDelta = new Vector2 (Input.GetAxisRaw ("Mouse X"), Input.GetAxisRaw ("Mouse Y"));
	
		// Scale input against the sensitivity setting and multiply that against the smoothing value.
		mouseDelta = Vector2.Scale (mouseDelta, new Vector2 (sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));
	
		// Interpolate mouse movement over time to apply smoothing delta.
		_smoothMouse.x = Mathf.Lerp (_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
		_smoothMouse.y = Mathf.Lerp (_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

		
	}






}
