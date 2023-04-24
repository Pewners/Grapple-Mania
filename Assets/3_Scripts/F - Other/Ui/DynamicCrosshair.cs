using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DynamicCrosshair : MonoBehaviour
{
    public RectTransform crosshair;

    public float spreadToSizeConversionAt1;
    public float spreadToSizeConversionAt10;
    public float sizeChangeSpeed;
    public float baseSize;

    private float currSize;

    private void Update()
    {
        crosshair.sizeDelta = Vector2.one * currSize;
    }

    public void UpdateSize(float spread)
    {
        if (spread < 1) currSize = baseSize + spread * spreadToSizeConversionAt1;
        else
        {
            float progressToTen = spread / 9;
            float estimatedConversion = Mathf.Lerp(spreadToSizeConversionAt1, spreadToSizeConversionAt10, progressToTen);
            currSize = baseSize + spread * estimatedConversion;
        }

        print("updating crosshair to " + spread + " spread " + currSize + " " + crosshair.sizeDelta.x);

        // float sizeDifference = Mathf.Abs(crosshair.sizeDelta.x - targetSize);
        // float duration = sizeDifference / sizePerSecondChangeSpeed;
        // crosshair.DOSizeDelta(Vector2.one * targetSize, duration);
    }
}
