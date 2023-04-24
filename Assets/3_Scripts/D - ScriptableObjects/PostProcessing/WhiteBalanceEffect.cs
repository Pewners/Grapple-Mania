using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWhiteBalance", menuName = "RcLab/Effects/PostProcessing/WhiteBalanceEffect")]
public class WhiteBalanceEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.WhiteBalance; }

    [Range(-100f, 100f)] public float temperature;
    [Range(-100f, 100f)] public float tint;
}
