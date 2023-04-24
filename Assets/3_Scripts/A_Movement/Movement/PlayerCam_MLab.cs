using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // I use DoTween for the camera effects, so this reference is needed
using Cinemachine;
using RcLab;
using MilkShake;

// Dave MovementLab - PlayerCam
///
// Content:
/// - first person camera rotation
/// - camera effects such as fov changes, tilt or cam shake
/// - headBob effect while walking or sprinting
///
// Note:
/// This script is assigned to the player (like every other script).
/// It rotates the camera vertically and horizontally, while also rotating the orientation of the player, but only horizontally.
/// -> Most scripts then use this orientation to find out where "forward" is.
/// 
/// If you're a beginner, just ignore the effects and headBob stuff and focus on the rotation code.


public class PlayerCam_MLab : MonoBehaviour
{
    public bool locked;

    [Header("Assignables")]
    public Transform camT; // the camerHolder
    public Transform orientation; // reference to the orientation of the player
    public Camera cam; // the camera (inside the cameraHolder)
    public Camera weaponCam;
    public CinemachineVirtualCamera firstPersonCam;
    public CinemachineFreeLook thirdPersonCam;

    [Header("Modes")]
    public LayerMask firstPersonCullingMask;
    public LayerMask thirdPersonCullingMask;

    [Header("Effects")]
    public float baseFov = 90f;
    public float fovTransitionTime = 0.25f; // how fast the cameras fov changes
    public float tiltTransitionTime = 0.25f; // how fast the cameras tilt changes

    [Header("Effects - HeadBob")]
    [HideInInspector] public bool hbEnabled; // this bool is changed by the PlayerMovement script, depending on the MovementState
    public float hbAmplitude = 0.5f; // how large the headBob effect is
    public float hbFrequency = 12f; // how fast the headBob effect plays

    [Header("Effects - Shake")]

    public MilkShake.Shaker p_milkShakeShaker;
    public MilkShake.ShakePreset f_milkShakePreset;
   

    [Header("References")]
    public PlayerInput input;

    // the rest are just private variables to store information

    private float hbToggleSpeed = 3f; // if the players speed is smaller than the hbToggleSpeed, the headBob effect wont play
    private Vector3 hbStartPos;
    private Rigidbody rb;

    [HideInInspector] public float mouseX;
    [HideInInspector] public float mouseY;

    private float multiplier = 0.01f;

    private float xRotation;
    private float yRotation;

    private CameraMode mode;
    public enum CameraMode
    {
        FirstPerson,
        ThirdPerson
    }

    private void Start()
    {
        // store the startPosition of the camera
        hbStartPos = cam.transform.localPosition;

        // get the components
        camT = GameObject.Find("CameraHolder").transform;
        rb = GetComponent<Rigidbody>();
        input = PlayerReferences.instance.input;

        // lock the mouse cursor in the middle of the screen
        Cursor.lockState = CursorLockMode.Locked;
        // make the mouse coursor invisible
        Cursor.visible = false;

        SwitchToCameraMode(CameraMode.FirstPerson);

        AssignAllFeedbacks();
    }

    private void AssignAllFeedbacks()
    {
        //f_cinemachineImpulse = p_cinemachineImpulse.GetFeedbackOfType<MMF_CinemachineImpulse>();
        //f_basicShake = p_basicShake.GetFeedbackOfType<MMF_CameraShake>();
        //f_positionMove = p_positionMove.GetFeedbackOfType<MMF_Position>();
        //f_zoomShake = p_zoomShake.GetFeedbackOfType<MMF_CameraZoom>();
        //f_rotationShake = p_rotationShake.GetFeedbackOfType<MMF_RotationShake>();
    }

    private void Update()
    {
        if (locked) return;

        RotateCamera();

        // if headBob is enabled, start the CheckMotion() function
        /// which then starts -> PlayMotion() and ResetPosition()
        /// deactivated for now, headbob causes too many problems
        if (hbEnabled && false)
        {
            CheckMotion();
            cam.transform.LookAt(FocusTarget());
        }
    }

    public void RotateCamera()
    {
        // first get the mouseX and Y Input
        float mouseX = Input.GetAxisRaw("Mouse X") * input.camSenseX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * input.camSenseY;

        // then calculate the x and y rotation of your camera using this formula:
        /// yRotation plus mouseX
        /// xRotation minus mouseY
        yRotation += mouseX * input.camSenseX * multiplier;
        xRotation -= mouseY * input.camSenseY * multiplier;

        // make sure that you can't look up or down more than 90* degrees
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        // rotate the camera holder along the x and y axis
        camT.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        // rotate the players orientation, but only along the y (horizontal) axis
        if (mode == CameraMode.FirstPerson)
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        if (mode == CameraMode.ThirdPerson)
        {
            Vector3 dirToOrientation = orientation.position - thirdPersonCam.transform.position;
            orientation.forward = new Vector3(dirToOrientation.x, 0f, dirToOrientation.z);
        }
    }

    #region Camera Modes

    public void SwitchToCameraMode(CameraMode _mode)
    {
        // first person
        if(_mode == CameraMode.FirstPerson)
        {
            mode = CameraMode.FirstPerson;
            firstPersonCam.gameObject.SetActive(true);
            weaponCam.gameObject.SetActive(true);
            thirdPersonCam.gameObject.SetActive(false);
            cam.cullingMask = firstPersonCullingMask;
        }

        // third person
        else if(_mode == CameraMode.ThirdPerson)
        {
            mode = CameraMode.ThirdPerson;
            thirdPersonCam.gameObject.SetActive(true);
            weaponCam.gameObject.SetActive(false);
            firstPersonCam.gameObject.SetActive(false);
            cam.cullingMask = thirdPersonCullingMask;
        }
    }

