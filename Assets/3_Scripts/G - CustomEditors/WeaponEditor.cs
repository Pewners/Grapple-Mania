using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dave;
using static TreeEditor.TextureAtlas;
using System.Runtime.InteropServices;
using System.Linq;

[CustomEditor(typeof(RangedWeapon))]
public class WeaponEditor : Editor
{
    public SerializedProperty weaponName;

    // stats and settings

    /// baseStats
    public SerializedProperty
        force,
        chargeTime,
        cooldown,
        ammoFireAmount,
        timeBetweenShots,
        verticalAngle,
        horizontalAngle,
        stabilityNormal,
        stabilityWithAds,
        adsFovZoomPercent,
        recoilRigidbodyForce,
        recoilRigidbodyMaxSpeed,
        readyTime,
        awayTime,
        adsTime,
        handsNeeded,
        damageMultiplier,
        enableDamageFalloff,
        damageFalloff,
        weaponEffectAppliers,
        projectileEffectAppliers;

    public SerializedProperty spawnOnlyOnMouseDown;
    public SerializedProperty countSalveAsOneShot;

    public SerializedProperty
        projectileName,
        magazineSize,
        reloadTime,
        allowPartialReload,
        reloadSound;

    public List<string> projectileSectionNames;
    public List<string> projectileNames;
    string lastProjectileSectionName;

    public SerializedProperty spawnPosData;
    public SerializedProperty useCameraAsSpawnPoint;
    public SerializedProperty useStrictCameraForward;

    public SerializedProperty consumeAmmoOnMouseHold;

    public SerializedProperty minCooldown;
    public SerializedProperty cooldownDecreaseOnFire;
    public SerializedProperty timerUntilReset;

    public SerializedProperty enableOverheating;
    public SerializedProperty heatIncreaseOnFire;
    public SerializedProperty timeUntilHeatDecrease;
    public SerializedProperty heatDecreaseSpeed;
    public SerializedProperty heatEffectOnCooldown;

    // sound and vfx
    public SerializedProperty weaponSounds;

    public SerializedProperty shakePreset;

    /// muzzle Effect
    public SerializedProperty
        muzzleEffectType,
        muzzleEffectSize;
    public List<string> _muzzleEffectVariations;
    GameAssets.MuzzleEffect _lastMuzzleEffectType;

    public SerializedProperty postProcessingEffects;

    private WeaponAssets weaponAssets;

