using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChromaticAberration", menuName = "RcLab/Effects/PostProcessing/ChromaticAberrationEffect")]
public class ChromaticAberrationEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.ChromaticAberration; }

    [Range(0f, 1f)] public float intensity;
}
