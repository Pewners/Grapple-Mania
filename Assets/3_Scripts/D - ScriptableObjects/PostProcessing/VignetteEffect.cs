using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVignette", menuName = "RcLab/Effects/PostProcessing/VignetteEffect")]
public class VignetteEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.Vignette; }

    public Color color;
    [Range(0f, 1f)] public float intensity;
    [Range(0f, 1f)] public float smoothness;
    public bool rounded;
}