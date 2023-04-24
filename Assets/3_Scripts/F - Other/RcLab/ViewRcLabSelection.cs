using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RcLab;

public class ViewRcLabSelection : UiView
{
    public GameObject content;

    [Header("References")]
    public List<Transform> verticalLayoutGroups;
    public GameObject weaponSelectButtonPref;
    public TextMeshProUGUI text_sectionName;

    private WeaponAssets weaponAssets;
    private Inventory inv;
    private int currSection;
    private int sectionAmount;
    private WeaponAssetsSection sectionLoaded;
    private RangedWeaponAsset currentWeaponAsset;

    private bool isWeapon; // weapon or projectile assets
    private bool isSimple; // simple or complex weapons

    private void CustomStart()
    {
        inv = PlayerReferences.instance.inventory;

        weaponAssets = WeaponAssets.instance;
    }

    public override void Show()
    {
        base.Show();

        content.SetActive(true);

        CustomStart();

        currSection = 0;
        sectionAmount = isWeapon ? weaponAssets.GetAllWeaponSectionAmount() : weaponAssets.GetProjectileSectionAmount();
        if (!isWeapon) sectionAmount++; // projectiles have the "recommended" section added

        LoadSection();
    }

    public override void Hide()
    {
        base.Hide();

        content.SetActive(false);
    }

    private void ClearLayoutGroups()
    {
        for (int i = 0; i < verticalLayoutGroups.Count; i++)
        {
            for (int y = 0; y < verticalLayoutGroups[i].childCount; y++)
            {
                Destroy(verticalLayoutGroups[i].GetChild(y).gameObject);
            }
        }
    }

    private void LoadSection()
    {
        ClearLayoutGroups();

        WeaponAssetsSection sectionToLoad = FindSection(isWeapon);

        bool leftGroup = true;
        for (int i = 0; i < sectionToLoad.GetWeaponAmount(); i++)
        {
            Transform group = leftGroup ? verticalLayoutGroups[0] : verticalLayoutGroups[1];

            SelectWeaponButton button = Instantiate(weaponSelectButtonPref, group).GetComponent<SelectWeaponButton>();
            button.Setup(sectionToLoad.GetWeaponName(i), i);

            leftGroup = !leftGroup;
        }

        text_sectionName.SetText(sectionToLoad.sectionName);

        sectionLoaded = sectionToLoad;
    }

    private WeaponAssetsSection FindSection(bool isWeaponSection)
    {
        int realIndex = currSection;
        WeaponAssetsSection sectionToLoad = null;

        // weapon sections
        if (isWeaponSection)
        {
            if (currSection > weaponAssets.sections_simple.Count)
            {
                realIndex -= weaponAssets.sections_simple.Count;
                print("realIndex: " + realIndex);
                sectionToLoad = weaponAssets.sections_complex[realIndex];
                isSimple = false;
            }
            else
            {
                print("realIndex: " + realIndex);
                sectionToLoad = weaponAssets.sections_simple[realIndex];
                isSimple = true;
            }
        }
        // projectile sections
        else
        {
            // recommended section
            if(realIndex == 0)
            {
                ProjectileAssetsSection recommendedSection = new ProjectileAssetsSection();

                if (currentWeaponAsset != null)
                    recommendedSection.projectileNames = currentWeaponAsset.recommendedProjectiles;
                else
                    recommendedSection.projectileNames = new List<string>();

                recommendedSection.sectionName = "Recommended!";
                sectionToLoad = recommendedSection;
            }
            else
            {
                sectionToLoad = weaponAssets.sections_projectiles[realIndex - 1];
            }
        }

        return sectionToLoad;
    }

    #region ButtonClicks

    public void OnSelectWeaponClick(int index)
    {
        // weapon
        if (isWeapon)
        {
            if (isSimple)
            {
                inv.ChangeMainWeapon(sectionLoaded.GetWeapon(index));
                currentWeaponAsset = (sectionLoaded as WeaponAssetsSectionSimple).weaponAssets[index];
            }
            else
            {
                //inv.ChangeMainWeapon(sectionLoaded.GetWeaponContainer(index));
            }
        }

        // projectile
        else
        {
            inv.ChangeMainProjectile(sectionLoaded.GetWeaponName(index));
        }

        FindObjectOfType<ViewRcLabEditor>().LoadCurrentNames();

        Hide();
    }

    public void OnNextButtonClick()
    {
        currSection++;

        if (currSection >= sectionAmount)
            currSection = 0;

        LoadSection();
    }

    public void OnPreviousButtonClick()
    {
        currSection--;

        if (currSection < 0)
            currSection = sectionAmount - 1;

        LoadSection();
    }

    #endregion

    #region Setters

    public void SetIsWeapon(bool value) { isWeapon = value; }

    #endregion
}
