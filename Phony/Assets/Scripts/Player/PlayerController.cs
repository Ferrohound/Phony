using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (Rigidbody))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Interaction))]
/* This class controls the main character's movement and camera, in a 3D space. It is heavily
 * tied to the Gravity Script, of which there should be only one per world.
 * 
 * Throwing mechanics are implemented.
 * 
 * It also controls the IO when vehicles are being used
 */
public class PlayerController : MonoBehaviour{
	
	//persistant player
	public static PlayerController Instance;
	
    [Serializable]
    public class MovementSettings{
        public float maxSpeed = 4f;             //desired walking speed
        public float freeAcceleration = 4f;     //desired acceleration while not on the ground
        public float restoringForce = 14f;       //constant that controls how quickly the controller goes to the desired speed
        public float jumpSpeed = 6f;            //change in velocity when the controller jumps
    }

    public Camera cam;
    public MovementSettings movementSettings = new MovementSettings();
    public FirstPersonCamera mouseLook = new FirstPersonCamera();
    public ThirdPersonCamera mouseLook2 = new ThirdPersonCamera();
    public Interaction m_inter;
    public World world;
    public Animator m_anim;
    /// <summary>
    /// Controls whether the player is in first or third person.
    /// 0 - First person
    /// 1 - Third person
    /// </summary>
    private int CAMERAPERSPECTIVE;

    /// <summary>
    /// 0 - first person.
    /// 1 - third person
    /// </summary>
    public int Perspective {
        get { return CAMERAPERSPECTIVE; }
    }

    /// <summary>
    /// True if in a vehicle. False if not. 
    /// </summary>
    private bool vehicle = false;

    public bool InVehicle {
        get { return vehicle;  }
    }

    /// <summary>
    /// Whether input controls the player's camera.
    /// </summary>
    public bool cameraControl = true;

    /// <summary>
    /// Whether the camera is controlled at all
    /// </summary>
    public bool cameraEnable = true;

    /// <summary>
    /// Whether PlayerController is allowed to move
    /// </summary>
    public bool movementControl = true;

    private Rigidbody m_RigidBody;
    public CapsuleCollider m_Capsule;
    private float m_YRotation, shellOffset, groundCheckDistance, angle;
    private Vector3 m_GroundContactNormal;
    private bool m_Jump, m_PreviouslyGrounded, m_Jumping, grounded;
    private Vector3 inputVector;
    private List<GameObject> itemsInReach; 
    //private bool jumped;
    //private float jumpWait;

    public Vector3 inputV {
        get { return inputVector; }
    }
    
    //VARIABLES USED FOR INTERNAL CALCULATION
    public Vector3 posOld, posNew, temp;
    private Vector2 input;
    private RaycastHit hitInfo;
    private Quaternion q;
    private float wait;

    private Vector3 relativeVelocity;

    public Vector3 Velocity{
        get { return m_RigidBody.velocity; }
    }
    public bool Grounded{
        get { return grounded; }
    }
    public bool Jumping{
        get { return m_Jumping; }
    }
	
	//persistant player, don't destroy this object on load
	void Awake(){
		if(Instance == null){
			DontDestroyOnLoad (gameObject);
			Instance = this;
		} else if(Instance!=this) {
			Destroy(gameObject);
		}
	}
	
    private void Start(){
        cam = Camera.main;
        m_RigidBody = GetComponent<Rigidbody>();
        foreach(CapsuleCollider c in GetComponents<CapsuleCollider>()){
            if (!c.isTrigger) {
                m_Capsule = c; break; //make sure to get the right capsule collider
            }
        }
        m_inter = GetComponent<Interaction>();
        m_anim = GetComponentInChildren<Animator>();
        mouseLook.Init (this, cam);
        mouseLook2.Init(this, cam);
        world = GameObject.FindGameObjectWithTag("CONTROL").GetComponent<World>();          //get world script
        CAMERAPERSPECTIVE = 0;
        shellOffset = 0.1F; groundCheckDistance = 0.1F;
        posOld = gameObject.transform.position;
        //jumped = false;
        //jumpWait = 1.0f;
        itemsInReach = new List<GameObject>();
    }

