using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

[Serializable]
public class PlayerStats : Stats
{
    public List<StatModifier> statModifier = new List<StatModifier>();
    public float totalEXP;
    public float level;
    public float currentEXP;
    public float nextLevelEXP;
    public float skillPoints;
    public float totalStyle;
    public float currentStyle;
    public float maxStyle;
    public float styleLevel = 0;
    
    public void ConstructPlayer(PlayerStats other)
    {
        totalEXP = other.totalEXP;
        skillPoints = other.skillPoints;
        currentHP = other.maxHP;
        currentStamina = other.maxStamina;
    }
    public PlayerStats() {}
    public PlayerStats(PlayerStats other)
    {
        Construct(other);
        ConstructPlayer(other);
    }

}

