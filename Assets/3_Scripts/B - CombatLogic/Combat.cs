using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Dave;
using UnityEditorInternal;
using UnityEngine.ProBuilder;

/// PlayerRangedCombat - RangedCombatLab
/// 
/// Content:
/// - execute abilities of weapons
/// -> handles ability cycles and spawns projectiles
/// 
/// Note:
/// This script needs to be assigned to the player

namespace RcLab
{
    // Object - WeaponPlayer
    //
    // one reason to change:
    // if the RangedAction changes
    // 
    // further stuff :)
    // - ads handling
    
    public class Combat : MonoBehaviour
    {
        public CombatType combatType;
        public enum CombatType
        {
            Player,
            Enemy
        }

        public bool locked;

        // references
        public Transform cam;
        public CamRecoil_MLab camRecoil;
        private Rigidbody rb;
        public WeaponHolder weaponHolder;
        private RangedWeapon currWeapon;

        // retrieved from the weaponHolder
        [HideInInspector] public Transform attackPoint; // the point where the projectiles get spawned (unless customSpawnPoint addon is used)
        private UltimateBlaster ultimateBlaster;

        // raycast damage
        public float raycastInitialAccelThreshold;
        public LayerMask whatIsTarget;

        public Vector3 lastAttackViewPoint;

        private Dictionary<int, Coroutine> coroutineLookup = new Dictionary<int, Coroutine>();

        public CycleState state; // not directly needed, but could be used to play sounds, particle effects etc.
        public enum CycleState
        {
            none,
            charging,
            executing,
            cooldown
        }

        // events
        public UnityAction OnProjectileSpawn;
        public UnityAction OnWeaponStoppedRunning;

        private Inventory inv;
        private PlayerMovement_MLab movement;
        private PlayerInput input;
        private PlayerAbilities playerAbilities;

        private bool primaryFirePressed;

        // spread

        /// moving
        public float moveSpreadMultiplier;
        public float moveSpreadMultiplierChangeSpeed;
        public float rbSpeedThreshold;

        /// spread components
        private float _adsSpread; // changes in sync with adsAnimation (adsTime)
        private float targetMoveSpreadMultiplier;
        private float _moveSpreadMultiplier; // changes with spreadChangeSpeed

        /// final values
        private float currMinSpread;
        private float currSpread;

        // ads
        private bool adsActive = false;

        // consume ammo
        private float timeToNextConsumtion;

        // fast fire buildup
        private float timerTillFastFireReset;

        // heat buildup
        private bool decreasingHeat;
        private float timerTillHeatDecrease;

        public bool debuggingEnabled = false;
        public TextMeshProUGUI text_state;

        private List<int> runningWeaponIds;

        private void Start()
        {
            if (cam == null) cam = PlayerReferences.instance.cam.transform;

            inv = PlayerReferences.instance.inventory;
            rb = PlayerReferences.instance.rb;
            input = PlayerReferences.instance.input;
            movement = PlayerReferences.instance.movementOld;
            playerAbilities = PlayerReferences.instance.abilities;

            runningWeaponIds = new List<int>();

            _moveSpreadMultiplier = moveSpreadMultiplier;

            inv.OnWeaponEquipped += SetupWeapon;
            playerAbilities.OnComplexWeaponSwitch += UpdateCurrWeapon;
        }

        private void Update()
        {
            // debugging
            if(debuggingEnabled) 
                TextDebug();

            if (locked)
                return;

            primaryFirePressed = Input.GetKey(input.keybinds.abilityKeys[0]);

            UpdateSpread();
            UpdateAmmoConsumptionOnMouseHold();
            UpdateFastFireBuildup();
            UpdateOverheating();
        }

        #region Updates

        private void UpdateSpread()
        {
            if (currWeapon != null)
            {
                // get spread components
                _adsSpread = Mathf.Lerp(currWeapon.stabilityNormal.spread, currWeapon.stabilityWithAds.spread, weaponHolder.GetAdsProgress());
                targetMoveSpreadMultiplier = rb.velocity.magnitude > rbSpeedThreshold ? moveSpreadMultiplier : 1;
                _moveSpreadMultiplier = Mathf.Lerp(_moveSpreadMultiplier, targetMoveSpreadMultiplier, Time.deltaTime * moveSpreadMultiplierChangeSpeed);

                // calculate final values
                currSpread = _adsSpread * _moveSpreadMultiplier;
                currMinSpread = Mathf.Lerp(currWeapon.stabilityNormal.minimalSpread, currWeapon.stabilityWithAds.minimalSpread, weaponHolder.GetAdsProgress());

                print("spreadDebug: " + "adsSpread: " + _adsSpread + " moveSpread: " + _moveSpreadMultiplier);

                // update crosshair
                FindObjectOfType<DynamicCrosshair>().UpdateSize(currSpread);
            }
        }

