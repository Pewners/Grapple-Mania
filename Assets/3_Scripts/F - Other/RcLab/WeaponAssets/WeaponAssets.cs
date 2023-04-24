using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using System.Runtime.CompilerServices;
using System.Linq;

public class WeaponAssets : MonoBehaviour
{
    public static WeaponAssets instance;

    [Header("Simple Weapons")]
    public List<WeaponAssetsSection> sections_simple;

    [Header("Complex Weapons")]
    public List<WeaponAssetsSection> sections_complex;

    [Header("Projectiles")]
    public List<WeaponAssetsSection> sections_projectiles;

    private bool listsGenerated;
    private List<RangedWeaponAsset> allWeaponAssets;
    private List<string> allProjectileNames;

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

    public int GetAllWeaponSectionAmount() { return sections_simple.Count + sections_complex.Count; }
    public int GetProjectileSectionAmount() { return sections_projectiles.Count; }

    public RangedWeapon GetWeapon(int section, string name)
    {
        WeaponAssetsSectionSimple simpleSection = (WeaponAssetsSectionSimple)sections_simple[section];

        for (int i = 0; i < simpleSection.weaponAssets.Count; i++)
        {
            if (simpleSection.weaponAssets[i].weapon.name == name)
                return simpleSection.weaponAssets[i].weapon;
        }

        Debug.LogError("Weapon could not be found");
        return null;
    }

    public RangedWeaponContainer GetWeaponContainer(int section, string name)
    {
        WeaponAssetsSectionComplex complexSection = (WeaponAssetsSectionComplex)sections_complex[section];

        for (int i = 0; i < complexSection.weaponContainers.Count; i++)
        {
            if (complexSection.weaponContainers[i].name == name)
                return complexSection.weaponContainers[i];
        }

        Debug.LogError("WeaponContainer could not be found");
        return null;
    }

    private void GenerateAllWeaponAndProjectileAssets()
    {
        allWeaponAssets = new List<RangedWeaponAsset>();
        allProjectileNames = new List<string>();

        // weapons
        for (int i = 0; i < sections_simple.Count; i++)
        {
            for (int y = 0; y < sections_simple[i].GetWeaponAmount(); y++)
            {
                allWeaponAssets.Add(sections_simple[i].GetWeaponAsset(y));
            }
        }

        // projectiles
        for (int i = 0; i < sections_projectiles.Count; i++)
        {
            for (int y = 0; y < sections_projectiles[i].GetWeaponAmount(); y++)
            {
                allProjectileNames.Add(sections_projectiles[i].GetWeaponName(y));
            }
        }

        listsGenerated = true;
    }

    #region Getters

    public RangedWeapon GetRandomWeaponAndProjectile()
    {
        if (!listsGenerated) GenerateAllWeaponAndProjectileAssets();

        int randomWeapon = UnityEngine.Random.Range(0, allWeaponAssets.Count);

        int randomProjectileOutOfRecommended = UnityEngine.Random.Range(0, allWeaponAssets[randomWeapon].recommendedProjectiles.Count);

        print("randomWeaponTry " + randomWeapon + " randomProjectileTry " + randomProjectileOutOfRecommended);

        print("randomWeapon " + randomWeapon + " -> " + allWeaponAssets[randomWeapon].weapon.weaponName + "\n"
            + " randomProjectile " + randomProjectileOutOfRecommended + " -> " + allWeaponAssets[randomWeapon].recommendedProjectiles[randomProjectileOutOfRecommended]);

        RangedWeapon copiedWeapon = Instantiate(allWeaponAssets[randomWeapon].weapon);
        copiedWeapon.magazine.projectileName = allWeaponAssets[randomWeapon].recommendedProjectiles[randomProjectileOutOfRecommended];

        return copiedWeapon;
    }

    public List<string> LoadSectionNames()
    {
        List<string> values = new List<string>();
        for (int i = 0; i < sections_projectiles.Count; i++)
            values.Add(sections_projectiles[i].sectionName);
        return values;
    }

    public List<string> LoadProjectileNames(string sectionName)
    {
        List<string> values = new List<string>();
        for (int i = 0; i < sections_projectiles.Count; i++)
        {
            if (sections_projectiles[i].sectionName == sectionName)
            {
                ProjectileAssetsSection section = (ProjectileAssetsSection)sections_projectiles[i];
                for (int y = 0; y < section.projectileNames.Count; y++)
                    values.Add(section.projectileNames[y]);
            }
        }
        print($"Found {values.Count} projectile in section with name {sectionName}");
        return values;
    }

    public string GetFirstProjectileSectionName()
    {
        return sections_projectiles[0].sectionName;
    }

    #endregion
}
