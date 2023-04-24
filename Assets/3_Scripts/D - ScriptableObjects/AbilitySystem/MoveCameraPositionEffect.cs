using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCamShake", menuName = "RcLab/Effects/Camera/Shake/MovePosition")]
public class MoveCameraPositionEffect : CameraShakeEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.PositionMove; }

    public float intensity = 1.0f;
    public bool moveX = false;
    public bool moveY = false;
    public bool moveZ = true;
}
