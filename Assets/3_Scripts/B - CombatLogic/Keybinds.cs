using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
[CreateAssetMenu(fileName = "New Keybinds", menuName = "RcLab/System/Keybinds")]
// one reason to change:
// if new keybinds need to be added
public class Keybinds : ScriptableObject
{
    [Header("Main")]
    public KeyCode interact = KeyCode.E;
    public KeyCode reload = KeyCode.R;

    [Header("Abilities")]
    public List<KeyCode> abilityKeys = new List<KeyCode>()
        {
            KeyCode.Mouse0, // -> primaryFire
            KeyCode.Mouse1, // -> secondaryFire
            KeyCode.LeftShift, // -> primaryMovement
            KeyCode.LeftControl, // -> secondaryMovement
            KeyCode.Q, // -> primaryAbility
            KeyCode.E, // -> secondaryAbility
            KeyCode.F, // -> tertiaryAbility
            KeyCode.X // -> specialAbility
        };

    [Header("Numbers")]
    public List<KeyCode> numberKeys = new List<KeyCode>()
        {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };
}