    private void Update(){
        ControlUp();

        if (cameraControl && CrossPlatformInputManager.GetButtonDown("Perspective")) {
            StartCoroutine(switchCameras(1.2F));
        }
        if (CrossPlatformInputManager.GetButtonDown("Console")) {
            if (World.consoleUp) {
                world.ChangeConsoleState(false);
                cameraControl = true;
            } else {
                world.ChangeConsoleState(true);
                cameraControl = false;
            }
        }
    }

    void LateUpdate() {
        RotateView();
    }

    private void FixedUpdate(){
        posNew = gameObject.transform.position;
        GroundCheck();
        ControlUp();
        input = GetInput();
        if (!cameraControl) input = Vector2.zero;
        if (vehicle) { 
            //pass input
            return;
        }
        if (!movementControl) {
            posOld = posNew;
            return;
        }
        if (grounded) {
            switch (CAMERAPERSPECTIVE){
                case 0: //First person
                    inputVector = transform.forward * input.y + transform.right * input.x;
                    temp = Vector3.ProjectOnPlane(inputVector, hitInfo.normal) * movementSettings.maxSpeed;
                    temp -= relativeSurfaceVelocity(); //holds the difference between desired and actual speed
                    temp *= movementSettings.restoringForce*m_RigidBody.mass;
                    //Debug.Log(relativeSurfaceVelocity());
                    m_RigidBody.AddForce(temp, ForceMode.Force);
                    break;
                default: //Third person
                    //make sure the camera forward is correctly projected onto plane
                    inputVector = transform.InverseTransformDirection(cam.transform.forward);
                    inputVector.y = 0;
                    inputVector = transform.TransformDirection(inputVector);
                    inputVector = inputVector * input.y + cam.transform.right * input.x;
                    //
                    temp = Vector3.ProjectOnPlane(inputVector, hitInfo.normal) * movementSettings.maxSpeed;
                    temp -= relativeSurfaceVelocity(); //holds the difference between desired and actual speed
                    temp *= movementSettings.restoringForce*m_RigidBody.mass;
                    m_RigidBody.AddForce(temp, ForceMode.Force);
                    break;
            }
        } else {
            switch (CAMERAPERSPECTIVE){
                case 0:
                    inputVector = transform.forward * input.y + transform.right * input.x;
                    temp = Vector3.ProjectOnPlane(inputVector, world.GetGVector(m_RigidBody));
                    temp *= movementSettings.freeAcceleration;
                    m_RigidBody.AddForce(temp, ForceMode.Acceleration);
                    break;
                default:
                    inputVector = cam.transform.forward * input.y + cam.transform.right * input.x;
                    temp = Vector3.ProjectOnPlane(inputVector, world.GetGVector(m_RigidBody)) * movementSettings.maxSpeed;
                    temp *= movementSettings.freeAcceleration;
                    m_RigidBody.AddForce(temp, ForceMode.Acceleration);
                    break;
            }
        }
        posOld = posNew;
        /*
        if (Time.time - wait > jumpWait) {
            jumped = false;
        }*/
    }

    void OnTriggerEnter(Collider other) {
        GameItem k = other.GetComponent<GameItem>();
        if (k != null && !k.held) {
            foreach (GameObject g in itemsInReach) {
                if (other.gameObject.GetInstanceID() == g.GetInstanceID()) return;
            }
            itemsInReach.Add(other.gameObject);
            itemsInReach.Sort((x,y) => x.GetInstanceID().CompareTo(y.GetInstanceID()));
            //Debug.Log("Added: " + other.GetInstanceID());
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.GetComponent<GameItem>() != null) {
            int i = itemsInReach.FindIndex(x => x.GetInstanceID() == other.gameObject.GetInstanceID());
            if (i < 0) return;
            itemsInReach.RemoveAt(i);
            //Debug.Log("Removed: " + other.GetInstanceID());
        }
    }
    //=========================================================================================
    private Vector2 GetInput(){
        Vector2 input = new Vector2{
            x = CrossPlatformInputManager.GetAxis("Horizontal"),
            y = CrossPlatformInputManager.GetAxis("Vertical")
        };
        return input;
    }

    private void GroundCheck(){
        m_PreviouslyGrounded = grounded;
        if (Physics.SphereCast(transform.position, m_Capsule.radius*(1.0F*shellOffset), -transform.up, out hitInfo, (m_Capsule.height/2F)+m_Capsule.radius+groundCheckDistance,Physics.AllLayers,QueryTriggerInteraction.Ignore)){
            grounded = true;
            m_GroundContactNormal = hitInfo.normal;
        } else {
            grounded = false;
            m_GroundContactNormal = Vector3.up;
        }
        if (!m_PreviouslyGrounded && grounded && m_Jumping){
            m_Jumping = false;
        }
    }