    #endregion


    #region Save and Load Roatations

    public CamRotation SaveCamRotation()
    {
        CamRotation currRotation = new CamRotation();
        currRotation.rotation = camT.rotation;
        currRotation.xRotation = xRotation;
        currRotation.yRotation = yRotation;
        return currRotation;
    }

    public void LoadCamRotation(CamRotation camRotation, bool instant)
    {
        camT.rotation = camRotation.rotation;
        xRotation = camRotation.xRotation;
        yRotation = camRotation.yRotation;
    }

    #endregion

    /// double click the field below to show all fov, tilt and cam shake code
    /// Note: For smooth transitions I use the free DoTween Asset!
    #region Fov, Tilt and CamShake

    public void DoFovZoomPercentage(float zoomPercentage, float transitionTime = -1)
    {
        float multiplier = (100-zoomPercentage)/100;
        DoFov(baseFov * multiplier, transitionTime);
    }

    /// function called when starting to wallrun or starting to dash
    /// a simple function that just takes in an endValue, and then smoothly sets the cameras fov to this end value
    public void DoFov(float endValue, float transitionTime = -1)
    {
        if (transitionTime == -1)
            transitionTime = fovTransitionTime;

        DOVirtual.Float(firstPersonCam.m_Lens.FieldOfView, endValue, transitionTime, v =>
        {
            firstPersonCam.m_Lens.FieldOfView = v;
            weaponCam.fieldOfView = v;
        });
    }

    public void ResetFov(float transitionTime = -1)
    {
        if (transitionTime == -1)
            transitionTime = fovTransitionTime;

        DOVirtual.Float(firstPersonCam.m_Lens.FieldOfView, baseFov, transitionTime, v =>
        {
            firstPersonCam.m_Lens.FieldOfView = v;
            weaponCam.fieldOfView = v;
        });
    }

    /// function called when starting to wallrun
    /// smoothly tilts the camera
    public void DoTilt(float zTilt)
    {
        cam.transform.DOLocalRotate(new Vector3(0, 0, zTilt), tiltTransitionTime);
    }

    public void ResetTilt()
    {
        cam.transform.DOLocalRotate(Vector3.zero, tiltTransitionTime);
    }

    public void PlayShake(CameraShakeEffect camShakeEffect)
    {
        Effect.CameraEffectType cameraEffectType = camShakeEffect.GetCameraEffectType();

        float intensityModifier = camShakeEffect.intensityModifier;

        print("now playing camShake of type " + cameraEffectType);

        // milk shake shake
        if (cameraEffectType == Effect.CameraEffectType.CamShake)
        {
            MilkShakeCamShakeEffect shakeEffect = (MilkShakeCamShakeEffect)camShakeEffect;
            f_milkShakePreset.ShakeType = shakeEffect.shakeType;
            f_milkShakePreset.Strength = shakeEffect.strength;
            f_milkShakePreset.Roughness = shakeEffect.roughness;
            f_milkShakePreset.FadeIn = shakeEffect.fadeIn;
            f_milkShakePreset.FadeOut = shakeEffect.fadeOut;
            f_milkShakePreset.PositionInfluence = shakeEffect.positionInfluence;
            f_milkShakePreset.RotationInfluence = shakeEffect.rotationInfluence;
            Shaker.ShakeAll(f_milkShakePreset);
        }
    }

    #endregion


    /// double click the field below to show all headBob code
    /// as a beginner, just ignore this code for now
    #region HeadBob

    // Important Note: I learned how to create this code by following along with a YouTube tutorial
    // Credits: https://www.youtube.com/watch?v=5MbR2qJK8Tc&ab_channel=Hero3D

    private void CheckMotion()
    {
        // get the current speed of the players rigidbody (y axis excluded)
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        ResetPosition();

        // check if the speed is high enough to activate the headBob effect
        if (speed < hbToggleSpeed) return;

        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion)
    {
        // take the calculated motion and apply it to the camera
        cam.transform.localPosition += motion * Time.deltaTime;
    }

    private void ResetPosition()
    {
        if (cam.transform.localPosition == hbStartPos) return;

        // smoothly reset the position of the camera back to normal
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, hbStartPos, 1 * Time.deltaTime);
    }

    private Vector3 FootStepMotion()
    {
        // use Sine and Cosine to create a smooth looking motion that swings from left to right and from up to down 
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * hbFrequency) * hbAmplitude;
        pos.x += Mathf.Cos(Time.time * hbFrequency * 0.5f) * hbAmplitude * 2f;
        return pos;
    }


    private Vector3 FocusTarget()
    {
        // make sure the camera focuses (Looks at) a point 15 tiles away from the player
        // this stabilizes the camera
        Vector3 pos = new Vector3(transform.position.x, camT.position.y, transform.position.z);
        pos += camT.forward * 15f;
        return pos;
    }

    #endregion

    #region Setters

    public void SetHeadBobEnabled(bool enabled) { hbEnabled = enabled; }

    #endregion
}

public class CamRotation
{
    public Quaternion rotation;
    public float xRotation;
    public float yRotation;
}
