using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using System.Runtime.CompilerServices;
using System;
using UnityEngine.Events;
using System.Linq;

public class PlayerAbilities : MonoBehaviour
{
    // references
    private ActionPlayer actionPlayer;
    private EffectPlayer effectPlayer;
    private Combat combat;
    private Inventory inv;

    // coroutines
    [SerializeField] private Dictionary<int, Coroutine> coroutineLookup = new Dictionary<int, Coroutine>();
    [SerializeField] private Dictionary<int, PlayingAbilityData> playingAbilityLookup = new Dictionary<int, PlayingAbilityData>();

    // events
    public UnityAction OnWeaponCooldownFinished;
    public UnityAction<RangedWeapon> OnComplexWeaponSwitch;

    private void Start()
    {
        actionPlayer = PlayerReferences.instance.actionPlayer;
        effectPlayer = PlayerReferences.instance.effectPlayer;
        combat = PlayerReferences.instance.combat;
        inv = PlayerReferences.instance.inventory;
    }

    private void Update()
    {
        if(playingAbilityLookup.Count > 0)
        {
            foreach (PlayingAbilityData playingAbility in playingAbilityLookup.Values.ToList())
            {
                if (playingAbility == null) continue;
                if (playingAbility.cooldownStarted == false) continue;

                if (playingAbility.cooldown > 0)
                    playingAbility.cooldown -= Time.deltaTime;

                if (playingAbility.isWeaponAbility && playingAbility.cooldown <= 0)
                    OnWeaponCooldownFinished?.Invoke();
            }
        }
    }

    public void PlayAbility(Ability ability)
    {
        int id = ability.GetInstanceID();

        if(coroutineLookup.ContainsKey(id) )
            return;

        if (playingAbilityLookup.ContainsKey(id))
        {
            if (playingAbilityLookup[id].cooldown > 0)
            {
                print("Ability with id " + id + "is still in cooldown");
                return;
            }
            else
            {
                playingAbilityLookup.Remove(id);
            }
        }

        // 1 - Create PlayingAbility
        PlayingAbilityData playingAbility = new PlayingAbilityData();
        playingAbility.effectPlaylistId = ability.effectPlaylist.GetHashCode();
        playingAbilityLookup.Add(id, playingAbility);

        // 2 - Start Coroutine
        print("playing ability " + ability.abilityName + " with id " + id);
        Coroutine coroutine = StartCoroutine(AbilityCycle(ability));
        coroutineLookup.Add(id, coroutine);
    }

    public void StopAbility(int abilityId)
    {
        if (!coroutineLookup.ContainsKey(abilityId))
            return;

        StopCoroutine(coroutineLookup[abilityId]);
        coroutineLookup.Remove(abilityId);

        PlayingAbilityData playingAbility = playingAbilityLookup[abilityId];
        if(playingAbility.effectPlaylistCoroutine != null)
            StopCoroutine(playingAbility.effectPlaylistCoroutine);
        if (!playingAbility.cooldownStarted)
            StartCoroutine(StartCooldown(playingAbility, playingAbility.cooldown, true));

        print("stopped ability with id " + abilityId + " cooldown started " + playingAbility.cooldownStarted);

        //effectPlayer.StopPlaylist(playingAbility.effectPlaylistId);
    }

    // whenever any ability cooldown has finished and the left mouse button is pressed
    // try to start the primary ability again (some misscalls, but whatever)
    private void WeaponCooldownFinished()
    {
        if(OnWeaponCooldownFinished != null)
            OnWeaponCooldownFinished();

        print("Event: OnAnyCooldownFinished");
    }

