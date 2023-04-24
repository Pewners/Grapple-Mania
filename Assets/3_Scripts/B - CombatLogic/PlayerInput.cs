using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.BuiltIn.ShaderGraph;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;

/// Player Input - RangedCombatLab
/// 
/// Content:
/// - Receive Inputs and call Inventory/Weapon functions
/// 
/// Note:
/// This script needs to be attatched to the Player
/// All Keybinds can be changed in the Unity Inspector

namespace RcLab
{
    // one reason to change:
    // if new functions need to be called
    public class PlayerInput : MonoBehaviour
    {
        // main inputs
        public Keybinds keybinds;

        // cam sensitivity
        public CamInputData camInputData;
        public float camSenseX;
        public float camSenseY;

        [Header("References")]
        private Inventory inv;
        private Combat combat;
        private PlayerAbilities playerAbilities;

        [Header("Inputs")]
        [HideInInspector] public float horizontalInput;
        [HideInInspector] public float verticalInput;
        [HideInInspector] public float mouseX;
        [HideInInspector] public float mouseY;
        [HideInInspector] public int spaceInput;
        [HideInInspector] public int shiftInput;

        [HideInInspector] public bool viewActive;

        public UnityAction OnPrimaryFireUp;

        private void Start()
        {
            playerAbilities = PlayerReferences.instance.abilities;
            inv = PlayerReferences.instance.inventory;
            combat = PlayerReferences.instance.combat;

            inv.OnReloadStop += ReevaluateAdsState;
            playerAbilities.OnWeaponCooldownFinished += ReevaluatePrimaryFireState;
            inv.OnWeaponEquipAnimationFinished += ReevaluatePrimaryFireState;
            inv.OnReloadFinishedWithAnimation += ReevaluatePrimaryFireState;
        }

        private void Update()
        {
            if (!viewActive)
            {
                AbilityInputs();
                NumberKeyInputs();
            }

            MenuInputs();

            // basic inputs
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            spaceInput = Input.GetKey(KeyCode.Space) ? 1 : 0;
            shiftInput = Input.GetKey(KeyCode.LeftShift) ? 1 : 0;
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            bool adsActive = combat.GetAdsActive();
            camSenseX = adsActive ? camInputData.baseCamSenseX * camInputData.adsCamSenseMultiplier : camInputData.baseCamSenseX;
            camSenseY = adsActive ? camInputData.baseCamSenseY * camInputData.adsCamSenseMultiplier : camInputData.baseCamSenseY;

            // separate inputs
            if (Input.GetKeyDown(keybinds.reload)) inv.ReloadAllWeapons();

            // events
            if (Input.GetKeyUp(keybinds.abilityKeys[0]) && OnPrimaryFireUp != null)
                OnPrimaryFireUp();
        }

        #region Inputs

        private void AbilityInputs()
        {
            // activate ability cycles of RangedCombat script if respective keys are pressed down
            for (int i = 0; i < keybinds.abilityKeys.Count; i++)
            {
                if (Input.GetKeyDown(keybinds.abilityKeys[i]))
                {
                    print("ability key input " + keybinds.abilityKeys[i]);

                    if (i == 1 && !inv.IsDualWielding())
                        combat.ActivateAds();
                    else
                        StartAbilityCall(i);
                }
            }

            // deactivate ability cycles of RangedCombat script if respective keys are released
            for (int i = 0; i < keybinds.abilityKeys.Count; i++)
            {
                if (Input.GetKeyUp(keybinds.abilityKeys[i])) 
                {
                    if (i == 1)
                        combat.DeactivateAds();
                    
                    StopAbilityCall(i);
                }
            }
        }

        private void NumberKeyInputs()
        {
            // switch weapons based on number keys pressed
            for (int i = 0; i < keybinds.numberKeys.Count; i++)
            {
                if (Input.GetKeyDown(keybinds.numberKeys[i]))
                    inv.StartEquipWeapon(i);
            }
        }

        #endregion

        #region AbilityCalls

        public void StartAbilityCall(int abilityIndex)
        {
            if (viewActive) return;

            print("Input: Ability index " + abilityIndex);
            inv.TryPlayAbility(abilityIndex);
        }

        public void StopAbilityCall(int abilityIndex)
        {
            Ability ability = inv.GetAbility(abilityIndex);

            if (ability == null)
                return;

            if (!ability.StopOnKeyUp())
                return;

            // stop ability
            playerAbilities.StopAbility(ability.GetInstanceID());
        }

        #endregion

        #region Ui Calls

        private void MenuInputs()
        {
            if (SceneManager.GetActiveScene().name == "MainScreen") return;

            if (Input.GetKeyDown(keybinds.interact) && inv.interactableActive)
            {
                print("Input: Interact");
                inv.Interact();
            }

            // tab for weapon switcher
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                print("tab: " + UiManager.instance.IsViewActive(UiView.Type.RcLabWeaponSwitcher));

                if (!UiManager.instance.IsViewActive(UiView.Type.RcLabWeaponSwitcher))
                {
                    UiManager.instance.HideActiveView();
                    UiManager.instance.ShowView(UiView.Type.RcLabWeaponSwitcher);
                }
                else
                    UiManager.instance.HideActiveView();
            }
        }

        #endregion

        #region Handling Events

        private void ReevaluateAdsState()
        {
            if (Input.GetKey(keybinds.abilityKeys[1]))
                combat.ActivateAds();
            else
                combat.DeactivateAds();
        }

        private void ReevaluatePrimaryFireState()
        {
            if (Input.GetKey(keybinds.abilityKeys[0]) && !inv.IsAnyWeaponReloading())
                StartAbilityCall(0);
        }

        #endregion
    }
}

[Serializable]
public class CamInputData
{
    public float baseCamSenseX = 10f;
    public float baseCamSenseY = 10f;
    public float adsCamSenseMultiplier = 0.85f;
}
