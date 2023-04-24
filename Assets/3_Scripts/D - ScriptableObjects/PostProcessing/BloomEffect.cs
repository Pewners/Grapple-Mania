using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBloom", menuName = "RcLab/Effects/PostProcessing/BloomEffect")]
public class BloomEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.Bloom; }

    public float threshold = 1;
    public float intensity = 1;
}
