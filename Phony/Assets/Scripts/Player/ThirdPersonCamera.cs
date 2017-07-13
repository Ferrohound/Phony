using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;
/* Controls the third person camera.
 */
[System.Serializable]
public class ThirdPersonCamera {
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool lockCursor = true;

    /// <summary>
    /// Maximum and minimum natural distances that the camera likes to keep from the player.
    /// Third item is actual distance.
    /// Linear interpolation
    /// Can go below if the camera would be inside something. 
    /// </summary>
    public float[] Distances;
    /// <summary>
    /// Angles associated with max and min natural distances. Degrees. 
    /// Third item is actual angle
    /// Linear Interpolation
    /// </summary>
    public float[] stableAngles;

    private PlayerController m_c;
    private Camera cam;
    private Quaternion m_CharacterTargetRot;    //for internal calculations
    private Quaternion q, q1;
    private Quaternion targetRotation;
    private bool m_cursorIsLocked = true;

    public float cameraAxisRotation;           //Degrees
    public float angleOffset;                  //to keep third perspective from rotating about
    public float moveSensitivity; 

    //For internal calculations
    private RaycastHit rhit;
    private Vector3 targetCamLocation;
    private float msr;
    private int layermask = ~(1 << 14);

    public void Init(PlayerController character, Camera camera){
        m_CharacterTargetRot = character.transform.localRotation;
        m_c = character;
        cam = camera;

        Distances = new float[3];
        Distances[0] = 6f;         //Natural max
        Distances[1] = 2f;          //Natural min
        Distances[2] = 4f;          //placehold value
        stableAngles = new float[3];
        stableAngles[0] = 30f;      //Natural max
        stableAngles[1] = 0f;      //Natural min
        stableAngles[2] = 10f;       //placehold value

        cameraAxisRotation = 180;
        angleOffset = 0f;

        moveSensitivity = 0.2f;
    }


    public void LookRotation(){
        //Control the direction of the character
        if (m_c.inputV.x != 0){//no rotation required
            /* FOR WHATEVER FUCK REASON THIS DOESN'T WORK. I DON'T UNDERSTAND WHY. 
             * THAT'S WHY I IMPLEMENTED THE DUMB CODE BELOW
             * :D
             * will probably fix for dumb glitches later
            m_CharacterTargetRot = Quaternion.FromToRotation(m_c.transform.forward, m_c.inputV) * m_c.transform.localRotation;

            q = Quaternion.Slerp(m_c.transform.localRotation, m_CharacterTargetRot, 0.02f);
            float angle, angle2; 
            angle = Mathf.Abs(Quaternion.Angle(m_c.transform.localRotation, q)) * Mathf.Sign(Vector3.Dot(m_c.transform.up, new Vector3(q.x, q.y, q.z)));
            angle2 = Vector3.Angle(m_c.transform.forward, m_c.inputV);
            cameraAxisRotation -= angle;
            Debug.Log(angle);
            Debug.Log(cameraAxisRotation);
            Debug.Log(angle2);
            */
            float angle = Vector3.Angle(m_c.transform.forward, m_c.inputV);
            angle = Mathf.Lerp(0, angle, 0.1f);
            angle *= Mathf.Sign(Vector3.Dot(m_c.transform.up, Vector3.Cross(m_c.transform.forward, m_c.inputV)));
            m_c.transform.localRotation = Quaternion.AngleAxis(angle, m_c.transform.up) * m_c.transform.localRotation;
            cameraAxisRotation -= angle;
        }

        if (m_c.cameraControl){
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;
            if (CrossPlatformInputManager.GetAxisRaw("Fire1") != 0){
                cameraAxisRotation += yRot;
                stableAngles[2] -= xRot;
                if (stableAngles[2] > stableAngles[0]){
                    stableAngles[2] = stableAngles[0];
                } else if (stableAngles[2] < stableAngles[1]) {
                    stableAngles[2] = stableAngles[1];
                }
            }
            msr = CrossPlatformInputManager.GetAxisRaw("Mouse ScrollWheel");
            msr /= 1000; //to keep it in [-1, 1]
            if (msr != 0){
                Distances[2] -= msr * moveSensitivity;
                if (Distances[2] > Distances[0]){
                    Distances[2] = Distances[0];
                } else if (Distances[2] < Distances[1]){
                    Distances[2] = Distances[1];
                }
            }
        } 
        
        //control the location and rotation of the camera
        //up angle rotation first, then the rotation around the up vector second
        targetCamLocation = Quaternion.AngleAxis(cameraAxisRotation, Vector3.up) * Quaternion.AngleAxis(-stableAngles[2], Vector3.right) * Vector3.forward * Distances[2];
        //cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, targetCamLocation, 0.5f);
        //prevents the camera from going inside colliders
        if(Physics.Raycast(m_c.transform.position, m_c.transform.TransformDirection(targetCamLocation), out rhit, Distances[2], layermask)){
            targetCamLocation = m_c.transform.InverseTransformPoint(rhit.point);
        }
        cam.transform.localPosition = targetCamLocation;
        Vector3 t = -cam.transform.localPosition;
        t.y = 0;
        //cam.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, t); //this form is slerp - able
        cam.transform.LookAt(m_c.transform, m_c.transform.up);
        UpdateCursorLock();
    }

    public void SetCursorLock(bool value){
        lockCursor = value;
        if (!lockCursor){//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock(){
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate(){
        if (Input.GetKeyUp(KeyCode.Escape)){
            m_cursorIsLocked = false;
        } else if (Input.GetMouseButtonUp(0)){
            m_cursorIsLocked = true;
        }
        if (m_cursorIsLocked){
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else if (!m_cursorIsLocked){
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q){
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
        return q;
    }
}