        private void UpdateAmmoConsumptionOnMouseHold()
        {
            if (primaryFirePressed && currWeapon != null)
            {
                if (currWeapon.consumeAmmoOnMouseHold)
                {
                    timeToNextConsumtion -= Time.deltaTime;

                    if (timeToNextConsumtion <= 0)
                    {
                        timeToNextConsumtion = currWeapon.cooldown;
                        ultimateBlaster.Fire(20, currWeapon.cooldown, currWeapon.heat);
                        inv.SubtractAmmoFromMagazine(currWeapon.magazine);
                    }
                }
            }
        }

        private void UpdateFastFireBuildup()
        {
            if (timerTillFastFireReset > 0)
            {
                timerTillFastFireReset -= Time.deltaTime;

                if (timerTillFastFireReset < 0)
                {
                    currWeapon.FastFireBuildupReset();
                    timerTillFastFireReset = 0;
                }
            }
        }

        private void UpdateOverheating()
        {
            if (timerTillHeatDecrease > 0)
                timerTillHeatDecrease -= Time.deltaTime;

            if (timerTillHeatDecrease < 0)
            {
                if (currWeapon.HeatDecreaseStep(Time.deltaTime * currWeapon.heatDecreaseSpeed))
                    timerTillHeatDecrease = 0;

                ultimateBlaster.UpdateHeatColor(currWeapon.heat);
            }
        }

        #endregion

        #region Weapon Setup

        public void SetupWeapon(RangedWeapon weapon)
        {
            attackPoint = weaponHolder.GetCurrAttackPointTransform();
            ultimateBlaster = weaponHolder.GetCurrBlaster();
            UpdateCurrWeapon(weapon);

            // just bug fixing
            if (weapon.magazine.ammoLeft > weapon.magazine.magazineSize)
                weapon.magazine.ammoLeft = weapon.magazine.magazineSize;
        }

        public void UpdateCurrWeapon(RangedWeapon weapon)
        {
            currWeapon = weapon;
            print("updated weapon to " + weapon.weaponName + ", spread: " + weapon.stabilityNormal.spread);
        }

        #endregion

        #region Ads

        public void ActivateAds()
        {
            if (!inv.IsAnyWeaponEquipped()) return;

            // no ads available during dual wielding
            if (inv.IsDualWielding()) return;
            if (inv.IsMainWeaponReloading()) return;

            float delay = weaponHolder.StartTransition(WeaponHolder.WeaponPos.Ads);
            Invoke(nameof(SetAdsToTrue), delay);

            PlayerReferences.instance.cam.DoFovZoomPercentage(currWeapon.baseStats.adsFovZoomPercent, delay);
        }

        public void DeactivateAds(bool moveGun = true)
        {
            if (inv.IsMainWeaponReloading()) return;

            adsActive = false;

            float delay = -1;

            if (moveGun)
                delay = weaponHolder.StartTransition(WeaponHolder.WeaponPos.Normal);

            PlayerReferences.instance.cam.ResetFov(delay);
        }

        private void SetAdsToTrue() { adsActive = true; }
        private void SetAdsToFalse() { adsActive = false; }

        #endregion

        #region Weapon Player

        public void PlayRangedWeapon(RangedWeapon weapon)
        {
            print("PlayerCombat: Now Playing weapon " + weapon.weaponName);
            StartCoroutine(RangedWeaponCycle(weapon));
            runningWeaponIds.Add(weapon.GetInstanceID());
            /// coroutineLookup.TryAdd(weapon.GetInstanceID(), coroutine);
        }

        // should I not just use the ability system and that's it?
        //public void StopRangedWeapon(RangedWeapon weapon)
        //{
            ///int weaponInstanceId = weapon.GetInstanceID();
            ///if(coroutineLookup.ContainsKey(weaponInstanceId))
            ///    StopCoroutine(coroutineLookup[weaponInstanceId]);
        //}

