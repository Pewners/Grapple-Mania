
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RcLab;
using UnityEngine.Animations.Rigging;

public class AnimationPlayer : MonoBehaviour
{
    private Animator animator;

    // animations
    public List<WeaponAnimationData> weaponAnimations;
    public List<AnimationData> fixedAnimations;

    // references
    public TwoBoneIKConstraint constraint_leftHand;

    public enum AnimationType
    {
        Base,
        WeaponPunch,
        WeaponReload,
        OffhandPunch,
        OffhandHeavyPunch,
        OffhandCharge
    }

    private WeaponType currWeaponType;
    public enum WeaponType
    {
        Rifle,
        Pistol,
        Sword,
        Bow,
        Hands
    }

    private void Start()
    {
        animator = PlayerReferences.instance.animator;

        SwitchWeaponAnimation(WeaponType.Rifle);
    }
    
    public void SwitchWeaponAnimation(WeaponType _weaponType)
    {
        currWeaponType = _weaponType;

        WeaponAnimationData weaponAnimData = GetWeaponAnimationData(currWeaponType);

        ReturnToBaseAnimation();
    }

    /// All logic regarding interruptions and hand usage get's handled by the ability player and inventory script
    /// The AnimationPlayer therfore accepts any requests, even if that means interrupting currently playing animations
    public void PlayAnimation(AnimationType animationType)
    {
        // get correct animation data and play it
        AnimationData animation = GetAnimation(animationType);
        StopAllCoroutines();
        StartCoroutine(AnimationCycle(animation));
    }

    public IEnumerator AnimationCycle(AnimationData animation)
    {
        animator.CrossFade(animation.animationName, 0f, 0);

        yield return new WaitForSeconds(animation.duration);

        // return to base animation
        ReturnToBaseAnimation();
    }

    private void ReturnToBaseAnimation()
    {
        animator.CrossFade(GetAnimation(AnimationType.Base).animationName, 0f, 0);
    }

    public WeaponAnimationData GetWeaponAnimationData(WeaponType weaponType)
    {
        for (int i = 0; i < weaponAnimations.Count; i++)
        {
            if (weaponAnimations[i].weaponType == weaponType)
                return weaponAnimations[i];
        }

        Debug.LogError("No weaponAnimationData of this type could be found");
        return null;
    }

    public AnimationData GetAnimation(AnimationType animationType)
    {
        // first search through weapon animations
        for (int i = 0; i < weaponAnimations.Count; i++)
        {
            if (weaponAnimations[i].weaponType == currWeaponType)
            {
                for (int y = 0; y < weaponAnimations[i].animations.Count; y++)
                {
                    if (weaponAnimations[i].animations[y].animationType == animationType)
                        return weaponAnimations[i].animations[y];
                }
            }
        }

        // then search through fixed animations
        for (int i = 0; i < fixedAnimations.Count; i++)
        {
            if (fixedAnimations[i].animationType == animationType)
                return fixedAnimations[i];
        }

        Debug.LogError("Animation could not be found");
        return null;
    }

    [Serializable]
    public class WeaponAnimationData
    {
        public WeaponType weaponType;
        public List<AnimationData> animations;
        public Transform leftHandTarget;
        public Transform leftHandHint;
        public Transform rightHandTarget;
        public Transform rightHandHint;
    }

    [Serializable]
    public class AnimationData
    {
        public AnimationType animationType;
        public string animationName;
        public float duration;
    }
}
