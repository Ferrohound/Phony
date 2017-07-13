using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/* Controls the First person camera
 */
[Serializable]
public class FirstPersonCamera{
    public float XSensitivity = 3f;
    public float YSensitivity = 3f;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool lockCursor = true;

    private PlayerController m_c;
    private Camera cam;
    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private Quaternion targetRotation;
    private bool m_cursorIsLocked = true;

    private Quaternion q;

    /// <summary>
    /// Where the camera is located. 
    /// </summary>
    public Vector3 cameraLocation;

    public void Init(PlayerController character, Camera camera){
        m_CharacterTargetRot = character.transform.localRotation;
		if(camera == null)
			camera = Camera.main;
        m_CameraTargetRot = camera.transform.localRotation;
        m_c = character;
        cam = camera;

        cameraLocation = cam.transform.localPosition;
    }


    public void LookRotation(){
		//null check
		if( cam == null)
			cam = Camera.main;

        if (m_c.cameraControl) {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * YSensitivity;

            targetRotation = cam.transform.rotation; //get camera world rotation
            targetRotation *= Quaternion.Euler(-xRot, yRot, 0F);
            m_CharacterTargetRot *= Quaternion.Euler(0F, yRot, 0F);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0F, 0F);

            q = Quaternion.AngleAxis(yRot, m_c.transform.up);
            q *= m_c.transform.localRotation;
            m_c.transform.localRotation = Quaternion.Slerp(m_c.transform.localRotation, q, 0.5F);

            q = Quaternion.AngleAxis(-xRot, Vector3.right);
            q = ClampRotationAroundXAxis(q * cam.transform.localRotation);
            cam.transform.localRotation = Quaternion.Slerp(cam.transform.localRotation, q, 0.5F);
            UpdateCursorLock();
        }
    }

    public void SetCursorLock(bool value){
        lockCursor = value;
        if(!lockCursor){//we force unlock the cursor if the user disable the cursor locking helper
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
        if(Input.GetKeyUp(KeyCode.Escape)){
            m_cursorIsLocked = false;
        } else if(Input.GetMouseButtonUp(0)){
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
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);
        angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);
        return q;
    }
}