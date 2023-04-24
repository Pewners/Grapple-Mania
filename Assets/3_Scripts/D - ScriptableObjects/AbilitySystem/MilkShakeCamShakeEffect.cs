using MilkShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCamShake", menuName = "RcLab/Effects/Camera/UniversalShake")]
public class MilkShakeCamShakeEffect : CameraShakeEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.CamShake; }

    public ShakeType shakeType;

    public float strength;
    public float roughness;

    public float fadeIn;
    public float fadeOut;

    public Vector3 positionInfluence;
    public Vector3 rotationInfluence;
}
