using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Explosive Settings", menuName = "Custom/Settings/Explosive")]
public class ExplosiveSettings : ScriptableObject
{
    public float explosionRadius;
    public float explosionForce;
    public float autoExplosionDelay;
    public int explosionDamage;
    public string explosionEffectName;
}
