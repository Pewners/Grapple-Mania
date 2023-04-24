using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDepthOfField", menuName = "RcLab/Effects/PostProcessing/DepthOfFieldEffect")]
public class DepthOfFieldEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.DepthOfField; }

    public float start;
    public float end;
}
