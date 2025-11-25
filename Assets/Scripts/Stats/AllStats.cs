using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Stats : BaseStats
{
    public float currentHP = 100f;
    public float currentStamina = 100f;
    // AKA if you use a 50 stamina move with 45 stamina, you are able to. 
    public List<DamageMultiplier> damageMultipliers = new List<DamageMultiplier>();

    public Stats() {}
    public Stats(Stats other)
    {
        Construct(other);
        currentHP = other.maxHP;
        currentStamina = other.maxStamina;
    }
}
[Serializable]
public class BaseStats
{
    public float maxHP = 100f;
    public float turnSpeed = 100f;
    public float walkSpeed = 10f;
    public bool canSprint = true;
    public float sprintSpeed = 15f;
    public float sprintStaminaCost = 8f;
    public float maxStamina = 100f;
    public float staminaRegen = 10f;
    public float startStaminaRegen = 1f;
    public float startStaminaRegenFromZero = 2.5f;
    // AKA if you use a 50 stamina move with 45 stamina, you are able to. 
    public float overflowStaminaThreshold = 5f;
    public float baseEXP = 100f; // EXP on kill!
    public Resistances resistances = new Resistances();
    public void Construct(Stats other)
    {
        maxHP = other.maxHP;
        walkSpeed = other.walkSpeed;
        turnSpeed = other.turnSpeed;
        canSprint = other.canSprint;
        sprintSpeed = other.sprintSpeed;
        sprintStaminaCost = other.sprintStaminaCost;
        maxStamina = other.maxStamina;
        staminaRegen = other.staminaRegen;
        startStaminaRegen = other.startStaminaRegen;
        startStaminaRegenFromZero = other.startStaminaRegenFromZero;
        overflowStaminaThreshold = other.overflowStaminaThreshold;
        baseEXP = other.baseEXP;
        resistances = new Resistances(other.resistances);
    }
    public BaseStats() {}
    public BaseStats(Stats other)
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

