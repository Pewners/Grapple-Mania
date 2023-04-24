
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// Inventory - RangedCombatLab
/// 
/// Content:
/// - storing ranged abilities of player
/// - handle player ammo
/// - equipping and unequipping weapons
/// - select weapons based on player input
/// - exectue weapon abilities
/// 
/// Note:
/// This script needs to be attatched to the player object

namespace RcLab
{
    // one reason to change:
    // yes haha :D
    public class Inventory : MonoBehaviour
    {
        public InventoryType inventoryType;
        public enum InventoryType
        {
            Player,
            Enemy
        }


        /* Weapons */
        public int weaponHotbarSlots = 9; // the amount of weapons the player can carry

        public List<WeaponHotbarItem> weaponHotbar = new List<WeaponHotbarItem>(); // all of the weapons the player currently carries
        public int startWeaponIndex;
        [HideInInspector] public bool disableFire;


        /// Abilities
        // 0 - weapon1 - Mouse0
        // 1 - weapon2 (only if dual wielding) - Mouse1
        // 2 - movement1 - LeftShift
        // 3 - movement2 - LeftControl
        // 4 - primaryAbility - Q
        // 5 - secondaryAbility - E
        // 6 - tertiaryAbiltiy - F (not always needed)
        // 7 - specialAbility - X
        public List<Ability> equippedAbilities;
        public WieldingState wieldingState;
        public enum WieldingState
        {
            None,
            SingleHandWeapon,
            BothHandWeapon,
            DualWielding
        }

        // assign abilities on start (for now)
        [SerializeField] private AbilitySet characterAbilitySet;

        // for now
        public GameObject ultimateBlaster;
        public GameObject grappleGun;

        // reloading
        /// partial reloading
        private float timeForOneAmmoReload;
        private float timeTillNextAmmoReload;
        [SerializeField] private Dictionary<int, Coroutine> reloadCoroutineLookup = new Dictionary<int, Coroutine>();


        /* Items */
        public bool infiniteAmmo = false;

        // interaction
        public float interactionDistance;
        public float interactionSpherecastRadius;
        public LayerMask whatIsInteractable;
        
        // references
        private Transform camT;
        private Combat combat;
        private WeaponHolder weaponHolder;
        private PlayerAbilities abilities;

        // events
        public UnityAction OnReload;
        public UnityAction OnReloadStop;
        public UnityAction OnReloadFinishedWithAnimation;
        public UnityAction OnWeaponSwitch;
        public UnityAction<RangedWeapon> OnWeaponEquipped;
        public UnityAction OnWeaponEquipAnimationFinished;
        public UnityAction OnWeaponFireStop;
        //public Action OnWeightCategoryChange; idk

        public HandSlots handSlots;

        private bool complexWeaponAbilityEquipped;

        #region Start and Update

        private void Start()
        {
            if (inventoryType == InventoryType.Player) camT = GameObject.Find("CameraHolder").transform;

            combat = PlayerReferences.instance.combat;
            weaponHolder = PlayerReferences.instance.weaponHolder;
            abilities = PlayerReferences.instance.abilities;

            ListSetup();

            StartEquipWeapon(0);
        }

        private void ListSetup()
        {
            // main
            equippedAbilities = new List<Ability>()
            {
                null,                                         // 0
                null,                                         // 1
                characterAbilitySet.primaryMovementAbility,   // 2
                characterAbilitySet.secondaryMovementAbility, // 3
                characterAbilitySet.primaryCombatAbility,     // 4
                characterAbilitySet.secondaryCombatAbility,   // 5
                characterAbilitySet.tertiaryCombatAbility,    // 6
                characterAbilitySet.specialAbility,           // 7
            };

            List<WeaponHotbarItem> startHotbar = weaponHotbar;

            weaponHotbar = new List<WeaponHotbarItem>(weaponHotbarSlots);
            for (int i = 0; i < weaponHotbarSlots; i++)
            {
                weaponHotbar.Add(null);
            }

            for (int i = 0; i < startHotbar.Count; i++)
            {
                weaponHotbar[i] = startHotbar[i];
            }
        }

