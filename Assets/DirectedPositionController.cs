using UnityEngine;
using System.Collections;

public class DirectedPositionController : MonoBehaviour {

    //public GameLogic gamelogic;

	Transform tran;

	Transform cam;

	public float radius = 5f;

	private Vector3 pointOfInterest;

	public GameObject[] poiObjects;

	int counter = -1;

	//public GameObject[] poiObjectsAndVantagePoints;

	public bool DirectedPoiPointsOnly = true;

	public bool DirectedPoiPointsDistances = false;

	public bool RedFocusOnGreenOff = true;

    //this option makes the red object the green object of the next round
    public bool connectedSequentialPoints = false;

	bool trigger = true;

	void Start () {

		tran = this.transform;

		cam = Camera.main.transform;

		pointOfInterest = poiObjects[0].transform.position;
	
	}

	void Update () {

		if (Input.GetKeyDown (KeyCode.Z))
			trigger = true;

        
		//allow change between directed centers
		if (trigger && DirectedPoiPointsOnly) {
			trigger = false;
			counter++;
			if (counter >=  poiObjects.Length) {
				counter = 0;
			}

			pointOfInterest = poiObjects [counter ].transform.position;

            if (counter == 0) {
                radius = 2.0001f;

            }else
            {
                radius = 2 * Vector3.Magnitude(poiObjects[counter - 1].transform.position - poiObjects[counter].transform.position);
            }

        }
        

		if (trigger && DirectedPoiPointsDistances) {
			trigger = false;
			counter++;
			if (counter >= poiObjects.Length / 2) {
				counter = 0;
			}

            

			if (RedFocusOnGreenOff) {
				pointOfInterest = poiObjects [counter * 2].transform.position;
				radius = 2* Vector3.Magnitude (poiObjects [counter * 2].transform.position - poiObjects [counter * 2 + 1].transform.position);
			} else {
				pointOfInterest = poiObjects [counter * 2 + 1].transform.position;
				radius = Vector3.Magnitude (poiObjects [counter * 2].transform.position - poiObjects [counter * 2 + 1].transform.position);
			}

            //print("set POI and radius");
		}


		tran.position = Vector3.Normalize (-cam.position + pointOfInterest) * radius + cam.position;

	
	}

	public void TriggerChangeObject(){
		trigger = true;
	}
}
