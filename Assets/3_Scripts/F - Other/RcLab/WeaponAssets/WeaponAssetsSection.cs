using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

[Serializable]
public class WeaponAssetsSection : ScriptableObject
{
    public string sectionName;

    public virtual string GetWeaponName(int index)
    {
        return "NotImplemented";
    }

    public virtual RangedWeapon GetWeapon(int index)
    {
        return null;
    }

    public virtual RangedWeaponContainer GetWeaponContainer(int index)
    {
        return null;
    }

    public virtual RangedWeaponAsset GetWeaponAsset(int index)
    {
        return null;
    }

    // just wanted to learn how to cast stuff haha
    public int GetWeaponAmount()
    {
        // the "as" operator returns the object when the type is compatible

        var myProjectileCast = this as ProjectileAssetsSection;
        if(myProjectileCast != null)
        {
            return myProjectileCast.projectileNames.Count;
        }

        var myWeaponAssetsSimpleCast = this as WeaponAssetsSectionSimple;
        if (myWeaponAssetsSimpleCast != null)
        {
            return myWeaponAssetsSimpleCast.weaponAssets.Count;
        }

        var myWeaponAssetsComplexCast = this as WeaponAssetsSectionComplex;
        if (myWeaponAssetsComplexCast != null)
        {
            return myWeaponAssetsComplexCast.weaponContainers.Count;
        }

        Debug.LogError("Dave please");
        return 0;
    }
}