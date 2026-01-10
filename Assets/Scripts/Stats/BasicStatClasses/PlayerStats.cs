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
    public float viewers = 0f;
    public float loyalViewers = 0f;
    public float money = 0f; 
    public float reputation = 0f; // -100 -> 100
    public float corruption = 0f; // 0 -> 100
    public float sponsers = 0f; // as many as you want
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

