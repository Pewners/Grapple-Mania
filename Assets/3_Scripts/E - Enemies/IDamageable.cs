using RcLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, Color color = default);

    Stats GetPlayerStatsObj();
}
