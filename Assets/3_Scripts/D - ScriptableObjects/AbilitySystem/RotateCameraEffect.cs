using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCamShake", menuName = "RcLab/Effects/Camera/Shake/RotationShake")]
public class RotateCameraEffect : CameraShakeEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.RotationShake; }

    public float shakeSpeed = 20f;
    public float shakeRange = 2f;
    public Vector3 direction = new Vector3(1f, 0f, 0.5f);
}
