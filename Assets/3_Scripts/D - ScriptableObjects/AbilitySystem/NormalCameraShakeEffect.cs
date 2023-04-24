using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCamShake", menuName = "RcLab/Effects/Camera/Shake/BasicShake")]
public class NormalCameraShakeEffect : CameraShakeEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.NormalShake; }

    public float amplitude = 0.5f;
    public float frequency = 10;
}
