using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

[CreateAssetMenu(fileName = "New WeaponAssetsSection", menuName = "RcLab/System/Hardcoded/WeaponAssetsSectionSimple")]
public class WeaponAssetsSectionSimple : WeaponAssetsSection
{
    public List<RangedWeaponAsset> weaponAssets;

    public override string GetWeaponName(int index)
    {
        return weaponAssets[index].weapon.weaponName;
    }

    public override RangedWeapon GetWeapon(int index)
    {
        return weaponAssets[index].weapon;
    }

    public override RangedWeaponAsset GetWeaponAsset(int index)
    {
        return weaponAssets[index];
    }
}

[Serializable]
public class RangedWeaponAsset
{
    public RangedWeapon weapon;
    public List<string> recommendedProjectiles;
}
