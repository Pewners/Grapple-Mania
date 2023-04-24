using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFilmGrain", menuName = "RcLab/Effects/PostProcessing/FilmGrainEffect")]
public class FilmGrainEffect : PostProcessingEffect
{
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.FilmGrain; }

    [Range(0f, 1f)] public float intensity;
    [Range(0f, 1f)] public float response = 0.8f;
}
