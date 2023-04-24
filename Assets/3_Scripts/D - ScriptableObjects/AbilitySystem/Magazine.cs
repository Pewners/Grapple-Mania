using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
// Data Structure
// one reason to change:
// if the magazine variables need to change
public class Magazine
{
    [Tooltip("Type in the exact name of the Projectile Prefab in the Resources Folder.")]
    public string projectileName = "";

    public int magazineSize = 30;
    public float reloadTime = 1f;

    public bool allowPartialReload;

    public AudioClipData reloadSound;

    // internal
    [HideInInspector] public string identifier;
    [HideInInspector] public bool reloading;
    [HideInInspector] public bool partialReloading;
    public int ammoLeft = 0;

    public bool IsFull() { return ammoLeft >= magazineSize; }

    [HideInInspector] public string currProjctileSectionName;
    [HideInInspector] public int selection_ProjectileSection;
    [HideInInspector] public int selection_ProjectileName;
}
