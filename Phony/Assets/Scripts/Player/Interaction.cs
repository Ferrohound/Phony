using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/* This class allows the character to interact with objects, doors, and people. 
 * Things can also be picked up and thrown, assuming that they implement the Grabable Interface
 */
public class Interaction : MonoBehaviour {

    public Transform leftT, rightT, FPL, FPR; //locations of left and right hand. Use hands in rig for this
    public Transform left, right;   //items in left and right hand

    public bool bothHands; //whether both hands are being used for an item
	
    public int simuPoints;          //Number of points to simulate
    public float simuDelta;         //Time in which to accelerate into future
    public float throwSpeed;        //Maximum speed of a throw
    public float throwTime;         //Time to charge a throw
    public float sensitivity;       //mouse scrollwheel sensitivity
    private float curCharge;        //Current speed charge of a throw
    private Vector3[] vPoints;      //Points to simulate for linerender
    //private bool fpILA;             //In first person, whether you have been looking at an item
    //Components
    private LineRenderer lr;
    private PlayerController pc;
    private Transform cam;
    private World w;

    //FOR INTERNAL CALCULATION
    private float h;                //how long the button has been held down
    private RaycastHit rhit;
    private Collider[] collt;
    private int callFlag;           //so update can call functions in fixedupdate
    private Vector3[] renderPoints;
    private bool leftDown, rightDown, fDown, f2Down;

	void Start () {
        InitializeVariables();

	}
	
	void Update () {
        //dropping items
        leftDown = CrossPlatformInputManager.GetButtonDown("Left");
        rightDown = CrossPlatformInputManager.GetButtonDown("Right");
        fDown = CrossPlatformInputManager.GetButton("Fire1");
        f2Down = CrossPlatformInputManager.GetButton("Fire2");
        if (left != null && leftDown){
		    StartCoroutine(Drop (0));
        } else if (right != null && rightDown) {
			StartCoroutine (Drop (1));
        } else if (pc.Perspective == 0) {//first person
            if (left != null && fDown) {//Left throw is being charged
                lr.enabled = true;
                UpdateThrowSpeed();
                //ThrowPrediction(leftT.position, cam.forward);
                ThrowPrediction(FPL.position, cam.forward);
            } else if (right != null && f2Down) {//Right throw is being charged
                lr.enabled = true;
                UpdateThrowSpeed();
                //ThrowPrediction(rightT.position, cam.forward);
                ThrowPrediction(FPR.position, cam.forward);
            } else if (left != null && CrossPlatformInputManager.GetButtonUp("Fire1")) {//release left throw
                lr.enabled = false;
                callFlag = 0;//call throw in fixed update
            } else if (right != null && CrossPlatformInputManager.GetButtonUp("Fire2")) {//release right throw
                lr.enabled = false;
                callFlag = 1;//call throw in fixed update
            } else if(cam!=null && Physics.Raycast(cam.position, cam.forward, out rhit, 3.0f)){//figure out whether there is an interactable in front of you
                if (rhit.transform.GetComponent<GameItem>() != null){
                    //do both hands stuff checking before anything else here


                    if (leftDown && left == null){//pick up item
                        left = rhit.transform;
                        left.GetComponent<GameItem>().Interact(pc, 1);//ignore collisions
                    }
                    else if (rightDown && right == null){//pick up item
                        right = rhit.transform;
                        right.GetComponent<GameItem>().Interact(pc, 2);
                    } else {//update ui
                        //like maybe highlight the item or have stuff pop up
                    }
                } else if ((leftDown || rightDown) && rhit.transform.GetComponent<Interactable>() != null) {
                    rhit.transform.GetComponent<Interactable>().Interact(pc, 0); //default behaviour
                }
            }
        } else {//third person and item is next to you. If multiple items, choose the one that is closest to forward
            if (!pc.itemsAvailable()) return;
            if (CrossPlatformInputManager.GetButtonDown("Left")) { 
                left = pc.thirdPersonPickUp().transform;
                left.GetComponent<GameItem>().Interact(pc, 1);//ignore collisions
            } else if(CrossPlatformInputManager.GetButtonDown("Right")){
                right = pc.thirdPersonPickUp().transform;
                right.GetComponent<GameItem>().Interact(pc, 2);
            }
        }
	}

    void FixedUpdate() {
        switch (callFlag) { 
            case 0:
                Throw(true);
                callFlag = -1;
                break;
            case 1:
                Throw(false);
                callFlag = -1;
                break;
            default: break;
        }
        if (left != null) {
            left.localPosition = Vector3.zero;
        }
        if (right != null) {
            right.localPosition = Vector3.zero;
        }
    }

