using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RcLab
{
    public class Action : ScriptableObject
    {
        public string actionName = "New Action";

        public ActionType actionType;
        public enum ActionType
        {
            Ranged,
            Movement,
            Melee
        }

        public float startDelay = 0f;
        public bool skipActionDuration = false;
        public float timeTillNextAction = 0f;

        public int iterations = 1;

        public virtual RangedWeapon GetRangedWeapon()
        {
            return null;
        }

        public virtual float GetDuration()
        {
            return 0f;
        }

        public virtual float GetActiveDuration()
        {
            return 0f;
        }

        public virtual float GetDurationWithoutCooldown()
        {
            return 0f;
        }

        public virtual float GetCooldown()
        {
            return 0f;
        }

        public virtual HandsNeeded GetHandsNeeded()
        {
            return HandsNeeded.None;
        }
    }
}
