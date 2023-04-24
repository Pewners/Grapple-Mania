using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLensDistortion", menuName = "RcLab/Effects/PostProcessing/LensDistortionEffect")]
public class LensDistortionEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.LensDistortion; }

    [Range(-1f, 1f)] public float intensity;
    [Range(0f, 1f)] public float xMultiplier = 1;
    [Range(0f, 1f)] public float yMultiplier = 1;
    [Range(0.01f, 5f)] public float scale = 1;
}
