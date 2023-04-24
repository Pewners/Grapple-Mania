using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Popup : MonoBehaviour
{
    [Header("Settings")]
    public float moveUpDistance;
    public float peakScale;
    public float endScale;
    [Range(0f,1f)]
    public float peakScalePoint;

    public float lifeTime = 1f;

    private bool setupDone;

    public void Setup(string content, Color color)
    {
        GetComponent<TextMeshPro>().SetText(content);
        GetComponent<TextMeshPro>().color = color;
        setupDone = true;

        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        Vector3 endPos = transform.position + transform.up * moveUpDistance;
        transform.DOMove(endPos, lifeTime);

        float durationTillPeakScale = lifeTime * peakScalePoint;
        transform.DOScale(peakScale, durationTillPeakScale);

        yield return new WaitForSeconds(durationTillPeakScale);

        float durationLeft = lifeTime - durationTillPeakScale;
        transform.DOScale(endScale, durationLeft);

        yield return new WaitForSeconds(durationLeft);

        Destroy(gameObject);
    }
}