    private void OnEnable()
    {
        weaponAssets = FindObjectOfType<WeaponAssets>();

        weaponName = serializedObject.FindProperty("weaponName");

        // stats and settings

        /// baseStats
        force = serializedObject.FindProperty("baseStats").FindPropertyRelative("force");
        damageMultiplier = serializedObject.FindProperty("baseStats").FindPropertyRelative("damageMultiplier");
        chargeTime = serializedObject.FindProperty("baseStats").FindPropertyRelative("chargeTime");
        cooldown = serializedObject.FindProperty("baseStats").FindPropertyRelative("cooldown");
        ammoFireAmount = serializedObject.FindProperty("baseStats").FindPropertyRelative("projectileSpawnAmount");
        timeBetweenShots = serializedObject.FindProperty("baseStats").FindPropertyRelative("timeBetweenSpawns");
        verticalAngle = serializedObject.FindProperty("baseStats").FindPropertyRelative("verticalAngle");
        horizontalAngle = serializedObject.FindProperty("baseStats").FindPropertyRelative("horizontalAngle");
        stabilityNormal = serializedObject.FindProperty("baseStats").FindPropertyRelative("stabilityNormal");
        stabilityWithAds = serializedObject.FindProperty("baseStats").FindPropertyRelative("stabilityWithAds");
        adsFovZoomPercent = serializedObject.FindProperty("baseStats").FindPropertyRelative("adsFovZoomPercent");
        recoilRigidbodyForce = serializedObject.FindProperty("baseStats").FindPropertyRelative("recoilRigidbodyForce");
        recoilRigidbodyMaxSpeed = serializedObject.FindProperty("baseStats").FindPropertyRelative("recoilRigidbodyMaxSpeed");

        /// damage falloff
        enableDamageFalloff = serializedObject.FindProperty("enableDamageFalloff");
        damageFalloff = serializedObject.FindProperty("baseStats").FindPropertyRelative("damageFalloff");
        //damageFalloffStartDistance = serializedObject.FindProperty("baseStats").FindPropertyRelative("damageFalloff").FindPropertyRelative("startDistance");
        //damageFalloffEndDistance = serializedObject.FindProperty("baseStats").FindPropertyRelative("damageFalloff").FindPropertyRelative("endDistance");
        //maxDamageFalloffReductionPercent = serializedObject.FindProperty("baseStats").FindPropertyRelative("damageFalloff").FindPropertyRelative("maxDamageReductionPercent");

        /// handlingStats
        readyTime = serializedObject.FindProperty("baseStats").FindPropertyRelative("handling").FindPropertyRelative("readyTime");
        awayTime = serializedObject.FindProperty("baseStats").FindPropertyRelative("handling").FindPropertyRelative("awayTime");
        adsTime = serializedObject.FindProperty("baseStats").FindPropertyRelative("handling").FindPropertyRelative("adsTime");
        handsNeeded = serializedObject.FindProperty("baseStats").FindPropertyRelative("handling").FindPropertyRelative("handsNeeded");

        spawnOnlyOnMouseDown = serializedObject.FindProperty("spawnOnlyOnMouseDown");
        countSalveAsOneShot = serializedObject.FindProperty("countSalveAsOneShot");

        // magazine
        projectileName = serializedObject.FindProperty("magazine").FindPropertyRelative("projectileNameSelection");
        magazineSize = serializedObject.FindProperty("magazine").FindPropertyRelative("magazineSize");
        reloadTime = serializedObject.FindProperty("magazine").FindPropertyRelative("reloadTime");
        allowPartialReload = serializedObject.FindProperty("magazine").FindPropertyRelative("allowPartialReload");
        reloadSound = serializedObject.FindProperty("magazine").FindPropertyRelative("reloadSound");

        // effects

        weaponEffectAppliers = serializedObject.FindProperty("weaponEffectAppliers");
        projectileEffectAppliers = serializedObject.FindProperty("projectileEffectAppliers");

        // custom spawn pos
        spawnPosData = serializedObject.FindProperty("spawnPosData");
        useCameraAsSpawnPoint = serializedObject.FindProperty("useCameraAsSpawnPoint");
        useStrictCameraForward = serializedObject.FindProperty("useStrictCameraForward");

        consumeAmmoOnMouseHold = serializedObject.FindProperty("consumeAmmoOnMouseHold");

        minCooldown = serializedObject.FindProperty("minCooldown");
        cooldownDecreaseOnFire = serializedObject.FindProperty("cooldownDecreaseOnFire");
        timerUntilReset = serializedObject.FindProperty("timerUntilReset");

        enableOverheating = serializedObject.FindProperty("enableOverheating");
        heatIncreaseOnFire = serializedObject.FindProperty("heatIncreaseOnFire");
        timeUntilHeatDecrease = serializedObject.FindProperty("timeUntilHeatDecrease");
        heatDecreaseSpeed = serializedObject.FindProperty("heatDecreaseSpeed");
        heatEffectOnCooldown = serializedObject.FindProperty("heatEffectOnCooldown");

        // sound and vfx

        weaponSounds = serializedObject.FindProperty("weaponSounds");

        shakePreset = serializedObject.FindProperty("shakePreset");

        muzzleEffectType = serializedObject.FindProperty("muzzleEffect").FindPropertyRelative("muzzleEffectType");
        muzzleEffectSize = serializedObject.FindProperty("muzzleEffect").FindPropertyRelative("size");

        _lastMuzzleEffectType = GameAssets.MuzzleEffect.None;
        _muzzleEffectVariations = new List<string>();

        postProcessingEffects = serializedObject.FindProperty("postProcessingEffects");

        projectileSectionNames = weaponAssets.LoadSectionNames();
        RangedWeapon script = target as RangedWeapon;
        if (script.magazine.currProjctileSectionName == string.Empty)
            script.magazine.currProjctileSectionName = weaponAssets.GetFirstProjectileSectionName();
        projectileNames = weaponAssets.LoadProjectileNames(script.magazine.currProjctileSectionName);

        Debug.Log("WeaponEditor enabled");
    }

