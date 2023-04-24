
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// Database Structure //

[Serializable]
public class SoundDatabase
{
    [SerializeField] public Dictionary<Category, SoundCategory> categories;

    public enum Category
    {
        Hand,
        Sword,
        Bow,
        RangedWeapon,
        Projectile,
        Armor,
        Player,
        Explosion,
        Elemental,
        Ui,
        Electrical
    }

    public AudioClip GetSound(Category category, string soundName)
    {
        return categories[category].audioClipLookup[soundName];
    }
}

[CreateAssetMenu(fileName = "New SoundDatabase", menuName = "RcLab/System/Sound/SoundDatabaseCategory")]
public class SoundCategory : ScriptableObject
{
    public Dictionary<string, AudioClip> audioClipLookup;
}
