using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ProjectileAssetsSection", menuName = "RcLab/System/Hardcoded/ProjectileAssetsSection")]
public class ProjectileAssetsSection : WeaponAssetsSection
{
    public List<string> projectileNames;

    public override string GetWeaponName(int index)
    {
        return projectileNames[index];
    }
}
