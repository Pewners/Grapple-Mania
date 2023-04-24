using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectPlaylist
{
    public List<EffectPlaylistItem> effects;
}

[Serializable]
public class EffectPlaylistItem
{
    [HideInInspector] public string customInspectorName;

    // timing
    public float startDelay;

    // effect
    public Effect effect;

    // general effect modifiers
    [Range(0.1f, 3f)] public float intensity = 1;

    // intensity supported for
    // motion blur
    // lensDistortion
    // filmGrain
    // bloom
    // chromatic abberation
    // vignette
    //
    // cinemachine impulse
    // position move
    // rotation shake
}
