using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiBar : UiElement
{
    [Header("References")]
    public Image image_fill;
    public float reduceSpeed = 2;

    private float target;

    public void UpdateElement(float value, float maxValue)
    {
        target = value / maxValue;
    }

    public override void UpdateElement()
    {
        base.UpdateElement();
    }

    public void ChangeColor(Color color)
    {
        image_fill.color = color;
    }

    private void Update()
    {
        image_fill.fillAmount = Mathf.MoveTowards(image_fill.fillAmount, target, reduceSpeed * Time.deltaTime);
    }
}
