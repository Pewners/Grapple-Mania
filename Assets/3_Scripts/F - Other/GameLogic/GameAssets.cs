using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    // 1 - Materials
    // 2 - Effect Sounds
    // 3 - Music Sounds
    // 4 - Muzzle Effects
    // 5 - Impact Effects

    #region Singleton

    public static GameAssets instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion



    #region Prefab Lists && Enum Definitions


    /* 1 - Materials */

    public List<MaterialPref> materialPrefs;
    public enum MaterialType
    {
        GlowBlue,
        GlowRed,
        GlowOrange,
        GlowYellow,
        GlowViolet,
        GlowPink,
        GlowGreen,
        GlowWhite,
        GlowGray,
        Black
    }


    /* 2 - Effect Sounds */
    /* 3 - Music Sounds */

    public enum EffectSound
    {
        None,
        Gun,
        Launcher,
        WeaponSwap,
        Explosion1,
        Dash,
        GameLost,
        Locked,
        OpenBox,
        Pickup,
        Reload,
        Sucess,
        DoorOpen,
        HoverOverButton,
        StartRun,
        EnemyDestruction,
        Spin
    }

    public enum MusicSound
    {
        Main
    }

    public List<EffectAudioClip> effectAudioClips;
    public List<MusicAudioClip> musicAudioClips;


    /* 4 - Muzzle Effects */

    public List<MuzzleEffectPref> muzzleEffectPrefs;

    public enum MuzzleEffect
    {
        None,
        RcLab_BasicMuzzleFlash
    }


    /* 5 - Impact Effects */

    public List<ImpactEffectPref> impactEffectPrefs;

    public enum ImpactEffect
    {
        None,
        RcLab_Blue, RcLab_Orange, RcLab_Violet, RcLab_White,
    }


    /* Sound Effects */
    public SoundDatabase soundDatabase;


    #endregion



    #region Getters

    public Material GetMaterial(MaterialType materialPrefType)
    {
        for (int i = 0; i < materialPrefs.Count; i++)
        {
            if (materialPrefs[i].materialPrefType == materialPrefType)
                return materialPrefs[i].material;
        }

        Debug.LogError("Material could not be found");
        return null;
    }

    public GameObject GetMuzzleEffectPrefab(MuzzleEffect type, string effectVariation)
    {
        if (type == MuzzleEffect.None)
            return null;

        print("searching for: " + type + effectVariation);

        for (int i = 0; i < muzzleEffectPrefs.Count; i++)
        {
            if (muzzleEffectPrefs[i].muzzleEffectType == type)
            {
                for (int y = 0; y < muzzleEffectPrefs[i].effectObjects.Count; y++)
                {
                    print(muzzleEffectPrefs[i].muzzleEffectType + " effect variation: " + muzzleEffectPrefs[i].effectObjects[y].effectVariation);

                    if (muzzleEffectPrefs[i].effectObjects[y].effectVariation == effectVariation)
                        return muzzleEffectPrefs[i].effectObjects[y].effectPref;
                }
            }
        }

        Debug.LogError("MuzzleEffectPref could not be found");
        return null;
    }

    public GameObject GetImpactEffectPrefab(ImpactEffect type, string effectVariation)
    {
        print("Sasuke GameDev: " + type.ToString() + " " + effectVariation);

        for (int i = 0; i < impactEffectPrefs.Count; i++)
        {
            if (impactEffectPrefs[i].impactEffectType == type)
            {
                for (int y = 0; y < impactEffectPrefs[i].effectObjects.Count; y++)
                {
                    if (impactEffectPrefs[i].effectObjects[y].effectVariation == effectVariation)
                        return impactEffectPrefs[i].effectObjects[y].effectPref;
                }
            }
        }

        Debug.LogError("ImpactEffectPref could not be found");
        return null;
    }

    #endregion

}

#region Prefab Data Classes



// General

[Serializable]
public class MaterialPref
{
    public GameAssets.MaterialType materialPrefType;
    public Material material;
}



// Audio

[Serializable]
public class EffectAudioClip
{
    public GameAssets.EffectSound effect;
    public AudioClip audioClip;
    [Range(0f, 1f)] public float baseVolume = 0.5f;
}

[Serializable]
public class MusicAudioClip
{
    public GameAssets.MusicSound music;
    public AudioClip audioClip;
    [Range(0f, 1f)] public float baseVolume = 0.5f;
}



// Effects

[Serializable]
public class MuzzleEffectPref
{
    public string name;
    public GameAssets.MuzzleEffect muzzleEffectType;
    public List<EffectGameObject> effectObjects;
}

[Serializable]
public class ImpactEffectPref
{
    public string name;
    public GameAssets.ImpactEffect impactEffectType;
    public List<EffectGameObject> effectObjects;
}

#endregion


#region Custom ObjectPrefab Classes

/// sub class for colored effects
[Serializable]
public class EffectGameObject
{
    public string effectVariation;
    public GameObject effectPref;
}

#endregion

/// for example MuzzleEffectPref -> MuzzleEffect (can be modified in size and color)
#region Modified Data Classes

[Serializable]
public class MuzzleEffectModifiable
{
    public GameAssets.MuzzleEffect muzzleEffectType;
    public string effectVariation;
    public float size = 1;
}

[Serializable]
public class ImpactEffectModifiable
{
    public GameAssets.ImpactEffect impactEffectType;
    public string effectVariation;
    public float size = 1;
}

#endregion