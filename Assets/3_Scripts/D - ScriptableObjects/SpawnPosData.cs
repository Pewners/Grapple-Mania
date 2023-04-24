using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnPosData
{
    public Mode mode;
    public enum Mode
    {
        Free,
        Line,
        Circle
    }

    public List<FreePointsCreator> freePointsCreator;
    public List<LineCreator> lineCreator;
    public List<CircleCreator> circleCreator;

    public int GetSpawnPointAmount()
    {
        int amount = 0;

        if(mode == Mode.Line)
        {
            for (int i = 0; i < lineCreator.Count; i++)
                amount += lineCreator[i].pointAmount;
        }

        else if (mode == Mode.Free)
        {
            for (int i = 0; i < lineCreator.Count; i++)
                amount++;
        }

        else if (mode == Mode.Circle)
        {
            for (int i = 0; i < circleCreator.Count; i++)
                amount += circleCreator[i].pointAmount;
        }

        return amount;
    }
}

[Serializable]
public class FreePointsCreator
{
    public Vector3 relativePoint;
}

[Serializable]
public class LineCreator 
{
    [Range(1, 10)] public int pointAmount = 1;
    [Range(-5f, 5f)] public float startPos = -1f;
    [Range(-5f, 5f)] public float endPos = 1f;
    [Range(-90f, 90f)] public float rotation = 0f;
    public Vector3 baseOffset = Vector3.zero;
}

[Serializable]
public class CircleCreator
{
    [Range(1, 10)] public int pointAmount = 1;
    public float radius;
    public Vector3 baseOffset;
}