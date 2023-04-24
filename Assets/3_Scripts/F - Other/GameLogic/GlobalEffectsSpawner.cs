using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEffectsSpawner : MonoBehaviour
{
    #region Singleton

    public static GlobalEffectsSpawner instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    // Would this be cleaner?
    /*
    public void SpawnEffect(ImpactEffectModifiable impactEffect, Vector3 spawnPos)
    {
        SpawnImpactEffect(impactEffect, spawnPos);
    }
    */

    public float effectLifetime = 0.3f;

    public void SpawnMuzzleEffect(MuzzleEffectModifiable muzzleEffect, Vector3 spawnPos, Transform parent, Transform orientation)
    {
        GameObject muzzleEffectPref = GameAssets.instance.GetMuzzleEffectPrefab(muzzleEffect.muzzleEffectType, muzzleEffect.effectVariation);
        GameObject muzzleEffectObj = Instantiate(muzzleEffectPref, parent);
        muzzleEffectObj.transform.position = orientation.position;
        muzzleEffectObj.transform.forward = orientation.forward;
        for (int i = 0; i < muzzleEffectObj.transform.childCount; i++)
            muzzleEffectObj.transform.GetChild(i).localScale *= muzzleEffect.size;
        StartCoroutine(DestoryGameObjectAfterTime(muzzleEffectObj, effectLifetime));
    }

    public void SpawnImpactEffect(ImpactEffectModifiable impactEffect, Vector3 spawnPos) 
    {
        GameObject impactEffectPref = GameAssets.instance.GetImpactEffectPrefab(impactEffect.impactEffectType, impactEffect.effectVariation);
        GameObject impactEffectObj = Instantiate(impactEffectPref, spawnPos, Quaternion.identity);
        for (int i = 0; i < impactEffectObj.transform.childCount; i++)
            impactEffectObj.transform.GetChild(i).localScale *= impactEffect.size;
        StartCoroutine(DestoryGameObjectAfterTime(impactEffectObj, effectLifetime));
    }

    private IEnumerator DestoryGameObjectAfterTime(GameObject gameObject, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
