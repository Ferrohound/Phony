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

    public bool bothHands;          //whether both hands are being used for an item
	
    public int simuPoints;          //Number of points to simulate
    public float simuDelta;         //Time in which to accelerate into future
    public float throwSpeed;        //Maximum speed of a throw
    public float throwTime;         //Time to charge a throw
    public float sensitivity;       //mouse scrollwheel sensitivity
    private float curCharge;        //Current speed charge of a throw
    private Vector3[] vPoints;      //Points to simulate for linerender
    //private bool fpILA;             //In first person, whether you have been looking at an item
    float carryDistance = .75f;
	
	//Components
    private LineRenderer lr;
    private PlayerController pc;
    public Transform cam;
    private World w;

    //FOR INTERNAL CALCULATION
    private float h;                //how long the button has been held down
    private RaycastHit rhit;
    private Collider[] collt;
    private int callFlag;           //so update can call functions in fixedupdate
    private Vector3[] renderPoints;
    private bool fDown, f2Down, fHold, f2Hold, fUp, f2Up;
    private float sT, sT2, hT; //start time, hold time. Using for determining when to throw
    private bool lP, rP; //whether an item was picked up this time

    void Start () {
        InitializeVariables();

	}
	
	void Update () {
		Vector3 f = cam.forward;
		Vector3 up = cam.up;
		Vector3 RD = Vector3.Cross(up, f);
		Vector3 LD = Vector3.Cross(f, up);
		
		FPL.position = (cam.position/* - new Vector3(-3, 0, 0)*/) 
			+ cam.transform.forward * carryDistance + LD * 0.5f;
		FPR.position = (cam.position/* + new Vector3(3, 0, 0)*/) 
			+ cam.transform.forward * carryDistance + RD * 0.5f;
		//Debug.Log(cam.forward);

        fDown = CrossPlatformInputManager.GetButtonDown("Fire1");
        f2Down = CrossPlatformInputManager.GetButtonDown("Fire2");
        fHold = CrossPlatformInputManager.GetButton("Fire1");
        f2Hold = CrossPlatformInputManager.GetButton("Fire2");
		fUp = CrossPlatformInputManager.GetButtonUp("Fire1");
		f2Up = CrossPlatformInputManager.GetButtonUp("Fire2");

        if (fDown) { sT = Time.time; }
        if (f2Down) { sT2 = Time.time; }

		if(left != null && right != null) //if objects are held in both hands
			HUD.Combine.SetActive(true);
		
		if (pc.Perspective == 0) {//first person
            if (left != null && fHold && (Time.time - sT > hT)) {//Left throw is being charged
                lr.enabled = true;
                UpdateThrowSpeed();
                ThrowPrediction(FPL.position, cam.forward);
            } else if (right != null && f2Hold && (Time.time - sT2 > hT)) { //Right throw is being charged
                lr.enabled = true;
                UpdateThrowSpeed();
                ThrowPrediction(FPR.position, cam.forward);
            } else if(right != null && left!= null && CrossPlatformInputManager.GetButtonDown("Combine")) { //WILL HAVE TO CHANGE
				Combine();
			} else if (left != null && fUp && (Time.time - sT > hT)) {//release left throw
				//turn off left throwing UI
				//and combine UI if right isn't null
				//====================================================================TO DO
                HUD.LeftThrow.SetActive(false);
                HUD.Combine.SetActive(false);

                lr.enabled = false;
                callFlag = 0;//call throw in fixed update
            } else if(left != null && fUp && !lP) { //drop
                HUD.LeftThrow.SetActive(false);
                HUD.Combine.SetActive(false);
                StartCoroutine(Drop(0));
            } else if (right != null && f2Up && (Time.time - sT2 > hT)) {//release right throw
				//turn off right throw UI
				//and combine UI if right isn't null
				//====================================================================TO DO
                HUD.RightThrow.SetActive(false);
                HUD.Combine.SetActive(false);
                lr.enabled = false;
                callFlag = 1;//call throw in fixed update
            } else if (right != null && f2Up && !rP){//drop
                HUD.RightThrow.SetActive(false);
                HUD.Combine.SetActive(false);
                StartCoroutine(Drop(1));
            } else if(cam!=null && Physics.Raycast(cam.position, cam.forward, out rhit, 3.0f)){//figure out whether there is an interactable in front of you
                if (rhit.transform.GetComponent<GameItem>() != null){
                    if (fDown && left == null){//pick up item
						//turn off left holding UI element and on leftThrow
						//==============================================================TO DO
						HUD.LeftPickUp.SetActive(false);
						HUD.LeftThrow.SetActive(true);
                        lP = true;
                        if (right != null){
                            HUD.Combine.SetActive(false);
                        }
                        left = rhit.transform;
                        left.GetComponent<GameItem>().Interact(pc, 1);//ignore collisions
                    } else if (f2Down && right == null){//pick up item
						//turn off right holding UI and on rightThrow
						//==============================================================TO DO
						HUD.RightPickUp.SetActive(false);
						HUD.RightThrow.SetActive(true);
                        rP = true;
						if(left != null)
							HUD.Combine.SetActive(false);
						
                        right = rhit.transform;
                        right.GetComponent<GameItem>().Interact(pc, 2); //ignore collisions
                    } else {//update ui
                        //like maybe highlight the item or have stuff pop up
						//if left is null
						if(left == null){
							HUD.LeftPickUp.SetActive(true);
						}
						if(right == null){
							HUD.RightPickUp.SetActive(true);
						}
                    }
                } else if ((fDown || f2Down) && rhit.transform.GetComponent<Interactable>() != null) {
                    rhit.transform.GetComponent<Interactable>().Interact(pc, 0); //default behaviour
                }
            }
			//otherwise, turn off UI stuff =============================================TO DO
			else{
				HUD.LeftPickUp.SetActive(false);
				HUD.RightPickUp.SetActive(false);
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

        if (fUp && lP) lP = false;
        if (f2Up && rP) rP = false;
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
        sensitivity = 2.0f;
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

        hT = 1.0f;
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
		Debug.Log("Combining " + L.itemName + " and " + R.itemName);
        Debug.Log("Combining ID's " + L.item.ID + " and " + R.item.ID);
        //Ingredients ing = new Ingredients(0, L.itemName, R.itemName);
        //Recipe result = Recipes.Cook(ing);
        Recipe result = Recipes.Cook(L.item.ID, R.item.ID);
		
		if (result == null){
			Debug.Log("Result is null!");
			return;
		}
		
		Debug.Log(result._name);
		//do other stuff=================================================TO DO
		
		if(!GameItem.iDatabase.ItemBank.ContainsKey(result._name))
			return;
		
		GameObject newItem = GameItem.iDatabase.ItemBank[result._name];
		
		//play an animation probably ================================== TO DO
		//detirmine which items to destroy
		if(L.itemName == result._item1){
			if(result._dst1){
				Destroy(L.gameObject);
				left = null;
			}
			if(result._dst2){
				Destroy(R.gameObject);
				right = null;
			}
		}else{
			if(result._dst1){
				Destroy(R.gameObject);
				right = null;
			}
			if(result._dst2){
				Destroy(L.gameObject);
				left = null;
			}
		}
		
		//instantiate the new item and set its parent to a free hand
		if(left == null){
			//Instantiate(newItem, FPL);
			GameObject actualItem = Instantiate(newItem, FPL.position, FPL.rotation);
			left = actualItem.transform;
			GameItem LI = left.GetComponent<GameItem>();
			LI.Initialize();
			LI.Interact(pc, 1);//ignore collisions
		} else if(right == null){
			//Instantiate(newItem, FPR);
			GameObject actualItem = Instantiate(newItem, FPR.position, FPR.rotation);
			right = actualItem.transform;
			GameItem RI = right.GetComponent<GameItem>();
			RI.Interact(pc, 1);//ignore collisions
		}
		
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