    public IEnumerator AbilityCycle(Ability ability)
    {
        print("ability cycle started -> " + ability.actions.Count + " actions to play");

        PlayingAbilityData playingAbility = playingAbilityLookup[ability.GetInstanceID()];
        playingAbility.cooldown = ability.GetCooldown();

        if(ability.effectPlaylist != null)
            playingAbility.effectPlaylistCoroutine = StartCoroutine(effectPlayer.EffectPlaylistCycle(ability.effectPlaylist));

        // weapon shortcut
        if (ability.isWeapon && ability.weaponType == Ability.WeaponType.simpleWeapon)
        {
            RangedWeapon weapon = ability.GetFirstRangedWeapon();
            combat.PlayRangedWeapon(weapon);
            print("weaponCycleDuration" + weapon.GetWeaponCycleDurationWithoutCooldown());
            yield return new WaitForSeconds(weapon.GetWeaponCycleDurationWithoutCooldown());

            // don't yield the IEnumerator! This makes it a child coroutine that stops together with the ability
            yield return StartCoroutine(StartCooldown(playingAbility, weapon.cooldown, ability.isWeapon));;
        }

        // normal action cycle
        else
        {
            for (int i = 0; i < ability.actions.Count; i++)
            {
                RcLab.Action action = ability.actions[i];

                for (int a = 0; a < action.iterations; a++)
                {
                    print("total action duration: " + (action.startDelay + (action.skipActionDuration ? 0f : action.GetDuration()) + action.timeTillNextAction));

                    yield return new WaitForSeconds(action.startDelay);

                    print("complex ranged aciton? " + ability.IsComplexWeaponAbility() + " " + (action.actionType == RcLab.Action.ActionType.Ranged));
                    if (ability.IsComplexWeaponAbility() && action.actionType == RcLab.Action.ActionType.Ranged)
                    {
                        if (!TryReduceComplexWeaponMagazine(ability))
                        {
                            yield return new WaitForSeconds(action.timeTillNextAction);
                            continue;
                        }
                    }

                    actionPlayer.PlayAction(action);

                    float actionDuration = action.skipActionDuration ? action.GetActiveDuration() : action.GetDurationWithoutCooldown();
                    yield return new WaitForSeconds(actionDuration);

                    HandleComplexWeaponSwitch(ability, i);

                    float actionCooldown = action.skipActionDuration ? 0f : action.GetCooldown();

                    yield return new WaitForSeconds(actionCooldown);

                    yield return new WaitForSeconds(action.timeTillNextAction);
                }
            }

            // don't yield the IEnumerator! This makes it a child coroutine that stops together with the ability
            yield return StartCoroutine(StartCooldown(playingAbility, ability.GetCooldown(), ability.isWeapon));;
        }

        /// I don't fully understand whether StopCoroutine actually stops the coroutine while playing or just after it finished
        /// Anyways, checking if the coroutineIsInCooldown just to make sure that the looping works correctly

        // loop ability if it's a weapon
        if (ability.isWeapon)
        {
            if (!ability.IsSpawnOnlyOnMouseDown())
            {
                if (coroutineLookup.ContainsKey(ability.GetInstanceID()))
                    coroutineLookup[ability.GetInstanceID()] = StartCoroutine(AbilityCycle(ability));
            }
        }
    }

    private void HandleComplexWeaponSwitch(Ability ability, int i)
    {
        RcLab.Action action = ability.GetNextActionInAbility(i);
        if (ability.IsComplexWeaponAbility() && action.actionType == RcLab.Action.ActionType.Ranged)
            OnComplexWeaponSwitch?.Invoke(action.GetRangedWeapon());
    }

    private bool TryReduceComplexWeaponMagazine(Ability ability)
    {
        Magazine mag = ability.GetMag();

        print("subtract ammo from complex weapon " + mag);
        if (mag.ammoLeft <= 0)
        {
            inv.StopWeaponFire(0);
            inv.StartReload(mag);
            return false;
        }

        inv.SubtractAmmoFromMagazine(mag, 1);

        return true;
    }

    private IEnumerator StartCooldown(PlayingAbilityData playingAbility, float cooldown, bool isWeaponAbility)
    {
        print("ability cooldown started");
        playingAbility.cooldownStarted = true;
        playingAbility.isWeaponAbility = isWeaponAbility;
        //while (playingAbility.cooldown >= 0f)
        //{
        //print("ability cooldown running...");
        //playingAbility.cooldown -= Time.deltaTime;
        //yield return null;
        //}

        // handled in void update, somehow the coroutine is cancelled unwanted sometimes idk

        yield return new WaitForSeconds(cooldown);
    }
}

public class PlayingAbilityData
{
    public float cooldown;
    public int effectPlaylistId;
    public bool cooldownStarted;
    public Coroutine effectPlaylistCoroutine;
    public Magazine complexWeaponMagazine;
    public bool isWeaponAbility;
    public float weaponCooldown;
}