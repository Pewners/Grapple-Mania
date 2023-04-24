using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHallEntry : MonoBehaviour, IInteractable
{
    private CombatHallManager chm;

    private void Start()
    {
        chm = FindObjectOfType<CombatHallManager>();
    }

    public void Interact()
    {
        chm.StartRandomWave();
    }
}