        private void Update()
        {
            if (inventoryType == InventoryType.Player) CheckForInteractions();
        }

        #endregion

        #region CheckAbilities

        public void TryPlayAbility(int index)
        {
            if (index + 1 > equippedAbilities.Count)
                return;

            TryPlayAbility(equippedAbilities[index]);
        }
        public void TryPlayAbility(Ability ability)
        {
            if(ability == null)
            {
                print("Can't play ability because ability slot is empty");
                return;
            }

            print("Inventory: Trying to play ability " + ability.abilityName);

            HandsNeeded handsNeededForAbility = ability.GetHandsNeeded();

            if (!handSlots.Check(handsNeededForAbility, ability.isWeapon))
            {
                // compare priority of new ability vs priority of current ability (or 0 if hand is carrying a weapon)
                /// in progress...
                print("TryPlayAbility: HandSlots Check failed");
                return;
            }

            if (IsAnyWeaponReloading())
            {
                // interrupt reload but don't fire yet
                if (ability.isWeapon)
                    StopReload(ability.GetMag());

                print("TryPlayAbility: Reloading Check failed");
                return;
            }

            if (ability.isWeapon && currEquipWeaponCoroutine != null)
            {
                print("TryPlayAbility: Equip Check failed");
                return;
            }

            abilities.PlayAbility(ability);

            // set hand usage (if it's a weapon no need to change anything)
            if (!ability.isWeapon)
                handSlots.SetUsage(handsNeededForAbility, HandUsage.Ability);
        }

        private void UpdateWieldingState()
        {
            // dual wielding
            if (equippedAbilities[0] != null && equippedAbilities[1] != null)
                wieldingState = WieldingState.DualWielding;

            // could be one bothHandWeapon or one singleHandWeapon
            else if (equippedAbilities[0] != null && equippedAbilities[1] == null)
            {
                if (handSlots.CheckUsage(HandType.Left, HandUsage.Weapon) && handSlots.CheckUsage(HandType.Left, HandUsage.Weapon))
                    wieldingState = WieldingState.BothHandWeapon;
                else
                    wieldingState = WieldingState.SingleHandWeapon;
            }

            else
                wieldingState = WieldingState.None;
        }

        #endregion

        #region Equip and Unequip Weapons

        public void StartEquipWeapon(int weaponHotbarIndex)
        {
            print("equipping weapon with hotbar index " + weaponHotbarIndex);

            if (weaponHotbar[weaponHotbarIndex] == null) return;
            if (GetCurrentWeapon() == weaponHotbar[weaponHotbarIndex].ability.GetFirstRangedWeapon()) return;

            if (currEquipWeaponCoroutine != null)
                StopCoroutine(currEquipWeaponCoroutine);

            currEquipWeaponCoroutine = StartCoroutine(EquipWeapon(weaponHotbarIndex));
        }

        Coroutine currEquipWeaponCoroutine;
        private IEnumerator EquipWeapon(int weaponHotbarIndex)
        {
            Ability weaponAbility = weaponHotbar[weaponHotbarIndex].ability;
            List<RangedWeapon> weapons = weaponAbility.GetWeapons();
            
            HandsNeeded handsNeeded = weaponAbility.GetHandsNeeded();

            print("1234: " + weaponAbility.abilityName + " " + handsNeeded.ToString());

            int safety = 0;
            while (!handSlots.Check(handsNeeded, false))
            {
                yield return StartCoroutine(UnequipWeapon());
                safety++;
                if (safety == 3)
                    continue;
            }

            if (handsNeeded == HandsNeeded.Both)
                EquipBothHandWeapon(weaponAbility);
            else
                EquipOneHandWeapon(weaponAbility);

            for (int i = 0; i < weapons.Count; i++)
                weapons[i].ApplyAllEffects();

            weaponHolder.SwitchBlaster(weaponAbility.blasterType);

            EquipWeaponEvents(weapons[0]);

            yield return new WaitForSeconds(weaponHolder.StartTransition(WeaponHolder.WeaponPos.Normal));

            currEquipWeaponCoroutine = null;
            complexWeaponAbilityEquipped = weaponAbility.weaponType == Ability.WeaponType.complexWeapon;

            if (OnWeaponEquipAnimationFinished != null)
                OnWeaponEquipAnimationFinished();
        }

