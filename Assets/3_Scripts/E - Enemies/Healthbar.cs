using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("Settings")]
    public bool dontRotate = false;
    public Image healthbarSprite;
    public float reduceSpeed = 2;
    private float target = 1;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        target = currentHealth / maxHealth;
    }

    private void Update()
    {
        if (!dontRotate)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
        }

        healthbarSprite.fillAmount = Mathf.MoveTowards(healthbarSprite.fillAmount, target, reduceSpeed * Time.deltaTime);
    }
}