    private void InitializeVariables(){
        simuPoints = 40;
        vPoints = new Vector3[simuPoints];
        renderPoints = new Vector3[simuPoints / 2];
        simuDelta = 0.05f;
        throwSpeed = 20f;
        throwTime = 1.0f;
        curCharge = throwSpeed / 2f;
        sensitivity = 0.001f;
        pc = GetComponent<PlayerController>();
        cam = Camera.main.transform;
        lr = GetComponentInChildren<LineRenderer>();
        w = GameObject.FindGameObjectWithTag("CONTROL").GetComponent<World>();
     
        callFlag = -1;
		if(lr!=null){
			lr.enabled = false;
			lr.positionCount = simuPoints;
			lr.alignment = LineAlignment.View;
		}
    }

    private void CheckItemsNearby() {
        if (pc.Perspective == 0) {//check if what you are looking at is pickupable
            
        } else {//check if anything near you is pickupable
            
        }
    }
    private void UpdateThrowSpeed() {
        curCharge += CrossPlatformInputManager.GetAxis("Mouse ScrollWheel")*sensitivity;
        if (curCharge < 0) {
            curCharge = 0f;
        } else if (curCharge > throwSpeed) {
            curCharge = throwSpeed;
        }
        //Debug.Log(curCharge);
    }
    /// <summary>
    /// Predicts the trajectory of a thrown object.&#13;&#10;
    /// Uses the velocity verlet algorithm to perform the prediction.
    /// The Line Renderer is used to show the points
    /// </summary>
    /// <param name="start">The starting point of the throw in world space</param>
    /// <param name="direction">The direction of the throw in world space</param>
    private void ThrowPrediction(Vector3 start, Vector3 direction){
        //Simulation
        Vector3 aV0 = w.GetGVector(start, 1.0f);
        Vector3 aV1;
        Vector3 vV0 = direction.normalized * curCharge + pc.Velocity;
        vPoints[0] = start; 
        for (int i = 1; i < vPoints.Length; i++) {
            vPoints[i] = vPoints[i - 1] + vV0 * simuDelta + 0.5f * simuDelta * simuDelta * aV0;
            aV1 = w.GetGVector(vPoints[i], 1.0f);
            vV0 += 0.5f * simuDelta * (aV0 + aV1);
            aV0 = aV1;
        }
        for (int i = 0; i < vPoints.Length; i = i + 2) {
            renderPoints[i / 2] = vPoints[i];
        }
        //Line Rendering
        lr.SetPositions(renderPoints);
        lr.material.mainTextureScale = new Vector2(Vector3.Distance(renderPoints[0], renderPoints[1]),1f); //work on it some more
    }
    /// <summary>
    /// Throws the object with given curCharge speed along direction of camera. 
    /// </summary>
    /// <param name="side">True for left, false for right</param>
    private void Throw (bool side) {
        if (side) { 
            left.GetComponent<Rigidbody>().velocity = cam.forward.normalized * curCharge+pc.Velocity;
            left.GetComponent<GameItem>().Interact(pc, 0);
            left = null;
        } else {
            right.GetComponent<Rigidbody>().velocity = cam.forward.normalized * curCharge+pc.Velocity;
            right.GetComponent<GameItem>().Interact(pc, 0);
            right = null;
        }
        curCharge = throwSpeed / 2f;
    }
	
	//player has items in both hands, both items have GameItem components
	void Combine(){
		GameItem L = left.GetComponent<GameItem>();
		GameItem R = right.GetComponent<GameItem>();
		
		Ingredients ing = new Ingredients(0, L.name, R.name);
		Recipe result = Recipes.Cook(ing);
		
		if (result == null)
			return;
		
		Debug.Log(result._name);
		//do other stuff=================================================TO DO
		
		if(!GameItem.iDatabase.ItemBank.ContainsKey(result._name))
			return;
		
		GameObject newItem = GameItem.iDatabase.ItemBank[result._name];
		
		//play an animation probably ================================== TO DO
		
		//destroy the two held items
		Destroy(left.gameObject);
		Destroy(right.gameObject);
		
		//instantiate the new item and set its parent to the left hand
		Instantiate(newItem, leftT);
		left = newItem.transform;
		
		//set bothHands to false
		bothHands = false;
		
	}


	//dropping item. calls game item scrip to turn on colider after 0.3 secs

	private IEnumerator Drop(int item) {
		yield return new WaitForSeconds(0.3f);
		if (item == 0) {
			left.GetComponent<GameItem>().Interact (pc, 0);
            pc.CheckItemTrigger(left.gameObject);
			left = null;
		} else {
			right.GetComponent<GameItem>().Interact (pc, 0);
            pc.CheckItemTrigger(right.gameObject);
			right = null;
		}
		yield return null;

	}
		
}