        private void EquipOneHandWeapon(Ability weaponAbility)
        {
            int freeHand = handSlots.Find(HandUsage.Free, HandType.Right);
            equippedAbilities[freeHand] = weaponAbility;

            handSlots.SetUsage(freeHand, HandUsage.Weapon);

            if (handSlots.CheckUsage(HandType.Left, HandUsage.Weapon) && handSlots.CheckUsage(HandType.Right, HandUsage.Weapon))
                wieldingState = WieldingState.DualWielding;
            else
                wieldingState = WieldingState.SingleHandWeapon;
        }

        private void EquipBothHandWeapon(Ability weaponAbility)
        {
            equippedAbilities[0] = weaponAbility;

            handSlots.SetUsage(HandType.Left, HandUsage.Weapon);
            handSlots.SetUsage(HandType.Right, HandUsage.Weapon);

            wieldingState = WieldingState.BothHandWeapon;
        }

        private void EquipWeaponEvents(RangedWeapon weapon)
        {
            if (OnWeaponSwitch != null) 
                OnWeaponSwitch();
            if (OnWeaponEquipped != null) 
                OnWeaponEquipped(weapon);

            if (inventoryType == InventoryType.Player)
            {
                SoundManager.PlayEffect(weapon.weaponSounds.equipSound);
                FindObjectOfType<DynamicCrosshair>().UpdateSize(weapon.stabilityNormal.spread);
            }
        }

        public IEnumerator UnequipWeapon()
        {
            if (wieldingState == WieldingState.None)
            {
                Debug.LogError("Can't unequip weapon since no weapon is equipped");
                yield break;
            }

            print("unequipping current weapon...");

            int weaponToUnequipIndex = -1;

            if(wieldingState == WieldingState.BothHandWeapon)
            {
                weaponToUnequipIndex = 0;
                handSlots.SetUsage(HandType.Left, HandUsage.Free);
                handSlots.SetUsage(HandType.Right, HandUsage.Free);
                wieldingState = WieldingState.None;
            }

            else if (wieldingState == WieldingState.SingleHandWeapon)
            {
                int weaponHand = handSlots.Find(HandUsage.Weapon, HandType.Left);
                weaponToUnequipIndex = weaponHand;
                handSlots.SetUsage(weaponHand, HandUsage.Free);
                wieldingState = WieldingState.None;
            }

            else if (wieldingState == WieldingState.DualWielding)
            {
                int preferredHand = handSlots.Find(HandUsage.Weapon, HandType.Left);
                weaponToUnequipIndex = preferredHand;
                handSlots.SetUsage(preferredHand, HandUsage.Free);
                wieldingState = WieldingState.SingleHandWeapon;
            }

            TryStopReload(equippedAbilities[weaponToUnequipIndex].GetMag());
            abilities.StopAbility(equippedAbilities[weaponToUnequipIndex].GetInstanceID());
            equippedAbilities[weaponToUnequipIndex] = null;

            float transitionDuration = weaponHolder.StartTransition(WeaponHolder.WeaponPos.Away);
            yield return new WaitForSeconds(transitionDuration);

            print("unequipped weapon with index " + weaponToUnequipIndex);
        }

        public void LoadWeaponObject(int weaponIndex)
        {
            // weaponCreator_ultimateBlaster.CreateWeaponObject(weapon);
        }

        #endregion

        #region Weapon Handling

