using UnityEngine;
using System.Collections;

public class AnyOrbitNavigation : MonoBehaviour {

    // By Benjamin Outram
    // AnyOrbit allows orbital motion around and towards the orbital center, which is a separate game object.  Moving the orbital center will alter the path of the user as the camera rotates, so they move on a smooth path towards a new orbit around the new center.

	public float startRadius = 10f;

	public float radiusThresholdFactor = 0.2f; //minimum radius as a factor of distance to targetOrbitCenter (maximum radius is inverse of this)

	public GameObject cam;
	//public GameObject camHolder;
	public GameObject targetOrbitCenter;
	public GameObject illustrationTorus;

	private Vector3 lastTargetOrbitCenterPos;
	public Transform lastCamTransform;

	public float radiiShrinkFactor = 1.0f;

    //for debugging, assign objects to these to view the orbital center
	//public GameObject cubeX;
	//public GameObject cubeY;

	float radiusX_export;
	float radiusY_export;
	Vector3 torusCenter_export;

	public float forwardTeleportFactor = 0.5f;

	public GameObject head;
	public bool setOffsetAtStart = false;
	public Vector3 positionOffset = Vector3.zero;

	public float GetTorusRadX (){
		return radiusX_export;
	}
	public float GetTorusRadY (){
		return radiusY_export;
	}
	public Vector3 GetTorusCenter (){
		return torusCenter_export;
	}

    public bool turnOffAnyOrbit = false;


	void Start () {

		//initialise camera position and parameters
		transform.position = targetOrbitCenter.transform.position + new Vector3 (0, 0, - startRadius);
		SetLastParams ();

		if (setOffsetAtStart && head != null) {
			Invoke ("SetPosOffset", 1f);

        }
    }

	void SetPosOffset() {
		positionOffset = head.transform.localPosition;

	}

	
	void Update () {

		//SetPosOffset ();

		AnyOrbit ();

		SetLastParams ();

		if (Input.GetKeyDown (KeyCode.Mouse0) ){
			TeleportForward ();
		}
		if (Input.GetKeyDown (KeyCode.Mouse1) ){
			TeleportBackward ();
		}
	}

	void TeleportForward (){

		Vector3 newPos = transform.position + forwardTeleportFactor * Vector3.Magnitude (targetOrbitCenter.transform.position - transform.position) * lastCamTransform.forward;
		transform.position = newPos;
		lastCamTransform.position = newPos;
		//print ("mouse0");
	}
	void TeleportBackward (){
		
		Vector3 newPos = transform.position - Vector3.Magnitude (targetOrbitCenter.transform.position - transform.position) * lastCamTransform.forward;
		transform.position = newPos;
		lastCamTransform.position = newPos;
		//print ("mouse1");
	}


	// the parametric torus constructor for torus with symmetry along y-axis
	Vector3  TorusVerticle (float c, float a, float theta, float phi){

		float Cphi = Mathf.Cos (phi);
		float Sphi = Mathf.Sin (phi);
		float Ctheta = Mathf.Cos (theta);
		float Stheta = Mathf.Sin (theta);

		//parametric equations of a torus with symmetry axis along y-axis in left-handed coordinates (vertical)
		float VTx = -(c + a * Ctheta) * Sphi;
		float VTy = a * Stheta;
		float VTz = -(c + a * Ctheta) * Cphi;	

		Vector3 torusParametricPosition = new Vector3 (VTx, VTy, VTz);

		return torusParametricPosition;
	}

	// the parametric torus constructor for torus with symmetry along y-axis
	Vector3  TorusHorizontal (float c, float a, float theta, float phi, Vector3 rotateAboutPoint, float rotateAboutAngle){

		float Cphi = Mathf.Cos (phi);
		float Sphi = Mathf.Sin (phi);
		float Ctheta = Mathf.Cos (theta);
		float Stheta = Mathf.Sin (theta);

		//parametric equations of a torus with symmetry axis along y-axis in left-handed coordinates (vertical)
		float VTx = -a * Sphi;
		float VTy = (c + a * Cphi) * Stheta;
		float VTz = - (c + a * Cphi) * Ctheta;

		Vector3 torusParametricPosition = new Vector3 (VTx, VTy, VTz);

		Vector3 torusParametricPositionRotated = Quaternion.Euler(new Vector3(0,rotateAboutAngle,0)) * (torusParametricPosition - rotateAboutPoint) + rotateAboutPoint;

		return torusParametricPositionRotated;
	}




