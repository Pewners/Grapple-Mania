using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Effect", menuName = "RcLab/Effects/Animation")]
public class AnimationEffect : Effect
{
    public override EffectType GetEffectType() { return EffectType.Animation; }
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.None; }
}
