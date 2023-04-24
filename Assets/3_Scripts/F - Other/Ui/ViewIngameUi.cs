using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using System;

public class ViewIngameUi : UiView
{
    public GameObject content;

    [Header("References")]
    private Inventory inv;
    private Combat ranged;

    public List<IngameUiAsset> uiAssets;

    public enum IngameUiType
    {
        Main,
        RocketControl
    }

    // public PotionObjectCreator potionCreator_activePotion;

    private void Start()
    {
        /// for now ingame Ui is always active
        Show();
    }

    private void CustomStart()
    {
        inv = PlayerReferences.instance.inventory;
        ranged = PlayerReferences.instance.combat;

        // subscribe to events
        inv.OnReload += UpdateIngameUi;
        inv.OnWeaponSwitch += UpdateIngameUi;

        ranged.OnProjectileSpawn += UpdateMagazineInfo;
    }

    public override void Show()
    {
        base.Show();

        content.SetActive(true);

        CustomStart();

        UpdateIngameUi();
    }

    /// see subscribed events
    public void UpdateIngameUi()
    {
        
    }

    public void UpdateMagazineInfo()
    {
        
    }

    public override void Hide()
    {
        base.Hide();

        content.SetActive(false);
    }

    public void ShowIngameUi(IngameUiType type)
    {
        for (int i = 0; i < uiAssets.Count; i++)
        {
            if (uiAssets[i].type == type)
                uiAssets[i].gameObject.SetActive(true);
            else
                uiAssets[i].gameObject.SetActive(false);
        }
    }
}

[Serializable]
public class IngameUiAsset
{
    public ViewIngameUi.IngameUiType type;
    public GameObject gameObject;
}