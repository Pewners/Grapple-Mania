using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Dave MovementLab - MoveCamera
///
// Content:
/// - Moves the cameraHolder to the position of the player
/// 
// Note:
/// This script is assigned to the CameraHolder


public class MoveCamera_MLab : MonoBehaviour
{
    public Transform cameraPos; // an empty gameObject inside the player, that indicates where the camera should be

    public Transform remoteTarget = null;

    private void Update()
    {
        if(remoteTarget != null)
        {
            transform.position = remoteTarget.position;
        }

        // move the cameraHolder to the intende position
        else
        {
            transform.position = cameraPos.position;
        }
    }

    public void SetRemoteTarget(Transform target)
    {
        remoteTarget = target;
    }

    public void RemoveRemoteTarget()
    {
        remoteTarget = null;
    }
}
