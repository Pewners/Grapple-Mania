using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using RcLab;
using Dave;

public class ArcheryTarget : MonoBehaviour, IDamageable
{
    [HideInInspector] public float health { get; set; }

    public Transform target;
    public float spinDuration = 0.25f;
    public float spins = 1f;

    public AudioClipData spinSound;
    public float minSoundInterval = 0.07f;
    private bool noSound;

    private bool spinning;
    private float spinSpeed;

    private Vector3 startEulerRoatation;

    private void Start()
    {
        startEulerRoatation = target.eulerAngles;
    }

    private void Update()
    {
        if (spinning)
        {
            target.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
            print("spinning...");
        }
    }

    public void TakeDamage(float damage, Color _color = default)
    {
        print("archeryTarget hit");

        float roundedDamage = MathsExtension.Round(damage, RcLabSettings.roundDamageToDecimals);

        Color color = _color == default ? Color.red : _color;
        FindObjectOfType<DebugExtensionManager>().SpawnPopup(transform.position, DebugExtensionManager.PopupType.Damage, roundedDamage.ToString(), color);

        StartSpinning();
    }

    private void StartSpinning()
    {
        spinning = true;
        spinSpeed = (360f * spins) / spinDuration;
        Invoke(nameof(StopSpinning), spinDuration);

        SoundManager.PlayEffect(spinSound);
        noSound = true;

        Invoke(nameof(ResetSound), minSoundInterval);
    }
    private void ResetSound()
    {
        noSound = false;
    }

    private void StopSpinning()
    {
        spinning = false;
        target.DORotate(startEulerRoatation, 0.1f);
    }

    public Stats GetPlayerStatsObj()
    {
        throw new System.NotImplementedException();
    }
}
