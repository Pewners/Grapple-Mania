using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Dave
{
    public static class MathsExtension
    {
        #region SpawnPosCalculations

        public static Vector3 CalculateSpawnPos(Transform relativeTransform, Vector3 spawnPoint, SpawnPosData data, int pointIndex)
        {
            Vector3 spawnPointOffset = CalculateSpawnPosOffset(data, pointIndex);

            Debug.Log("MathsExtension: SpawnPointOffset is " + spawnPointOffset);

            if (spawnPointOffset != default)
            {
                spawnPoint += relativeTransform.right * spawnPointOffset.x;
                spawnPoint += relativeTransform.up * spawnPointOffset.y;
                spawnPoint += relativeTransform.forward * spawnPointOffset.z;
            }

            return spawnPoint;
        }

        public static Vector3 CalculateSpawnPosOffset(SpawnPosData data, int pointIndex)
        {
            if (data.mode == SpawnPosData.Mode.Free)
            {
                return FreePointsOffset(data.freePointsCreator[pointIndex]);
            }

            else if (data.mode == SpawnPosData.Mode.Line)
            {
                // find correct line and relative linePointIndex
                LineCreator lineCreator = data.lineCreator[0];
                int linePointIndex = pointIndex;

                int count = 0;
                for (int i = 0; i < data.lineCreator.Count; i++)
                {
                    int _linePointIndex = 0;

                    for (int y = 0; y < data.lineCreator[i].pointAmount; y++)
                    {
                        if (count == pointIndex)
                        {
                            lineCreator = data.lineCreator[i];
                            linePointIndex = _linePointIndex;
                        }
                        count++;
                        _linePointIndex++;
                    }
                }

                return LineOffset(lineCreator, linePointIndex);
            }

            /// Note: A circle is kind of just a line - right?
            else if (data.mode == SpawnPosData.Mode.Circle)
                return CircleOffset(data.circleCreator[0], pointIndex);

            Debug.LogError("Something really went wrong here haha");
            return Vector3.zero;
        }

        private static Vector3 FreePointsOffset(FreePointsCreator data)
        {
            return data.relativePoint;
        }

        private static Vector3 LineOffset(LineCreator data, int linePointIndex)
        {
            float lineLength = data.endPos - data.startPos;
            float lineSegmentLength = lineLength;

            if(data.pointAmount != 1)
                lineSegmentLength = lineLength / (data.pointAmount - 1);

            float xOffset = data.startPos + lineSegmentLength * linePointIndex;

            Vector3 offset = new Vector3(xOffset, 0f, 0f);
            return offset + data.baseOffset;
        }

        private static Vector3 CircleOffset(CircleCreator data, int pointIndex)
        {
            Debug.LogWarning("CircleOffset has not been implemented yet!");
            return Vector3.zero;
        }

        #endregion

        #region Spread & Recoil

        public static Vector3 CalculateRelativeSpreadVector(Transform forwardT, float minSpread, float spread)
        {
            float xSpread = UnityEngine.Random.Range(minSpread, spread) * 0.01f;
            float ySpread = UnityEngine.Random.Range(minSpread, spread) * 0.01f;

            bool inverseXSpread = UnityEngine.Random.Range(0, 2) == 1;
            bool inverseYSpread = UnityEngine.Random.Range(0, 2) == 1;
            if (inverseXSpread) xSpread = -xSpread;
            if (inverseYSpread) ySpread = -ySpread;

            Vector3 spreadDirectionOffset = forwardT.right * xSpread + forwardT.up * ySpread;

            return spreadDirectionOffset;
        }

        public static Vector3 AddSpreadToVector(Transform forwardT, float minSpread, float spread)
        {
            Vector3 forwardDirectionWithSpread = forwardT.forward + CalculateRelativeSpreadVector(forwardT, minSpread, spread);

            return forwardDirectionWithSpread;
        }

        #endregion

        #region Vector Math & Quaternions

        public static Vector3 RotateVectorByVerticalAngle(float verticalAngle, Vector3 forceDirection, Transform relativeForward)
        {
            if (verticalAngle != 0)
            {
                Quaternion turnUp = Quaternion.AngleAxis(verticalAngle, relativeForward.right);

                // quaternion needs to be on the left!!!
                forceDirection = turnUp * forceDirection;
            }

            return forceDirection;
        }

        #endregion

        #region Damage

        public static float CalculateDamageWithFalloff(float damage, float travelDistance, RangedWeapon.DamageFalloffStats falloffStats)
        {
            if(falloffStats == null)
                return damage;

            // falloff hasn't started
            if (travelDistance < falloffStats.startDistance) 
                return damage;

            float relativeDistanceFromStart = travelDistance - falloffStats.startDistance;
            float totalDistanceFromStartToEnd = falloffStats.endDistance - falloffStats.startDistance;

            // how much of the "falloff path" has been travelled, 1 means full falloff
            float falloffDelta = relativeDistanceFromStart / totalDistanceFromStartToEnd;

            float damageMultiplier = falloffStats.damageMultiplierCurve.Evaluate(falloffDelta);
            if (damageMultiplier < 0)
                return damage;

            float adjustedDamage = damage * damageMultiplier;

            Debug.Log("DamageFalloff calculation: " + "\n" +
                      "travelDistance: " + travelDistance + "\n" +
                      " fallOffStartDistance: " + falloffStats.startDistance + "\n" +
                      " fallOffEndDistance: " + falloffStats.endDistance + "\n" +
                      " finalMultiplier: " + damageMultiplier);

            return adjustedDamage;
        }

        #endregion

        #region LayerMasks

        public static int CalculateLayerMask(int layer)
        {
            return 1 << layer;
        }

        public static int CombineLayerMask(int layer1, int layer2)
        {
            int layermask1 = 1 << layer1;
            int layermask2 = 1 << layer2;
            int combinedLayerMask = layermask1 | layermask2; // Or, (1 << layer1) | (1 << layer2)

            return combinedLayerMask;
        }

        #endregion

        #region GeneralMaths

        public static float Round(float value, int digits)
        {
            float mult = Mathf.Pow(10.0f, (float)digits);
            return Mathf.Round(value * mult) / mult;
        }

        #endregion
    }
}
