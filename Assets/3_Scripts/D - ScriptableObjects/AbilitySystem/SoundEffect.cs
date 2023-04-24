using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "RcLab/Effects/Sound")]
public class SoundEffect : Effect
{
    public override EffectType GetEffectType() { return EffectType.Sound; }
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.None; }
}
