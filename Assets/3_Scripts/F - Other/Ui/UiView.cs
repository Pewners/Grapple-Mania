using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiView : MonoBehaviour
{
    public Type type;
    public bool stationary;
    private bool active;

    public enum Type
    {
        AmmoStation,
        WeaponStation,
        AlchemyStation,
        Inventory,
        SubView,
        IngameUi,
        RcLabWeaponSwitcher
    }

    public virtual void Show()
    {
        active = true;
    }

    public virtual void Hide()
    {
        active = false;
    }

    #region Getters

    public bool IsActive()
    {
        return active;
    }

    #endregion
}