    private void RotateView(){
        if (CAMERAPERSPECTIVE == 0) {
            mouseLook.LookRotation();
        } else {
            mouseLook2.LookRotation();
        }
    }
    /// <summary>
    /// Controls the up direction of the player
    /// </summary>
    private void ControlUp() {
        temp = world.GetGVector(m_RigidBody);
        q = Quaternion.FromToRotation(transform.up, temp);
        q *= transform.localRotation;
        transform.localRotation = q;
    }
    /// <summary>
    /// Returns the relative velocity between the player and the surface being walked on.
    /// Works better if surface being walked on has a rigidbody attached. 
    /// </summary>
    /// <param name="walkedOn"></param>
    /// <returns></returns>
    private Vector3 relativeSurfaceVelocity() {
        if (hitInfo.rigidbody == null) {
            return Vector3.zero;
        }
        Vector3 t  = hitInfo.point - hitInfo.transform.position;          //point relative to transform
        t = Vector3.Cross(hitInfo.rigidbody.angularVelocity, t)+hitInfo.rigidbody.velocity;//world speed of point 
        t = (posNew - posOld) / Time.fixedDeltaTime - t;       //relative velocity between surface and character
        return t;
    }
    /// <summary>
    /// Controls the changes from first person and third person perspectives.
    /// </summary>
    /// <param name="duration">Approximate duration over which the change occurs</param>
    /// <returns></returns>
    private IEnumerator switchCameras(float duration) {
        cameraControl = false;
        int steps = (int) Mathf.Ceil(duration / Time.deltaTime);
        if (CAMERAPERSPECTIVE == 0){        //first to third person
            CAMERAPERSPECTIVE = 1;
            mouseLook2.cameraAxisRotation = 180f;
            mouseLook2.angleOffset = 0f;
            for (int i = 0; i < steps; i++) {
                mouseLook2.Distances[2] = mouseLook2.Distances[1] + (mouseLook2.Distances[0] - mouseLook2.Distances[1]) * ((float) i) / steps;
                mouseLook2.stableAngles[2] = mouseLook2.stableAngles[1] + (mouseLook2.stableAngles[0] - mouseLook2.stableAngles[1]) * ((float) i) / steps;
                yield return null;
            }
        } else {                            //third to first person
            float cD = mouseLook2.Distances[2];
            float cA = mouseLook2.stableAngles[2];
            for (int i = 0; i < steps; i++ ) {
                mouseLook2.cameraAxisRotation = Mathf.Lerp(mouseLook2.cameraAxisRotation, 180f, 0.4f);
                mouseLook2.stableAngles[2] = cA * (1 - ((float) i) / steps);
                mouseLook2.Distances[2] = cD * (1 - ((float) i) / steps);
                yield return null;
            }
            mouseLook2.cameraAxisRotation = 180f;
            cam.transform.localRotation = Quaternion.identity;                      //HAVE TO DO THIS SO WEIRD SHIT DOESN'T HAPPEN
            cam.transform.localPosition = mouseLook.cameraLocation;
            CAMERAPERSPECTIVE = 0;
        }
        cameraControl = true;
        yield return null;
    }
    /// <summary>
    /// Chooses which object in collider volume to pick up.
    /// Currently: first object in list. Should probably update
    /// </summary>
    public GameObject thirdPersonPickUp() {
        GameObject g =  itemsInReach[0];
        itemsInReach.RemoveAt(0);
        return g;
    }
    public bool itemsAvailable() {
        if (itemsInReach.Count == 0) return false;
        return true;
    }
    /// <summary>
    /// When an item is dropped, return it reach array
    /// </summary>
    /// <param name="g"></param>
    public void CheckItemTrigger(GameObject g) {
        foreach (GameObject c in itemsInReach) {
            if (g.GetInstanceID() == c.GetInstanceID()) return;
        }
        itemsInReach.Add(g);
        itemsInReach.Sort((x, y) => x.GetInstanceID().CompareTo(y.GetInstanceID()));
    }
}