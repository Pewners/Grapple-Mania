using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCamShake", menuName = "RcLab/Effects/Camera/Shake/ZoomShake")]
public class ZoomCameraEffect : CameraShakeEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.ZoomShake; }

    public float targetFov = 80f;
}