        private IEnumerator RangedWeaponCycle(RangedWeapon weapon)
        {
            print("WeaponAmmo: " + weapon.magazine.ammoLeft + " " + inv.IsComplexWeaponAbilityEquipped());
            if (weapon.magazine.ammoLeft <= 0 && !inv.IsComplexWeaponAbilityEquipped())
            {
                print("WeaponAmmo: Stopped!");
                inv.StopWeaponFire(0);
                inv.StartReload(weapon);
                yield break;
            }

            state = CycleState.charging;
            yield return new WaitForSecondsRealtime(weapon.chargeTime);

            state = CycleState.executing;
            yield return StartCoroutine(ExecutionCycle(weapon));

            state = CycleState.cooldown;
            yield return new WaitForSecondsRealtime(weapon.cooldown);

            RemoveRunningWeaponId(weapon.GetInstanceID());
            StartCoroutine(CallEventsIfWeaponIsNotRunning(weapon.GetInstanceID(), 0.05f));
        }
        
        // can not be stopped (smallest combat unit)
        private IEnumerator ExecutionCycle(RangedWeapon weapon)
        {
            int objectsToSpawn = weapon.ammoFireAmount;
            int objectsSpawned = 0;

            if(!inv.IsComplexWeaponAbilityEquipped())
                inv.SubtractAmmoFromMagazine(weapon.magazine);

            while (objectsSpawned < objectsToSpawn)
            {
                SpawnProjectile(weapon, weapon.GetCurrIndexOfSalve());
                objectsSpawned++;
                weapon.IncreaseCurrIndexOfSalve();

                if (combatType == CombatType.Player)
                    if (OnProjectileSpawn != null) OnProjectileSpawn();

                float delay = weapon.timeBetweenShots;
                yield return new WaitForSecondsRealtime(delay);
            }

            yield break;
        }

        private void SpawnProjectileAtPoint(RangedWeapon weapon, Vector3 spawnPoint)
        {
            Projectile projectile = InstantiateProjectile(weapon, spawnPoint, attackPoint.forward);
            SendInformationToProjectile(weapon, projectile, spawnPoint, Vector3.zero);
            
            // needed?
            //SpawnProjectileEvents(weapon);
        }
        private void SpawnProjectile(RangedWeapon weapon, int indexOfSalve)
        {
            Vector3 spawnPoint = CalculateSpawnPoint(weapon, indexOfSalve);

            Vector3 forceDirection = CalculateForceDirectionWithSpread(weapon, spawnPoint);
            print("This cannot be direction: " + forceDirection);
            forceDirection = MathsExtension.RotateVectorByVerticalAngle(weapon.verticalAngle, forceDirection, attackPoint);

            Projectile projectile = InstantiateProjectile(weapon, spawnPoint, forceDirection);

            float initialAcceleration = CalculateInitialAcceleration(weapon);
            Vector3 forceAdded = AddForceToProjectile(projectile, forceDirection, initialAcceleration);

            SendInformationToProjectile(weapon, projectile, spawnPoint, forceAdded);

            if (initialAcceleration >= raycastInitialAccelThreshold)
                StartCoroutine(FireRaycast(spawnPoint, forceDirection, initialAcceleration, projectile.GetComponent<Projectile>()));

            // set additional information (only if needed to save processing power)
            /// bit of a clunky system, could be improved
            if (weapon.useCameraAsSpawnPoint)
                projectile.SetAdditionalInformation(weapon.useCameraAsSpawnPoint ? cam : attackPoint, MathsExtension.CalculateSpawnPosOffset(weapon.spawnPosData, indexOfSalve));

            SpawnProjectileEvents(weapon, indexOfSalve, initialAcceleration);
        }

        private Vector3 CalculateSpawnPoint(RangedWeapon weapon, int indexOfSalve)
        {
            Transform spawnPointTransform = weapon.useCameraAsSpawnPoint ? cam : attackPoint;
            Vector3 spawnPoint = spawnPointTransform.position;
            if (weapon.useCustomSpawnPoints)
                spawnPoint = MathsExtension.CalculateSpawnPos(spawnPointTransform, spawnPoint, weapon.spawnPosData, indexOfSalve);

            return spawnPoint;
        }