    public override void OnInspectorGUI()
    {
        RangedWeapon script = target as RangedWeapon;


        // basic unity inspector
        ///base.OnInspectorGUI();
        ///return;

        /* my custom inspector */

        serializedObject.Update();

        EditorUi.DrawComponentTitle(EditorUiData.Component.RangedWeapon, EditorUi.CustomColor.blue, script.weaponName);
        EditorGUILayout.PropertyField(weaponName);

        #region Stats and Settings

        // Section 1 - Base Stats

        EditorUi.DrawHeader("Base Stats", 1);

        /// damage
        EditorUi.DrawSubHeader("Damage");

        EditorGUILayout.PropertyField(force);
        EditorGUILayout.PropertyField(damageMultiplier);
        EditorGUILayout.PropertyField(enableDamageFalloff);

        if (script.enableDamageFalloff)
        {
            EditorGUILayout.PropertyField(damageFalloff);
        }

        /// attack pattern

        EditorUi.DrawSubHeader("Attack Pattern");

        EditorGUILayout.PropertyField(chargeTime);
        EditorGUILayout.PropertyField(cooldown);
        EditorGUILayout.PropertyField(ammoFireAmount);
        EditorGUILayout.PropertyField(timeBetweenShots);

        /// stability

        EditorUi.DrawSubHeader("Stability");

        EditorGUILayout.PropertyField(stabilityNormal);
        EditorGUILayout.PropertyField(stabilityWithAds);
        EditorGUILayout.PropertyField(adsFovZoomPercent);
        EditorGUILayout.PropertyField(recoilRigidbodyForce);

        if(script.baseStats.recoilRigidbodyForce > 0)
        {
            EditorGUILayout.PropertyField(recoilRigidbodyMaxSpeed);
        }

        /// handling

        EditorUi.DrawSubHeader("Handling");

        EditorGUILayout.PropertyField(readyTime);
        EditorGUILayout.PropertyField(awayTime);
        EditorGUILayout.PropertyField(adsTime);
        EditorGUILayout.PropertyField(handsNeeded);

        /// adjustments

        EditorUi.DrawSubHeader("Adjustments");

        //EditorGUILayout.PropertyField(horizontalAngle);
        EditorGUILayout.PropertyField(verticalAngle);

        /// settings

        EditorUi.DrawSubHeader("Settings");

        EditorGUILayout.PropertyField(spawnOnlyOnMouseDown);

        if(script.baseStats.projectileSpawnAmount > 1)
        {
            EditorGUILayout.PropertyField(countSalveAsOneShot);
        }


        // Section 2 - Magazine

        EditorUi.DrawHeader("Magazine", 2);
        DrawProjectileAssetSectionSelection(script);
        DrawProjectileNameSelection(script, script.magazine.currProjctileSectionName);
        EditorGUILayout.PropertyField(magazineSize);
        EditorGUILayout.PropertyField(reloadTime);
        EditorGUILayout.PropertyField(allowPartialReload);
        EditorGUILayout.PropertyField(reloadSound);


        // Section 3 - Effects

        EditorUi.DrawHeader("Stat Effects", 3);

        EditorGUILayout.PropertyField(weaponEffectAppliers);
        EditorGUILayout.PropertyField(projectileEffectAppliers);


        // Section 4 - Settings

        EditorUi.DrawHeader("Extra Settings", 4);


        /// 4.1 - Spawn Points

        EditorUi.DrawToggle(ref script.useCustomSpawnPoints, "Enable Custom Spawn Points");

        if (script.useCustomSpawnPoints)
        {
            EditorGUILayout.PropertyField(spawnPosData);
            EditorGUILayout.PropertyField(useCameraAsSpawnPoint);
            EditorGUILayout.PropertyField(useStrictCameraForward);
        }


        /// 4.2 - Ammo Consumntion

        EditorUi.DrawToggle(ref script.enableCustomAmmoConsumption, "Enable Ammo Consumption");

        if (script.enableCustomAmmoConsumption)
        {
            EditorGUILayout.PropertyField(consumeAmmoOnMouseHold);
        }


        /// 4.3 - Fast Fire Buildup

        EditorUi.DrawToggle(ref script.enableFastFireBuildup, "Enable Fast Fire Buildup");

        if (script.enableFastFireBuildup)
        {
            EditorGUILayout.PropertyField(minCooldown);
            EditorGUILayout.PropertyField(cooldownDecreaseOnFire);
            EditorGUILayout.PropertyField(timerUntilReset);
        }


        /// 4.4 - Overheating

        EditorUi.DrawToggle(ref script.enableOverheating, "Enable Overheating");

        if (script.enableOverheating)
        {
            EditorGUILayout.PropertyField(heatIncreaseOnFire);
            EditorGUILayout.PropertyField(timeUntilHeatDecrease);
            EditorGUILayout.PropertyField(heatDecreaseSpeed);
            EditorGUILayout.PropertyField(heatEffectOnCooldown);
        }

        serializedObject.ApplyModifiedProperties();

        #endregion

        #region Sound and Vfx

        // Section 5 - Sound and Vfx

        EditorUi.DrawHeader("Sound and Vfx", 5);


        /// 5.1 - Weapon Sounds

        EditorUi.DrawToggle(ref script.enableWeaponSound, "Enable Sound");

        if (script.enableWeaponSound)
        {
            EditorGUILayout.PropertyField(weaponSounds);
        }


        /// 5.2 - Camera Effects

        EditorUi.DrawToggle(ref script.enableCameraEffects, "Enable Camera Effects");

        if (script.enableCameraEffects)
        {
            EditorGUILayout.PropertyField(shakePreset);
        }


        /// 5.3 - Muzzle Effects

        EditorUi.DrawToggle(ref script.enableMuzzleEffects, "Enable Muzzle Effects");

        if (script.enableMuzzleEffects)
        {
            DrawMuzzleEffectVariations(script);
        }

        /// 5.4 - Post Processing Effects

        EditorUi.DrawToggle(ref script.enablePostProcessingEffects, "Enable Post Processing Effects");

        if (script.enablePostProcessingEffects)
        {
            EditorGUILayout.PropertyField(postProcessingEffects);
        }

        #endregion

        _lastMuzzleEffectType = script.muzzleEffect.muzzleEffectType;
        lastProjectileSectionName = script.magazine.currProjctileSectionName;

        serializedObject.ApplyModifiedProperties();

        if (EditorApplication.isPlaying)
            Repaint();
    }

