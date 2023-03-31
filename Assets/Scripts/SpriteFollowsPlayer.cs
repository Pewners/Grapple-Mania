using UnityEngine;
using System.Collections;

public class SpriteFollowsPlayer : MonoBehaviour
{
    private Camera cameraToLookAt;

    private void Start()
    {
        cameraToLookAt = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        transform.LookAt(cameraToLookAt.transform);
    }
}