using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Direction
{
    public Type type;
    public enum Type
    {
        FixedVector,
        LocalDirection
    }

    public Vector3 fixedVector;

    [Range(-1f, 1f)] public float localXAxis;
    [Range(-1f, 1f)] public float localYAxis;
    [Range(-1f, 1f)] public float localZAxis;

    #region Constructors

    public Direction(Type type, Vector3 vector)
    {
        this.type = type;

        if (type == Type.FixedVector)
        {
            this.fixedVector = vector;
        }

        else if (type == Type.LocalDirection)
        {
            this.localXAxis = vector.x;
            this.localYAxis = vector.y;
            this.localZAxis = vector.z;
        }
    }

    #endregion

    #region Setters

    public void RemoveUpwardsAxis()
    {
        if (type == Type.FixedVector)
            fixedVector = new Vector3(fixedVector.x, 0f, fixedVector.z);
        else if (type == Type.LocalDirection)
            localYAxis = 0f;
    }

    public void ResetToZero()
    {
        if (type == Type.FixedVector)
            fixedVector = Vector3.zero;
        else if (type == Type.LocalDirection)
        {
            localXAxis = 0f;
            localYAxis = 0f;
            localZAxis = 0f;
        }
    }

    #endregion

    #region Getters

    public Vector3 GetNormalizedVector(Transform localTransform = null)
    {
        if (type == Type.FixedVector)
        {
            return fixedVector.normalized;
        }

        else if (type == Type.LocalDirection)
        {
            if (localTransform == null)
            {
                Debug.LogError("No localTransform given, can't calculate directional vector");
                return Vector3.zero;
            }

            Vector3 direction = Vector3.zero;
            direction += localTransform.right * localXAxis;
            direction += localTransform.forward * localZAxis;
            direction += localTransform.up * localYAxis;
            return direction.normalized;
        }

        else
        {
            Debug.LogError("Not supported Direction Type");
            return Vector3.zero;
        }
    }

    #endregion
}
