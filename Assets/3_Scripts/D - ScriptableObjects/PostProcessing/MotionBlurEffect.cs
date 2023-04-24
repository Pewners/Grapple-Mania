using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMotionBlur", menuName = "RcLab/Effects/PostProcessing/MotionBlurEffect")]
public class MotionBlurEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.MotionBlur; }

    [Range(0f, 1f)] public float intensity;
    [Range(0f, 0.2f)] public float clamp = 0.05f;
}
