using RcLab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RandomWeaponSlot : MonoBehaviour, IInteractable
{
    [Header("References")]
    public TextMeshProUGUI text_content;

    private RangedWeapon randomWeapon;

    private WeaponAssets weaponAssets;
    private Inventory inv;

    private void Start()
    {
        weaponAssets = WeaponAssets.instance;
        inv = PlayerReferences.instance.inventory;

        Setup();
    }

    public void Setup()
    {
        randomWeapon = weaponAssets.GetRandomWeaponAndProjectile();

        text_content.SetText(randomWeapon.weaponName + "\n" + "x" + "\n" + randomWeapon.magazine.projectileName);
    }

    public void Interact()
    {
        inv.ChangeMainWeapon(randomWeapon);
    }
}
