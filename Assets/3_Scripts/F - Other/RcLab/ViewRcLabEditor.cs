using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using TMPro;

public class ViewRcLabEditor : UiView
{
    public GameObject content;

    [Header("References")]
    public TextMeshProUGUI text_currWeaponName;
    public TextMeshProUGUI text_currProjectileName;
    public UiView view_RcLabSelection;

    private Inventory inv;
    private WeaponAssets weaponAssets;

    private void CustomStart()
    {
        inv = PlayerReferences.instance.inventory;
        weaponAssets = FindObjectOfType<WeaponAssets>();
    }

    public override void Show()
    {
        base.Show();

        content.SetActive(true);

        CustomStart();

        LoadCurrentNames();
    }

    public override void Hide()
    {
        base.Hide();

        content.SetActive(false);
    }

    public void LoadCurrentNames()
    {
        RangedWeapon currWeapon = inv.GetCurrentWeapon();
        if (currWeapon == null) return;

        text_currWeaponName.SetText(currWeapon.name);
        text_currProjectileName.SetText(currWeapon.magazine.projectileName);
    }

    #region OnButtonClicks

    public void OnSelectWeaponClick()
    {
        view_RcLabSelection.GetComponent<ViewRcLabSelection>().SetIsWeapon(true);
        view_RcLabSelection.Show();
    }

    public void OnSelectProjectileClick()
    {
        view_RcLabSelection.GetComponent<ViewRcLabSelection>().SetIsWeapon(false);
        view_RcLabSelection.Show();
    }

    public void OnApplyProjectileChangesClick()
    {
        PlayerReferences.instance.inventory.ApplyProjectileChanges();
    }

    public void OnRandomizeClick()
    {
        RangedWeapon randomWeapon = weaponAssets.GetRandomWeaponAndProjectile();
        inv.ChangeMainWeapon(randomWeapon);
        FindObjectOfType<ViewRcLabEditor>().LoadCurrentNames();
    }

    #endregion
}
