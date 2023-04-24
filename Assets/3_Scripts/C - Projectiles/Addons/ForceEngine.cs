using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


[RequireComponent(typeof(Rigidbody))]
public class ForceEngine : ProjectileAddon
{
    // engine setup
    [Tooltip("Needed for correct bouncing off walls!")]
    public bool useVelocityForward = true;
    public float forwardForce = 1000;
    public float upwardForce;
    public float rightForce;

    public AnimationCurve forceMultiplierCurve;
    public float curveDuration;

    public float maxRbVelocity = -1;

    public float engineStartDelay = 0.05f;
    public float engineStopDelay = -1;

    // force fadeout
    public bool useForceFadeout = false;
    public float fadeoutDuration;
    [Range(0f,1f)]
    public float minForce = 0f;
    private float fadeoutMultiplier;

    // special - pulsive
    public float pulseIntervall = 2f;
    private float timeToNextPulse;
    public float offPulseDrag = 0.5f;

    // torque settings
    public Vector3 torqueDirection;


    public ForceMode forceMode = ForceMode.Force;

    public EngineMode engineMode = EngineMode.continuous;
    public enum EngineMode
    {
        continuous,
        pulsive
    }


    private Rigidbody rb;
    private bool engineActive;

    private float timeElapsed;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // otherwise velocity would be 0
        if (useVelocityForward) rb.AddForce(transform.forward, ForceMode.Force);

        Invoke(nameof(StartEngine), engineStartDelay);
    }

    private void StartEngine()
    {
        engineActive = true;

        // ForceFadeout
        if (useForceFadeout)
            DOVirtual.Float(fadeoutMultiplier, minForce, fadeoutDuration, v =>
            {
                fadeoutMultiplier = v;
                print("fadoutMultiplier: " + v);
            });

        if(engineStopDelay != -1)
            Invoke(nameof(StopEngine), engineStopDelay);
    }

    private void StopEngine()
    {
        engineActive = false;
    }

    private void Update()
    {
        if (!engineActive) return;

        // EngineMode - Continuous
        if (engineMode == EngineMode.continuous)
            AddEngineForce();

        // EngineMode - Pulsive
        else if(engineMode == EngineMode.pulsive)
        {
            timeToNextPulse -= Time.deltaTime;

            if(timeToNextPulse <= 0)
            {
                AddEngineForce();
                timeToNextPulse = pulseIntervall;
            }
            else
            {
                rb.drag = offPulseDrag;
            }
        }

        // Add Torque
        AddEngineTorque();

        // Limit velocity if needed
        if(maxRbVelocity != -1 && rb.velocity.magnitude > maxRbVelocity)
            rb.velocity = rb.velocity.normalized * maxRbVelocity;

        timeElapsed += Time.deltaTime;
    }

    private void AddEngineForce()
    {
        Vector3 forwardDirection = transform.forward;

        if (useVelocityForward) forwardDirection = rb.velocity.normalized;

        Vector3 forceToApply = forwardDirection * forwardForce + transform.up * upwardForce + transform.right * rightForce;

        forceToApply *= forceMultiplierCurve.Evaluate(timeElapsed / curveDuration);
        print("Evaluation: " + forceMultiplierCurve.Evaluate(timeElapsed / curveDuration));

        if (forceMode == ForceMode.Force)
            rb.AddForce(forceToApply * Time.deltaTime, forceMode);

        if (forceMode == ForceMode.Impulse)
            rb.AddForce(forceToApply, forceMode);
    }

    private void AddEngineTorque()
    {
        transform.Rotate(torqueDirection * Time.deltaTime);
    }
}