        // stops all equipped weapons from firing
        public void StopAllWeaponFire()
        {
            if (wieldingState == WieldingState.BothHandWeapon)
                abilities.StopAbility(equippedAbilities[0].GetInstanceID());

            else if (wieldingState == WieldingState.SingleHandWeapon)
            {
                int weaponHand = handSlots.Find(HandUsage.Weapon, HandType.Right);
                abilities.StopAbility(equippedAbilities[weaponHand].GetInstanceID());
            }

            else if (wieldingState == WieldingState.DualWielding)
            {
                abilities.StopAbility(equippedAbilities[0].GetInstanceID());
                abilities.StopAbility(equippedAbilities[1].GetInstanceID());
            }
        }

        // 0 means left weapon, 1 means right weapon
        public void StopWeaponFire(int weaponIndex)
        {
            if (equippedAbilities.Any())
                abilities.StopAbility(equippedAbilities[weaponIndex].GetInstanceID());

            if (OnWeaponFireStop != null)
                OnWeaponFireStop();
        }

        #endregion

        #region Magazine Player

        // generally called when pressing R
        public void ReloadAllWeapons()
        {
            if (wieldingState == WieldingState.BothHandWeapon)
                StartReload(0);
        
            else if (wieldingState == WieldingState.SingleHandWeapon)
            {
                int weaponHand = handSlots.Find(HandUsage.Weapon, HandType.Right);
                StartReload(weaponHand);
            }

            else if (wieldingState == WieldingState.DualWielding)
            {
                StartReload(0);
                StartReload(1);
            }
        }

        public void StartReload(RangedWeapon weapon)
        {
            for (int i = 0; i < equippedAbilities.Count; i++)
            {
                if (equippedAbilities[i] == null) 
                    continue;

                if (equippedAbilities[i].isWeapon == true)
                {
                    if (equippedAbilities[i].GetFirstRangedWeapon() == weapon)
                        StartReload(equippedAbilities[i].GetFirstRangedWeapon().magazine);
                }
            }
        }
        public void StartReload(int weaponIndex)
        {
            print("reloading weapon with weaponIndex " + weaponIndex);

            Magazine mag = equippedAbilities[weaponIndex].GetMag();

            StartReload(mag);
        }
        public void StartReload(Magazine mag)
        {
            if (inventoryType == InventoryType.Enemy) return;

            if (mag.reloading) return;

            if (mag.ammoLeft >= mag.magazineSize)
            {
                UiManager.instance.ShowPopup("magazine already full", "", UiManager.PopupType.FloatingText);
                return;
            }

            combat.DeactivateAds(false); // no weapon movement, just deactivate the ads bool

            mag.reloading = true;

            float durationTillReloadPos = weaponHolder.StartTransition(WeaponHolder.WeaponPos.Reload);

            print("reload duration: " + durationTillReloadPos + " " + mag.reloadTime);

            int magId = mag.GetHashCode();

            Coroutine coroutine = null;

            if (mag.allowPartialReload == true)
                coroutine = StartCoroutine(PartiallyReloadMagazine(mag));

            else
                coroutine = StartCoroutine(FullMagazineReloadDelayed(mag, durationTillReloadPos + mag.reloadTime));

            reloadCoroutineLookup.Add(magId, coroutine);
        }

        private IEnumerator PartiallyReloadMagazine(Magazine mag)
        {
            float timeForOneAmmoReload = mag.reloadTime / mag.magazineSize;

            int safety = 0;
            while (mag.ammoLeft < mag.magazineSize)
            {
                yield return new WaitForSeconds(timeForOneAmmoReload);

                AddAmmoToMagazine(mag, 1, SoundEnabled.False);

                safety++;
                if (safety > 1000)
                    yield return null;
            }
        }

        private IEnumerator FullMagazineReloadDelayed(Magazine mag, float delay)
        {
            yield return new WaitForSeconds(delay);

            int magazineAmountMissing = mag.magazineSize - mag.ammoLeft;
            AddAmmoToMagazine(mag, magazineAmountMissing, SoundEnabled.True);
        }

