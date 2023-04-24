using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using DG.Tweening;
using System.Runtime.InteropServices.WindowsRuntime;

public class UltimateBlaster : MonoBehaviour
{
    public BlasterType blasterType;
    public enum BlasterType
    {
        None,
        Pistol,
        Rifle
    }

    [Header("Normal/Ads Mode")]
    public Transform weaponT;
    public Transform attackPoint;

    [SerializeField] private Transform weaponNormalPos;
    [SerializeField] private Transform weaponAdsPos;
    [SerializeField] private Transform weaponReloadPos;
    [SerializeField] private Transform weaponAwayPos;

    [Header("Recoil")]
    public Transform weaponModelT;
    public bool lockWeaponModelRecoil;
    public Transform weaponModelNormalPos;
    public Transform weaponModelRecoilPos;

    [Header("GlowChange")]
    public List<MeshRenderer> chipsetsLeft;
    public List<MeshRenderer> chipsetsRight;
    public Material glowMat;
    public Material normalMat;
    private int currGlowIndex;

    [Header("MagazineDisplay")]
    public UiBar bar_magazineFill;
    private Inventory inv;
    private Combat ranged;

    [Header("RotatingBalls")]
    public Transform rotationCenter;
    public float rotationBaseSpeed;
    private float rotationSpeed;
    private float boostedSpeed;

    [Header("HeatColor")]
    public Color color_normal;
    public Color color_heat;

    public float boostMultiplier = 3f;
    /// public float boostBuildUpDuration = 0.15f;
    public float boostFadeOutSpeed = 20f;

    private bool rotationBoosted;

    private bool rotationReversed;

    private Transform playerCamPos;

    private void CustomStart()
    {
        inv = PlayerReferences.instance.inventory;
        ranged = PlayerReferences.instance.combat;

        if(bar_magazineFill != null )
            bar_magazineFill.UpdateElement(1, 1);

        rotationSpeed = rotationBaseSpeed;

        if (blasterType == BlasterType.None)
            playerCamPos = PlayerReferences.instance.cam.transform;
    }

    private void OnEnable() 
    {
        CustomStart();
        SubscribeToEvents();
    }

    private void OnDisable() 
    { 
        UnsubscribeToEvents(); 
    }

    private void SubscribeToEvents()
    {
        inv.OnReload += DisplayMagazineReload;
        inv.OnWeaponSwitch += DisplayMagazineReload;
        ranged.OnProjectileSpawn += DisplayMagazineReload;
    }

    private void UnsubscribeToEvents()
    {
        inv.OnReload -= DisplayMagazineReload;
        inv.OnWeaponSwitch -= DisplayMagazineReload;
        ranged.OnProjectileSpawn -= DisplayMagazineReload;
    }

    private void Update()
    {
        RotateBalls();

        // boost rotation

        // return to normal rotationSpeed
        if (!rotationBoosted)
        {
            rotationSpeed -= boostFadeOutSpeed * Time.deltaTime;

            if (rotationSpeed < rotationBaseSpeed)
            {
                rotationSpeed = rotationBaseSpeed;
                boostedSpeed = rotationSpeed;
            }
        }

        // charging
        rotationReversed = ranged.state == Combat.CycleState.charging;

        // special case, no weapon model at all -> move attack point
        if(blasterType == BlasterType.None)
        {
            attackPoint.position = playerCamPos.position;
            print("attackPoint is at " + playerCamPos.transform.name);
        }
    }

    public void Fire(float force, float cooldown, float heat)
    {
        DoGlowStep();
        DisplayMagazine(false);

        /// float boostDuration = cooldown * 0.25f;
        BoostRotation(force * boostMultiplier, 0.25f);

        UpdateHeatColor(heat);
    }

    private void DoGlowStep()
    {
        if (chipsetsLeft.Count == 0)
            return;

        currGlowIndex++;

        if (currGlowIndex == chipsetsLeft.Count) 
            currGlowIndex = 0;

        for (int i = 0; i < chipsetsLeft.Count; i++)
        {
            if(i == currGlowIndex)
            {
                chipsetsLeft[i].material = glowMat;
                chipsetsRight[i].material = glowMat;
            }
            else
            {
                chipsetsLeft[i].material = normalMat;
                chipsetsRight[i].material = normalMat;
            }
        }
    }

    private void DisplayMagazineReload()
    {
        DisplayMagazine(true);
    }

    private void DisplayMagazine(bool reload)
    {
        if (bar_magazineFill == null) return;

        Magazine mag = inv.GetCurrentWeaponAbilityMagazine();
        int magazineSize = mag.magazineSize;
        int currMagazineLeft = mag.ammoLeft -  1;

        if (reload)
            currMagazineLeft += 1;

        bar_magazineFill.UpdateElement(currMagazineLeft, magazineSize);
    }

    private void BoostRotation(float amount, float boostDuration)
    {
        rotationBoosted = true;

        boostedSpeed = rotationSpeed += amount;

        rotationSpeed = boostedSpeed;

        StopAllCoroutines();
        StartCoroutine(ResetRotationBoost(boostDuration));
    }

    private IEnumerator ResetRotationBoost(float boostDuration)
    {
        yield return new WaitForSecondsRealtime(boostDuration);

        rotationBoosted = false;
    }

    private void RotateBalls()
    {
        if (rotationCenter == null) return;

        float inverseMultiplier = rotationReversed ? -1 : 1;
        rotationCenter.Rotate(new Vector3(0f, 0f, rotationSpeed * inverseMultiplier * Time.deltaTime));
    }

    public void UpdateHeatColor(float heat)
    {
        if (bar_magazineFill == null) return;

        bar_magazineFill.ChangeColor(Color.Lerp(color_normal, color_heat, heat));
    }

    #region Getters

    public Vector3 GetNormalLocalPos() { return weaponNormalPos == null ? Vector3.zero : weaponNormalPos.localPosition; }
    public Vector3 GetAdsLocalPos() { return weaponAdsPos == null ? Vector3.zero : weaponAdsPos.localPosition; }
    public Vector3 GetReloadLocalPos() { return weaponReloadPos == null ? Vector3.zero : weaponReloadPos.localPosition; }
    public Vector3 GetAwayLocalPos() { return weaponAwayPos == null ? Vector3.zero : weaponAwayPos.localPosition; }

    public Quaternion GetNormalLocalRot() { return weaponNormalPos == null ? Quaternion.identity : weaponNormalPos.localRotation; }
    public Quaternion GetAdsLocalRot() { return weaponAdsPos == null ? Quaternion.identity : weaponAdsPos.localRotation; }
    public Quaternion GetReloadLocalRot() { return weaponReloadPos == null ? Quaternion.identity : weaponReloadPos.localRotation; }
    public Quaternion GetAwayLocalRot() { return weaponAwayPos == null ? Quaternion.identity : weaponAwayPos.localRotation; }


    #endregion
}
