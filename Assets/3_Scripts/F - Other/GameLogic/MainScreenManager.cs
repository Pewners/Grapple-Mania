using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreenManager : MonoBehaviour
{
    private void Update()
    {
        if(Cursor.visible == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void StartRunCall()
    {
        GameLogic.instance.StartRun();
    }
}