        private void AddAmmoToMagazine(Magazine mag, int reloadAmount, SoundEnabled soundEnabled)
        {
            mag.ammoLeft += reloadAmount;

            if(soundEnabled == SoundEnabled.True)
                SoundManager.PlayEffect(mag.reloadSound);

            if(OnReload != null) OnReload();

            if (mag.ammoLeft == mag.magazineSize)
                StopReload(mag);

            print("reloaded " + reloadAmount + " ammo");
        }

        public void SubtractAmmoFromMagazine(Magazine mag, int subtractionAmount = 1)
        {
            if (mag.ammoLeft <= 0)
            {
                StopWeaponFire(0);
                return;
            }

            mag.ammoLeft--;

            print("subtract ammo 3 " + mag.ammoLeft);
        }

        private void TryStopReload(Magazine mag)
        {
            if(IsReloading(mag))
                StopReload(mag);
        }

        private void StopReload(Magazine mag)
        {
            mag.reloading = false;
            mag.partialReloading = false;

            int magId = mag.GetHashCode();
            StopCoroutine(reloadCoroutineLookup[magId]);
            reloadCoroutineLookup.Remove(magId);

            if(mag.IsFull())
                StartCoroutine(OnReloadFinishedWithAnimationDelayed(weaponHolder.GetTransitionTime(WeaponHolder.WeaponPos.Normal)));

            if (OnReloadStop != null)
                OnReloadStop();
        }

        // magazine full and animation finished
        private IEnumerator OnReloadFinishedWithAnimationDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            if(OnReloadFinishedWithAnimation != null)
                OnReloadFinishedWithAnimation();
        }

        public bool IsAnyWeaponReloading()
        {
            bool weaponAbility0Reloading = equippedAbilities[0] == null ? false : IsReloading(equippedAbilities[0].GetMag());
            bool weaponAbility1Reloading = equippedAbilities[1] == null ? false : IsReloading(equippedAbilities[1].GetMag());
            return weaponAbility0Reloading || weaponAbility1Reloading;
        }

        public bool IsMainWeaponReloading()
        {
            return equippedAbilities[0] == null ? false : IsReloading(equippedAbilities[0].GetMag());
        }

        public bool IsReloading(Magazine mag)
        {
            return mag.reloading || mag.partialReloading;
        }

        #endregion

        #region Debugging

        public TextMeshProUGUI text_weaponState;
        public void DebugWeaponState()
        {
            string debugText = "";
            debugText += wieldingState + "\n";
            debugText += "Left: " + handSlots.GetUsage(HandType.Left) + " " + "Right:" + handSlots.GetUsage(HandType.Right);
        }

        #endregion

        #region Interactions

        private IInteractable currInteractable;
        private Transform interactableT = null;
        public bool interactableActive;
        private bool lastInteractableActive;
        private void CheckForInteractions()
        {
            RaycastHit hit;
            if (Physics.SphereCast(camT.position, interactionSpherecastRadius, camT.forward, out hit, interactionDistance, whatIsInteractable))
            {
                // store interactable for later
                if (currInteractable == null && hit.transform.gameObject.TryGetComponent(out IInteractable interactable))
                {
                    currInteractable = interactable;
                    interactableT = hit.transform;
                }
            }
            else
            {
                currInteractable = null;
                interactableT = null;
            }

            interactableActive = interactableT != null;

            bool interactableActiveHasChanged = interactableActive != lastInteractableActive;

            if (interactableActiveHasChanged)
            {
                if (interactableActive) UiManager.instance.PlaceInteractionSign(interactableT);
                if (!interactableActive) UiManager.instance.RemoveInteractionSign();
            }

            lastInteractableActive = interactableActive;
        }

        public void Interact()
        {
            if (currInteractable != null)
                currInteractable.Interact();
        }

        #endregion

        #region Add and Subtract items etc.

        /// <summary>
        /// I would love to have 1 general AddItem and 1 general SubtractItem function sometime haha
        /// </summary>

