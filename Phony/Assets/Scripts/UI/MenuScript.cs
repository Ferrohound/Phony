using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Work In Progress
/// Menu for pausing game and surfing through inventory key items
/// Current purpose:
/// Pause game button
/// tite screen button
/// View Map/items shift through iventory
/// rotate/zoom examine item
/// 
/// </summary>
public class MenuScript: MonoBehaviour {

	public GameObject[] Maps;
	public Camera cam;
	public Canvas objPlane;
	public Transform testObj;

	public float rotationforce = 1; //rotations per min
	public float angdrag = 0;

	public float zoomPower = 0.05f;

	public int zoomLimit = 10;
	private int zoomCounter;
	private Vector3 mousePos;
	private Vector3 mouseDownPos; //axis when mouse button is down
	private Vector3 mouseUpPos;

	private Rigidbody testrigid;


	private bool rotateObj = false;


	//private Vector3 mousePos;

	void Awake() {
		Maps = new GameObject[0];
	}

	// Use this for initialization
	void Start () {

		zoomCounter = 0;

		if(testObj!=null)
			testrigid = testObj.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetMouseButtonDown(0)) {
			mouseDownPos = Input.mousePosition;
			testrigid.angularDrag = angdrag;
		}

		if (Input.GetMouseButton(0)) {
			RotateDrag ();
		}

		if (Input.GetMouseButton (1)) {
			zoomChange();
		}

	}


	void RotateDrag(){
		mouseUpPos = Input.mousePosition;
		Vector3 torq = (mouseUpPos - mouseDownPos);
		torq = new Vector3(torq.y, -torq.x, 0f);
		testrigid.AddTorque (torq*rotationforce);
		mouseDownPos = mouseUpPos;
	}

	/// there is no zoom shit is a lie
	void zoomChange(){
		//zoom in
		if (zoomCounter != zoomLimit) {
			testObj.localScale = testObj.localScale * (1 + zoomPower);
			++zoomCounter;
		} /*else if (zoomCounter != (-zoomLimit)) {
			testObj.localScale = testObj.localScale * (1 - zoomPower);
			--zoomCounter;
		}*/

		//zoom out
		//testObj.localScale = testObj;
	
	}


	//OBJECT ROTATION FOLLOWS MOUSE PATH
	void RotateObject (){	//rotatation speed based on mouse position 
		
		mousePos = cam.ScreenToWorldPoint (Input.mousePosition);
		Vector3 newRot =  (mousePos - testObj.position);
		newRot = new Vector3(Mathf.Round(newRot.y), -Mathf.Round(newRot.x), 0f);
		testObj.Rotate(newRot * rotationforce * Time.deltaTime, Space.World);

	}

	//OBJECT POSITION FOLLOWS MOUSE PATH
	void ObjectFollowMouse(){ 
		
		mousePos = cam.ScreenToWorldPoint (Input.mousePosition);
		testObj.position = new Vector3(mousePos.x, mousePos.y, 0f);	

	}

	//HELPER FUNCTIONS

	//RETURNS MOUSE AXIS WITH CENTER OF SCREEN AS 0,0 AND ROUNDED NICE NUMBERS
	Vector3 objAxis(Vector3 uglyPos){ 
		Vector3 nicePos =  (uglyPos - testObj.position);
	//	nicePos ;
		return nicePos; 
	}
}
