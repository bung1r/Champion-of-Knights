using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Stats
{
    public float currentHP = 100f;
    public float maxHP = 100f;
    public float walkSpeed = 10f;
    public float turnSpeed = 100f;
    public bool canSprint = true;
    public float sprintSpeed = 15f;
    public float sprintStaminaCost = 8f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float staminaRegen = 10f;
    public float startStaminaRegen = 1f;
    public float startStaminaRegenFromZero = 2.5f;
    // AKA if you use a 50 stamina move with 45 stamina, you are able to. 
    public float overflowStaminaThreshold = 5f;
    public float rotationSpeed = 200f;
    public Resistances resistances = new Resistances();
    public void Construct(Stats other)
    {
        currentHP = other.currentHP;
        maxHP = other.maxHP;
        walkSpeed = other.walkSpeed;
        turnSpeed = other.turnSpeed;
        canSprint = other.canSprint;
        sprintSpeed = other.sprintSpeed;
        sprintStaminaCost = other.sprintStaminaCost;
        currentStamina = other.currentStamina;
        maxStamina = other.maxStamina;
        staminaRegen = other.staminaRegen;
        startStaminaRegen = other.startStaminaRegen;
        startStaminaRegenFromZero = other.startStaminaRegenFromZero;
        overflowStaminaThreshold = other.overflowStaminaThreshold;
        resistances = new Resistances(other.resistances);
    }
    public Stats() {}
    public Stats(Stats other)
    {
        Construct(other);
    }
}

[Serializable]
public class PlayerStats : Stats
{
    public float totalEXP;
    public float level;
    public float currentEXP;
    public float nextLevelEXP;
    public PlayerStats() {}
    public PlayerStats(PlayerStats other)
    {
        Construct(other);
    }
}

[Serializable]
public class EnemyStats : Stats
{

    public float cautionRange = 20f;
    public float aggroRange = 10f;
    public EnemyStats() {}
    public EnemyStats(EnemyStats other)
    {
        Construct(other);
    }
}

