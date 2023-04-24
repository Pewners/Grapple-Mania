using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///
/// Nice try, but I guess I could just use SphereCasts from pointA to pointB ?
/// 
/// Also, the rotation stuff isn't really finished, the rest works though
/// 

public class ColliderCreatorTest : MonoBehaviour
{
    public Transform posA;
    public Transform posB;

    public float colliderHeight;
    public float colliderWidth;

    private GameObject colliderObj;
    private BoxCollider boxCollider;

    private bool setupDone;

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        if (!setupDone) return;

        CreateCollider();
    }

    private void Setup()
    {
        colliderObj = Instantiate(new GameObject(), transform);

        colliderObj.AddComponent(typeof(BoxCollider));

        boxCollider = colliderObj.GetComponent<BoxCollider>();

        setupDone = true;
    }

    private void CreateCollider()
    {
        Vector3 distance = posB.position - posA.position;

        Vector3 midPoint = posA.position + distance * 0.5f;

        Vector3 localizedMidPoint = midPoint - colliderObj.transform.position;

        // create a collider with the midPoint as center, then adjust the size and rotation

        colliderObj.transform.position = midPoint;
        boxCollider.center = Vector3.zero;
        boxCollider.size = new Vector3(distance.magnitude, colliderHeight, colliderWidth);

        float angle = Vector3.Angle(distance, posA.right);

        colliderObj.transform.rotation = Quaternion.Euler(0, -angle, 0);

        print("Angle: " + angle);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 distance = posB.position - posA.position;

        Vector3 midPoint = posA.position + distance * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(midPoint, 1f);
    }
}
