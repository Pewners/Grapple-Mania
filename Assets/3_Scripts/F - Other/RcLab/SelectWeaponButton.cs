using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectWeaponButton : MonoBehaviour
{
    public TextMeshProUGUI text_name;
    private string nameStored;
    private int index;

    public void Setup(string name, int _index)
    {
        text_name.SetText(name);
        nameStored = name;
        index = _index;
    }

    public void SelectWeapon()
    {
        FindObjectOfType<ViewRcLabSelection>().OnSelectWeaponClick(index);
    }
}
