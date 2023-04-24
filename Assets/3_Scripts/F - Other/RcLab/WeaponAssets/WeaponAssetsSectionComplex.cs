using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;

[CreateAssetMenu(fileName = "New WeaponAssetsSection", menuName = "RcLab/System/Hardcoded/WeaponAssetsSectionComplex")]
public class WeaponAssetsSectionComplex : WeaponAssetsSection
{
    public List<RangedWeaponContainer> weaponContainers;

    public override string GetWeaponName(int index)
    {
        return weaponContainers[index].name;
    }

    public override RangedWeaponContainer GetWeaponContainer(int index)
    {
        return weaponContainers[index];
    }
}
