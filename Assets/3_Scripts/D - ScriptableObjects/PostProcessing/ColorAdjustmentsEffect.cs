using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewColorAdjustments", menuName = "RcLab/Effects/PostProcessing/ColorAdjustmentsEffect")]
public class ColorAdjustmentsEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.ColorAdjustments; }

    public float postExposure;
    [Range(-100f, 100f)] public float contrast;
    [Range(-100f, 100f)] public float saturation;
    public Color colorFilter;
}
