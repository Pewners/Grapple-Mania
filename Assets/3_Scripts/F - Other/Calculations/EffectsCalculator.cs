using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this is half-hardcoded for now, I don't really know how much sense it makes to make everything
// customizable (shouldn't levels always have the same effects)

public class EffectsCalculator : MonoBehaviour
{
    public EffectsCalculator instance;

    [Header("Player Levels")]
    public float player_maxHealthPercentagePerLevel;
    public float player_carryStrengthPercentagePerLevel;

    [Header("Difficulty")]
    public float diff_enemyMaxHealthPercentagePerLevel;

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

    #region Player Levels

    /// maxHealth increase
    /// carryStrength increase
    /*
    public List<PlayerStatEffectOld> GetEffectsByLevel(int level)
    {
        List<PlayerStatEffectOld> statEffects = new List<PlayerStatEffectOld>();

        // 0 - constantVariables
        int tickArg = -1;
        PlayerStatEffectOld.ChangeType changeArg = PlayerStatEffectOld.ChangeType.Percentage;

        // 1 - MaxHealthBoost
        float maxHealthBoostArg = level * player_maxHealthPercentagePerLevel;
        statEffects.Add(new PlayerStatEffectOld(tickArg, RcLab.Stats.ChangeablePlayerStat.maxHealth, maxHealthBoostArg, changeArg));

        // 2 - CarryStrength
        float carryStrengthBoostArg = level * player_carryStrengthPercentagePerLevel;
        statEffects.Add(new PlayerStatEffectOld(tickArg, RcLab.Stats.ChangeablePlayerStat.carryStrength, carryStrengthBoostArg, changeArg));

        return statEffects;
    }
    */

    #endregion

    #region Difficulty

    /// enemy maxHealth increase
    /*
    public List<PlayerStatEffectOld> GetEnemyEffectsByDifficulty(float difficulty)
    {
        List<PlayerStatEffectOld> statEffects = new List<PlayerStatEffectOld>();

        // 0 - constantVariables
        int tickArg = -1;
        PlayerStatEffectOld.ChangeType changeArg = PlayerStatEffectOld.ChangeType.Percentage;

        // 1 - MaxHealthBoost
        float maxHealthBoostArg = difficulty * diff_enemyMaxHealthPercentagePerLevel;
        statEffects.Add(new PlayerStatEffectOld(tickArg, RcLab.Stats.ChangeablePlayerStat.maxHealth, maxHealthBoostArg, changeArg));

        return statEffects;
    }
    */

    #endregion
}
