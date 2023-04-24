using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewCamShake", menuName = "RcLab/Effects/Camera/Shake/CinemachineImpulse")]
public class CinemachineImpulseEffect : CameraShakeEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.CinemachineImpulse; }

    public Vector3 velocity = default;
}
