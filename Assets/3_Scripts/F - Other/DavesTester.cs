
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DavesTester : MonoBehaviour
{
    public bool enableTestsAfter1Second;
    public bool enableTestsWithControlT;

    [Header("Combat")]
    public RangedWeapon rangedWeapon;
    public Ability testAbility;

    [Header("Visuals")]
    public List<PostProcessingEffect> postProcessingEffects;
    public List<CameraShakeEffect> cameraShakeEffects;

    // references
    private EffectPlayer effectPlayer;

    private void Start()
    {
        if (enableTestsAfter1Second)
            Invoke(nameof(RunAllTests), 1f);

        effectPlayer = PlayerReferences.instance.effectPlayer;
    }

    private void Update()
    {
        if (enableTestsWithControlT && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
            RunAllTests();
    }

    private void RunAllTests()
    {
        //TestRangedWeapon();
        StartCoroutine(PlayPostProcessingEffects());
    }

    #region Combat Tests

    public void TestRangedWeapon()
    {
        PlayerReferences.instance.combat.PlayRangedWeapon(rangedWeapon);
    }

    private IEnumerator ClickSpamTest()
    {
        PlayerAbilities abilities = PlayerReferences.instance.abilities;
        for (int i = 0; i < 20; i++)
        {
            abilities.PlayAbility(testAbility);
            yield return new WaitForSeconds(0.05f);
            abilities.StopAbility(testAbility.GetInstanceID());
            yield return new WaitForSeconds(0.05f);
        }
    }

    #endregion

    #region PostProcessing Tests

    private IEnumerator PlayPostProcessingEffects()
    {
        for (int i = 0; i < postProcessingEffects.Count; i++)
        {
            effectPlayer.PlayEffect(postProcessingEffects[i]);
            yield return new WaitForSeconds(0.75f);
        }

        for (int i = 0; i < cameraShakeEffects.Count; i++)
        {
            effectPlayer.PlayEffect(cameraShakeEffects[i]);
            yield return new WaitForSeconds(0.75f);
        }
    }

    #endregion
}
