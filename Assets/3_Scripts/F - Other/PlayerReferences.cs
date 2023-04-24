using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

public class PlayerReferences : MonoBehaviour
{
    public static PlayerReferences instance;

    // components
    public Transform tr;
    public Rigidbody rb;
    public Collider col;
    public Animator animator;

    // main
    public PlayerInput input;
    public PlayerAbilities abilities;
    public ActionPlayer actionPlayer;
    public EffectPlayer effectPlayer;
    public AnimationPlayer animationPlayer;
    
    // movement
    public MovementCore movementCore;
    public Detector detector;
    public PlayerMovementlol movement;
    public PlayerCam_MLab cam;
    public Dashing_MLab dashing;
    public WallRunning_MLab wallRunning;
    public Sliding_MLab sliding;
    public Grappling_MLab grappling;
    public LedgeGrabbing_MLab ledgeGrabbing;

    // combat
    public Stats stats;
    public Inventory inventory;
    public Combat combat;
    public WeaponHolder weaponHolder;

    // old
    public Detector_MLab detectorOld;
    public PlayerMovement_MLab movementOld;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