        private Vector3 CalculateForceDirectionWithSpread(RangedWeapon weapon, Vector3 spawnPoint)
        {
            print("calculating force direction with " + currSpread + " spread");
            Vector3 spreadDirectionOffset = MathsExtension.CalculateRelativeSpreadVector(cam.transform, currSpread, currMinSpread);

            Vector3 baseDirection = cam.forward;
            Vector3 baseDirectionWithSpread = baseDirection + spreadDirectionOffset;
            Vector3 forceDirection = default;

            RaycastHit hit;

            if (weapon.useStrictCameraForward)
                forceDirection = cam.forward;

            else if (Physics.Raycast(cam.position, baseDirectionWithSpread, out hit))
            {
                forceDirection = (hit.point - spawnPoint).normalized + spreadDirectionOffset;
                lastAttackViewPoint = hit.point;
                //FindObjectOfType<DebugExtensionManager>().PlaceMarker(hit.point, 0.2f);
            }  

            else
                forceDirection = cam.forward;

            if(forceDirection == cam.forward)
                lastAttackViewPoint = cam.position + cam.forward * 50f;

            return forceDirection;
        }

        private Projectile InstantiateProjectile(RangedWeapon weapon, Vector3 spawnPoint, Vector3 forceDirection)
        {
            return InstantiateProjectile(weapon.magazine.projectileName, spawnPoint, forceDirection);
        }
        public Projectile InstantiateProjectile(string projectileName, Vector3 spawnPoint, Vector3 forceDirection)
        {
            Inventory inv = PlayerReferences.instance.inventory;
            GameObject projectilePref = Resources.Load<GameObject>(projectileName);
            GameObject projectileObj = Instantiate(projectilePref, spawnPoint, Quaternion.identity);
            projectileObj.transform.forward = forceDirection.normalized;
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            return projectile;
        }

        public float CalculateInitialAcceleration(RangedWeapon weapon) 
        { 
            return CalculateInitialAcceleration(weapon.force); 
        }
        public float CalculateInitialAcceleration(float force)
        {
            /// formula: a = v / t -> v = a * t (t = Time.fixedDeltaTime)
            float ammoWeight = 0.010f;
            float forceMultiplier = 0.01f; // for now, since I don't know how netwons work haha
            float initialAcceleration = (force * forceMultiplier) / ammoWeight;
            return initialAcceleration;
        }

        public Vector3 AddForceToProjectile(Projectile projectile, Vector3 forceDirection, float initialAcceleration)
        {
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            Vector3 forceToAdd = forceDirection.normalized * initialAcceleration;
            print("This cannot be: " + forceToAdd);
            projectileRb.AddForce(forceToAdd, ForceMode.Impulse);

            return forceToAdd;
        }

        private void SendInformationToProjectile(RangedWeapon weapon, Projectile projectile, Vector3 spawnPoint, Vector3 forceAdded)
        {
            List<ProjectileEffectApplier> playerStatEffects = new List<ProjectileEffectApplier>();

            // what effects need to be sent to the projectile?? The one's of the player?
            // or are they somehow calculated when taking damage, then the projectile only needs to store from
            // whom the damage came

            projectile.SetInformation(this, playerStatEffects, forceAdded, spawnPoint, weapon.damageMultiplier, weapon.damageFalloff);
        }

