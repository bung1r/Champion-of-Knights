using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.Search;
using UnityEngine;

[Serializable]
public class PlayerStatManager : StatManager
{
    public PlayerStatsSO statsSO;
    
    public PlayerStats stats;
    void Awake()
    {
        stats = statsSO.CreateRuntime();
        setPlayerStats(stats);
        AdjustLevels();
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
}

