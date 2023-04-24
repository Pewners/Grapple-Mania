using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRecoil_MLab : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float snapiness;
    [SerializeField] private float returnSpeed;
    [SerializeField] private float returnAfterDuration;

    [Header("Rotations")]
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private bool isReturning;
    private float timeElapsedAfterLastRecoil;

    private void Update()
    {
        // start returning after duration
        timeElapsedAfterLastRecoil += Time.deltaTime;
        isReturning = timeElapsedAfterLastRecoil > returnAfterDuration;

        if (isReturning)
            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);

        // smoothly follow the target rotation
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }

    // x -> up and down
    // y -> left and right
    // z -> tilting
    public void AddRecoil(Vector3 recoilXyz)
    {
        print("camRecoilAdded " + recoilXyz.ToString());

        targetRotation += new Vector3(recoilXyz.x, Random.Range(-recoilXyz.y, recoilXyz.y), Random.Range(-recoilXyz.z, recoilXyz.z));

        timeElapsedAfterLastRecoil = 0;
    }
}
