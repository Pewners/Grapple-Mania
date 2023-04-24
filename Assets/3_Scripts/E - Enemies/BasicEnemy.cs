using Dave;
using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, IDamageable
{
    public float maxHealth;
    [HideInInspector] public float health { get; set; }

    public Healthbar healthbar;

    private bool destroyed;

    private void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage, Color color = default)
    {
        health -= damage;

        if(healthbar != null)
            healthbar.UpdateHealthBar(maxHealth, health);

        print(gameObject.name + " damage: " + damage);

        float roundedDamage = MathsExtension.Round(damage, RcLabSettings.roundDamageToDecimals);
        FindObjectOfType<DebugExtensionManager>().SpawnPopup(transform.position, DebugExtensionManager.PopupType.Damage, roundedDamage.ToString(), color);

        // make sure enemy reacts to damage
        if(GetComponent<EnemyAiTutorial>() != null)
            GetComponent<EnemyAiTutorial>().sightRange = 100f;
        StopAllCoroutines();
        StartCoroutine(nameof(DelayedReset), 3f);

        if(health <= 0 && !destroyed)
        {
            Destruction();
            destroyed = true;
        }
    }

    private void DelayedReset()
    {
        GetComponent<EnemyAiTutorial>().ResetSightRange();
    }

    public void Destruction()
    {
        // call events
        GameLogic.instance.OnEnemyDefeated();

        Invoke(nameof(DelayedDestruction), 0.1f);
    }

    public void DelayedDestruction()
    {
        Destroy(gameObject);
    }

    public Stats GetPlayerStatsObj()
    {
        throw new System.NotImplementedException();
    }
}