	void AnyOrbit(){


		//determine position of targetOrbitCenter in camera FOV
		Vector3 robRel = lastCamTransform.InverseTransformPoint (lastTargetOrbitCenterPos);

		//Debug.Log ("robrel " + robRel);

		//determine radii based on relative position of targetOrbitCenter in FOV, do not allow radii to be zero or negative
		float radiusX = robRel.z - Mathf.Abs(robRel.x) * radiiShrinkFactor;
		if (radiusX < robRel.z * radiusThresholdFactor) {
			radiusX = robRel.z * radiusThresholdFactor;
		}
		float radiusY = robRel.z - Mathf.Abs(robRel.y) * radiiShrinkFactor;
		if (radiusY < robRel.z * radiusThresholdFactor) {
			radiusY = robRel.z * radiusThresholdFactor;
		}



		DetermineUserHeadMotionDirection ();

		//make large or small radii depending on if camera is rotating away from or towards the targetOrbitCenter
		if (headRotationDirection.x * Mathf.Sign (robRel.x) == -1f) {
			radiusX = robRel.z * robRel.z / radiusX;
		}
		if (headRotationDirection.y * Mathf.Sign(robRel.y) == 1f) {
			radiusY = robRel.z * robRel.z / radiusY;
		}

        //if targetOrbitCenter is behind the user, make the user rotation egocentric or if we want to temporarily halt anyorbit
        if (robRel.z <= 0 || turnOffAnyOrbit) { radiusX = 0; radiusY = 0; };

		if (radiusX == radiusY) {
			radiusY = 0.999f * radiusY; //math messes up when they are equal
		}

		radiusX_export = radiusX;
		radiusY_export = radiusY;
		//for debugging
		//radiusY = 20f;
		//radiusY = 20f;
		//radiusX = radiusY * 0.3f;
		//radiusY = radiusX * 0.2f;
		//cubeX.transform.position = lastCamTransform.TransformPoint (new Vector3 (0, 0, radiusX));
		//cubeY.transform.position = lastCamTransform.TransformPoint (new Vector3 (0, 0, radiusY));
		//Debug.Log ("radiusX " + (radiusX));



		Vector3 camAngle = cam.transform.eulerAngles * Mathf.Deg2Rad;
		Vector3 lastCamAngle = lastCamTransform.eulerAngles * Mathf.Deg2Rad;

		if(Mathf.Abs( Mathf.Sin(camAngle.x) ) < 0.99f){ //prevent ambiguities at extreme zenith angles

			float tR;
			float tr;

			if (radiusX > radiusY) {
				//do horizontal torus calculation
				//large and small torus radii:
				tR = radiusX - radiusY;
				tr = radiusY;

				//torus center is based on the last camera position & angle
				Vector3 torusCenter = lastCamTransform.position - TorusVerticle (tR, tr, lastCamAngle.x, lastCamAngle.y);
				torusCenter_export = torusCenter;

				//move to position based on constructed torus.
				transform.position = TorusVerticle (tR, tr, camAngle.x, camAngle.y) + torusCenter - positionOffset;

				//for plotting torus
				Torus.TORUS_R = tR;
				Torus.TORUS_r = tr;
				illustrationTorus.transform.position = torusCenter;
				illustrationTorus.transform.rotation = Quaternion.Euler( new Vector3 (0, 0, 0));


			}

			if (radiusY > radiusX) {

				//find large and small radii of vertical torus
				tR = radiusY - radiusX;
				tr = radiusX;

				//determine angle the torus should be facing
				Vector3 vec1 = lastCamTransform.rotation * Vector3.forward;
				float ang1 = Mathf.Atan2 (vec1.x, vec1.z) * Mathf.Rad2Deg;

				//torus center is based on the last camera position & angle
				Vector3 torusCenter = lastCamTransform.position + lastCamTransform.forward * radiusY;
				torusCenter_export = torusCenter;

				//move to position based on constructed torus.
				transform.position = TorusHorizontal (tR, tr, camAngle.x, Mathf.DeltaAngle(lastCamAngle.y, camAngle.y), Vector3.zero, ang1) + torusCenter  - positionOffset;

				//for plotting torus
				Torus.TORUS_R = tR;
				Torus.TORUS_r = tr;
				illustrationTorus.transform.position = torusCenter;
				illustrationTorus.transform.rotation = Quaternion.Euler( new Vector3 (0, 0, 0));
				illustrationTorus.transform.RotateAround (torusCenter, Vector3.forward , 90f); 
				illustrationTorus.transform.RotateAround (torusCenter, Vector3.up , ang1);

			}

		}

	}




	void SetLastParams (){

		lastCamTransform.rotation = cam.transform.rotation;
		lastCamTransform.position = transform.position + positionOffset;
		lastTargetOrbitCenterPos = targetOrbitCenter.transform.position;

	}

	Vector2 headRotationDirection = new Vector2 (1f, 1f);

	void DetermineUserHeadMotionDirection (){

		//project quaternion rotations into vectors in world space
		Vector3 vec1 = lastCamTransform.rotation * Vector3.forward;
		Vector3 vec2 = cam.transform.rotation * Vector3.forward;

		//determine angle of each vector onto X-Z plane
		float ang1 = Mathf.Atan2 (vec1.x, vec1.z);
		float ang2 = Mathf.Atan2 (vec2.x, vec2.z);

		//difference between angles
		float xAngleDifference = Mathf.DeltaAngle (ang1, ang2);

		headRotationDirection.x = xAngleDifference > 0 ? 1.0f : -1.0f;
		headRotationDirection.y = vec1.y - vec2.y > 0 ? 1.0f: -1.0f;
		//note: y direction of head does not need to bother converting to angles since we just need to compare y-components of the vector

		//Debug.Log ("xDir = " + headRotationDirection.x + "  yDir = " + headRotationDirection.y);

	}

}
