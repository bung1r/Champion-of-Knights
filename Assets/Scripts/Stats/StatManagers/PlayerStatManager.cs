using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

[Serializable]
public class PlayerStatManager : StatManager
{
    public PlayerStatsSO statsSO;
    
    public PlayerStats stats;
    private float lastHP = 0f;
    private float lastStam = 0f;
    private StatsUIManager statsUIManager; 
    void Awake()
    {
        stats = statsSO.CreateRuntime();
        setPlayerStats(stats);
        AdjustLevels();
    }   
    public void AssignUIManager(StatsUIManager uiManager)
    {
        statsUIManager = uiManager;
    }
    public void AddEXP(float exp)
    {
        stats.totalEXP += exp;
        if (stats.nextLevelEXP - exp <= 0)
        {
            AdjustLevels();
        } else
        {
            stats.nextLevelEXP -= exp;
            stats.currentEXP += exp;
        }
    }
    private float GetLevel(float currentLevel)
    {
        float temp = currentLevel;
        while (LevelToEXP(temp + 1) <= stats.totalEXP)
        {
            temp += 1;
            stats.skillPoints += 1;
        } 
        return temp;
    }
    private float LevelToEXP(float level)
    {   
        return 100 * (level * (level + 1) / 2);
    }
    public void AdjustLevels()
    {
        stats.level = GetLevel(stats.level);
        stats.nextLevelEXP = LevelToEXP(stats.level + 1) - stats.totalEXP;
        stats.currentEXP =  stats.totalEXP - LevelToEXP(stats.level);
    }
    public float GetSkillPoints() => stats.skillPoints;
    public void UIHandler()
    {
        if (statsUIManager == null) return;
        if (lastHP != stats.currentHP)
        {
            statsUIManager.UpdateHP(stats.currentHP, stats.maxHP);
            lastHP = stats.currentHP;
        }
        if (lastStam != stats.currentStamina)
        {
            statsUIManager.UpdateStam(stats.currentStamina, stats.maxStamina);
            lastStam = stats.currentStamina;
        }
    }
    public override void PostUpdate()
    {
        UIHandler();
    }


    public void OnParry()
    {
        statsUIManager.AddStyleEntry(StyleBonusTypes.Parry);
    }
    public void OnKill()
    {
        statsUIManager.AddStyleEntry(StyleBonusTypes.Multikill);
    }

}