    private void DrawProjectileAssetSectionSelection(RangedWeapon script)
    {
        if (projectileSectionNames != null)
        {
            EditorGUI.BeginChangeCheck();

            script.magazine.selection_ProjectileSection = EditorGUILayout.Popup("Projectile Category", script.magazine.selection_ProjectileSection, projectileSectionNames.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                script.magazine.currProjctileSectionName = projectileSectionNames[script.magazine.selection_ProjectileSection];
            }
        }
    }

    private void DrawProjectileNameSelection(RangedWeapon script, string projectileAssetSectionName)
    {
        if(script.magazine.currProjctileSectionName != lastProjectileSectionName)
        {
            projectileNames = weaponAssets.LoadProjectileNames(script.magazine.currProjctileSectionName);
        }

        if (projectileNames != null)
        {
            EditorGUI.BeginChangeCheck();

            script.magazine.selection_ProjectileName = EditorGUILayout.Popup("Projectile", script.magazine.selection_ProjectileName, projectileNames.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                 script.magazine.projectileName = projectileNames[script.magazine.selection_ProjectileName];
            }
        }
    }

    private void DrawMuzzleEffectVariations(RangedWeapon script)
    {
        EditorGUILayout.PropertyField(muzzleEffectType);

        /// load variations if needed
        if (_lastMuzzleEffectType != script.muzzleEffect.muzzleEffectType)
        {
            _muzzleEffectVariations = LoadEffectVariationSelection(script.muzzleEffect.muzzleEffectType);
            if(_muzzleEffectVariations!= null)
                if (_muzzleEffectVariations.Count > 0)
                    script.muzzleEffect.effectVariation = _muzzleEffectVariations[0];
        }

        /// dropdown menu
        if (_muzzleEffectVariations != null)
        {
            EditorGUI.BeginChangeCheck();

            script.selcted_muzzleEffect = EditorGUILayout.Popup("Effect Variations", script.selcted_muzzleEffect, _muzzleEffectVariations.ToArray());

            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log("Dropdown selection: " + _muzzleEffectVariations[script.selcted_muzzleEffect]);
                script.muzzleEffect.effectVariation = _muzzleEffectVariations[script.selcted_muzzleEffect];
            }

            EditorGUILayout.PropertyField(muzzleEffectSize);
        }
    }

    private List<string> LoadEffectVariationSelection(GameAssets.MuzzleEffect muzzleEffectType)
    {
        GameAssets gameAssets = Resources.Load<GameObject>("GameAssets").GetComponent<GameAssets>();

        List<string> variations = new List<string>();

        MuzzleEffectPref muzzleEffectPref = null;

        // find muzzleEffectPref
        for (int i = 0; i < gameAssets.muzzleEffectPrefs.Count; i++)
        {
            if (gameAssets.muzzleEffectPrefs[i].muzzleEffectType == muzzleEffectType)
                muzzleEffectPref = gameAssets.muzzleEffectPrefs[i];
        }

        if (muzzleEffectPref == null) return null;

        // find all variations
        for (int i = 0; i < muzzleEffectPref.effectObjects.Count; i++)
        {
            variations.Add(muzzleEffectPref.effectObjects[i].effectVariation);
        }

        Debug.Log("loaded all muzzle variations");

        return variations;
    }
}
