using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatEffect", menuName = "RcLab/Effects/Stats")]
public class StatEffect : Effect
{
    public override EffectType GetEffectType() { return EffectType.Stat; }
    public override CameraEffectType GetCameraEffectType() { return CameraEffectType.None; }

    public StatEffectType statEffectType;

    public float duration;
    public int stacks = 1;

    public bool enableValueStacking = false;
    public int maxReacheableStacks = 1;

    public bool enableDurationStacking = false;
    public float maxReacheableDuration;
}
