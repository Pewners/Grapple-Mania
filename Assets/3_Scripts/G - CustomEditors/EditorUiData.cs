using Dave;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "RcLab/System/Hardcoded/EditorUiData")]
public class EditorUiData : ScriptableObject
{
    public Texture texture_headerBarExtension;
    public Texture texture_bgBar;
    public Texture texture_white;

    public enum Component
    {
        // monobehaviours
        ActionPlayer,
        AnimationPlayer,
        Combat,
        EffectPlayer,
        Inventory,
        PlayerAbilities,
        PlayerInput,
        PlayerReferences,
        Stats,

        // ability, action, effect
        Ability,
        RangedAction, MeleeAction,
        MovementAction,

        CinemachineImpulseShake, BasicShake, MoveCamShake, RotationShake, ZoomShake,

        Vignette, MotionBlur, LensDistortion, Bloom, ChromaticAberration,
        ColorAdjustments, DepthOfField, FilmGrain, WhiteBalance,

        StatEffect,

        // weapon and projectile
        RangedWeapon,

        Projectile,
        AssistedBouncing, AutoAim, Boomerang, Connector, Controllable, Explosive,
        ForceEngine, Homing, Laser, Lingering, PhysicsMaterialCreator, Vanishing,
        ProjectileSpawnerConnection
    }

    //public Dictionary<Component, ComponentDesignData> componentDesignLookup = new Dictionary<Component, ComponentDesignData>();
    public CustomDictionary<Component, ComponentDesignData> cd_componentDesingLookup = new CustomDictionary<Component, ComponentDesignData>();

    //[Button]
    public void Hehe()
    {
        //foreach (KeyValuePair<Component, ComponentDesignData> entry in componentDesignLookup)
        //{
         //   cd_componentDesingLookup.Add(entry.Key, entry.Value);
        //}
    }
}