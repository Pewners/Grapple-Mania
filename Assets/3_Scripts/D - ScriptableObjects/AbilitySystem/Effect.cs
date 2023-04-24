using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : ScriptableObject
{
    [HideInInspector] public float intensityModifier = 1f;

    public enum EffectType
    {
        None,
        Camera,
        Sound,
        Animation,
        Stat
    }

    public enum CameraEffectType
    {
        None,
        NormalShake,
        CinemachineImpulse,
        PositionMove,
        ZoomShake,
        RotationShake,
        Vignette,
        ChromaticAberration,
        Bloom,
        DepthOfField,
        FilmGrain,
        WhiteBalance,
        LensDistortion,
        MotionBlur,
        PaniniProjection,
        ColorAdjustments,
        CamShake, // the basic milk shake asset cam shake
    }

    public virtual EffectType GetEffectType() { return EffectType.None; }

    public virtual CameraEffectType GetCameraEffectType() { return CameraEffectType.None; }
}
