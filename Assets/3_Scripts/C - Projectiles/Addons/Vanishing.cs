using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vanishing : MonoBehaviour
{
    // vanish settings
    public float vanishStartDelay;
    public float vanishDuration;

    public List<MeshRenderer> componentsToDeactivate;
    public List<GameObject> gameObjectsToDeactivate;

    private void Start()
    {
        Invoke(nameof(Vanish), vanishStartDelay);
    }

    private void Vanish()
    {
        for (int i = 0; i < componentsToDeactivate.Count; i++)
            componentsToDeactivate[i].enabled = false;

        for (int i = 0; i < componentsToDeactivate.Count; i++)
            gameObjectsToDeactivate[i].SetActive(false);

        Invoke(nameof(Reappear), vanishDuration);
    }

    private void Reappear()
    {
        for (int i = 0; i < componentsToDeactivate.Count; i++)
            componentsToDeactivate[i].enabled = true;

        for (int i = 0; i < componentsToDeactivate.Count; i++)
            gameObjectsToDeactivate[i].SetActive(true);
    }
}