        public void AddWeaponToHotbar(WeaponHotbarItem hotbarItem)
        {
            weaponHotbar.Add(hotbarItem);
        }

        #endregion   

        #region Getters

        public bool IsAnyWeaponEquipped()
        {
            return handSlots.CheckUsage(HandType.Left, HandUsage.Weapon) || handSlots.CheckUsage(HandType.Right, HandUsage.Weapon);
        }

        public bool IsComplexWeaponAbilityEquipped()
        {
            return complexWeaponAbilityEquipped;
        }

        public bool IsDualWielding()
        {
            return wieldingState == WieldingState.DualWielding;
        }

        public Ability GetAbility(int index)
        {
            if (index+1 > equippedAbilities.Count) 
                return null;
            return equippedAbilities[index];
        }

        public Ability GetCurrentWeaponAbility()
        {
            if (equippedAbilities.Count == 0)
                return null;

            if (equippedAbilities[0] == null)
                return null;

            return equippedAbilities[0];
        }

        public Magazine GetCurrentWeaponAbilityMagazine()
        {
            return GetCurrentWeaponAbility().GetMag();
        }

        public RangedWeapon GetCurrentWeapon()
        {
            if(wieldingState == WieldingState.DualWielding)
            {
                Debug.LogError("Dual wielding still needs to be implemented");
                return null;
            }

            if (equippedAbilities.Count == 0)
                return null;

            if (equippedAbilities[0] == null)
                return null;

            print("returning main weapon of ability 0" + equippedAbilities[0].GetFirstRangedWeapon().weaponName);
            return equippedAbilities[0].GetFirstRangedWeapon();
        }

        #endregion

        #region RcLabOnly

        public Ability modularMainAbility_RcLabOnly;
        private RangedWeapon originalWeapon;
        public void ChangeMainWeapon(RangedWeapon weapon)
        {
            originalWeapon = weapon;
            RangedWeapon clonedWeapon = Instantiate(weapon);

            print("changing main weapon to " + clonedWeapon.weaponName);

            modularMainAbility_RcLabOnly.isWeapon = true;
            modularMainAbility_RcLabOnly.name = "Modular Ability (Needed for WeaponSwitcher!) -> " + clonedWeapon.weaponName;
            modularMainAbility_RcLabOnly.abilityName = "Modular Ability -> " + clonedWeapon.weaponName;
            modularMainAbility_RcLabOnly.weaponType = Ability.WeaponType.simpleWeapon;
            modularMainAbility_RcLabOnly.SetWeapon(null);
            modularMainAbility_RcLabOnly.SetWeapon(clonedWeapon);
            modularMainAbility_RcLabOnly.blasterType = clonedWeapon.GetHandsNeeded() == HandsNeeded.One ? UltimateBlaster.BlasterType.Pistol : UltimateBlaster.BlasterType.Rifle;

            StartEquipWeapon(1);
            StartEquipWeapon(0);
        }

        private string newProjectileName;
        public void ChangeMainProjectile(string projectileName)
        {
            newProjectileName = projectileName;
            modularMainAbility_RcLabOnly.GetFirstRangedWeapon().magazine.projectileName = projectileName;
        }

        public void ApplyProjectileChanges()
        {
            originalWeapon.magazine.projectileName = newProjectileName;
        }

        #endregion

        private void OnDrawGizmos()
        {
            ///Gizmos.color = Color.red;
            ///Gizmos.DrawLine(camT.position, camT.position + camT.forward * interactionDistance);
        }

        public enum SoundEnabled
        {
            False,
            True
        }

        [Serializable]
        public class WeaponHotbarItem
        {
            public Ability ability;
            //public GameObject builtWeaponObj;
        }

        [Serializable]
        private class AbilitySet
        {
            public Ability primaryMovementAbility;
            public Ability secondaryMovementAbility;
            public Ability primaryCombatAbility;
            public Ability secondaryCombatAbility;
            public Ability tertiaryCombatAbility;
            public Ability specialAbility;
        }
    }
}