        private void SpawnProjectileEvents(RangedWeapon weapon, int indexOfSalve, float initialAcceleration)
        {
            bool firstTime = !weapon.countSalveAsOneShot || indexOfSalve == 0;
            bool lastTime = !weapon.countSalveAsOneShot || indexOfSalve == weapon.ammoFireAmount - 1;

            // PlayerOnly
            if (gameObject.tag == "Player")
            {
                // first time
                if (firstTime)
                {
                    // audio
                    SoundManager.PlayEffect(weapon.weaponSounds.fireSound);

                    // ultimate blaster
                    if (ultimateBlaster != null)
                        ultimateBlaster.Fire(initialAcceleration, weapon.cooldown, weapon.heat);

                    // cam shake -> handle with effect playlist instead
                    /// PlayerReferences.instance.cam.PlayShake(currCycle.weapon.shakePreset);

                    // gun shake
                    //weaponHolder.PlayGunShake();
                    float recoveryTime = weapon.ammoFireAmount > 1 ? weapon.timeBetweenShots : weapon.cooldown;
                    float intensity = weapon.ammoFireAmount > 1 ? 0.35f : 1f;
                    weaponHolder.PlayWeaponRecoil(intensity, recoveryTime);

                    // rb recoil
                    if (weapon.recoilRigidbodyForce > 0)
                    {
                        /// custom max speed (old system, needs to be updated soon)
                        if (weapon.recoilRigidbodyMaxSpeed != 15)
                            movement.AddForce(-attackPoint.forward * weapon.recoilRigidbodyForce, weapon.recoilRigidbodyMaxSpeed);
                        else
                            movement.AddForce(-attackPoint.forward * weapon.recoilRigidbodyForce, -1);
                    }

                    // post processing effects
                    if (weapon.enablePostProcessingEffects)
                    {
                        for (int i = 0; i < weapon.postProcessingEffects.Count; i++)
                        {
                            PostProcessingManager.instance.SetupEffect(weapon.postProcessingEffects[i]);
                        }
                    }

                    // fast fire
                    if (weapon.enableFastFireBuildup)
                    {
                        weapon.FastFireBuildupStep();
                        timerTillFastFireReset = weapon.timerUntilReset;
                    }

                    // overheating
                    if (weapon.enableOverheating)
                    {
                        weapon.HeatBuildupStep();
                        timerTillHeatDecrease = weapon.timeUntilHeatDecrease;
                    }
                }

                // last time
                if (lastTime)
                {
                    // recoil
                    Vector3 recoilXyz = Vector3.Lerp(weapon.stabilityNormal.recoilCam, weapon.stabilityWithAds.recoilCam, weaponHolder.GetAdsProgress());
                    camRecoil.AddRecoil(recoilXyz);
                }
            }

            // General
            /// spawn muzzle flash and adjust size
            if (firstTime && weapon.enableMuzzleEffects)
            {
                GlobalEffectsSpawner.instance.SpawnMuzzleEffect(weapon.muzzleEffect, attackPoint.position, ultimateBlaster.transform, attackPoint);
                print("spawning muzzle effect");
            }
        }


        private IEnumerator FireRaycast(Vector3 spawnPoint, Vector3 direction, float initialAcceleration, Projectile projectile)
        {
            // fire raycast one to find the target

            bool targetFound = false;
            float timeUntilImpact = 0f;

            RaycastHit targetHit;
            if(Physics.Raycast(spawnPoint, direction, out targetHit, 100f, whatIsTarget))
            {
                print("raycast2: targetFound " + targetHit.transform.name);

                targetFound = true;
                float distanceToTarget = Vector3.Distance(spawnPoint, targetHit.point);
                timeUntilImpact = distanceToTarget / initialAcceleration;

                print("raycast3: timeUntilImpact: " + timeUntilImpact + " = " + distanceToTarget + "/" + initialAcceleration);
            }

            if(!targetFound)
                yield return null;

            yield return new WaitForSecondsRealtime(timeUntilImpact);

            // fire second and final ray to check if target would have been hit

            if (Physics.Raycast(spawnPoint, direction, out targetHit, 100f, whatIsTarget))
            {
                projectile.OnRaycastHitDetected(targetHit.point, targetHit.transform.gameObject);
                print("raycast4: OnRaycastHitDetected");
            }
        }

        private IEnumerator CallEventsIfWeaponIsNotRunning(int weaponId, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (IsWeaponRunning(weaponId))
                yield break;

            print("Weapon with id " + weaponId + " has stopped running");

            if (OnWeaponStoppedRunning != null)
                OnWeaponStoppedRunning();
        }

        private bool IsWeaponRunning(int weaponId)
        {
            bool flag = false;
            for (int i = 0; i < runningWeaponIds.Count; i++)
            {
                if (runningWeaponIds[i] == weaponId)
                    flag = true;
            }
            return flag;
        }

        private void RemoveRunningWeaponId(int weaponId)
        {
            for (int i = 0; i < runningWeaponIds.Count; i++)
            {
                if (runningWeaponIds[i] == weaponId)
                    runningWeaponIds.RemoveAt(i);
            }
        }

        #endregion

        #region Getters

        public bool GetAdsActive() { return adsActive; }

        #endregion

        #region Debugging

        private void TextDebug()
        {
            text_state.SetText("RcState: " + state.ToString());
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 10f);
        }
    }
}
