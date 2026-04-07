using System;
using System.Collections.Generic;
[Serializable]
public class Stats : BaseStats
{
    public List<StatModifier> statModifiers = new List<StatModifier>();
    public float currentHP = 100f;
    public float currentStamina = 100f;
    // The boolean variables that shall change. 
    public bool isRunning = false;
    public bool isWalking = false;
    public bool inAttackAnim = false;
    public bool isGuarding = false;
    public bool isParrying = false;
    public bool isUsingItem = false;
    public int attackID = -1; // -1 means no attack, 0 means primary, etc. etc.
    public float stunTime = 0f; // 0 is not stunned, every x is one more sec of stun 
    public List<DamageMultiplier> damageMultipliers = new List<DamageMultiplier>();
    public List<PassiveAbilityBase> passiveAbilities = new List<PassiveAbilityBase>();
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
    public float regenHP = 0f; // per second
    public float startRegenHP = 2f; // seconds after taking damage
    // AKA if you use a 50 stamina move with 45 stamina, you are able to. 
    public float overflowStaminaThreshold = 5f;
    public float baseEXP = 100f; // EXP on kill!
    public Resistances resistances = new Resistances();
    // the stats i had to add for reasons. Bad code? Yes. Do I have time? Nah...
    // destruction path
    public float parryAdder = 0f;
    public float parryDmgMultiplier = 1f;
    public float popularityMultiplier = 1f; // only for players
    public float knockbackMultiplier = 1f;
    public float stunTimeMultiplier = 1f;
    // popularity path
    public float supplyCrateCooldownReduction = 0f;
    public float styleMultiplier = 1f;
    public float corruptionMultiplier = 1f;
    // honor path
    public float repMultiplier = 1f;
    public float staminaUsageMultiplier = 1f;
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
        regenHP = other.regenHP;
        startRegenHP = other.startRegenHP;
        resistances = new Resistances(other.resistances);
        // the stats i had to add for reasons. Bad code? Yes. Do I have time? Nah...

        parryAdder = other.parryAdder;
        parryDmgMultiplier = other.parryDmgMultiplier;
        popularityMultiplier = other.popularityMultiplier;
        knockbackMultiplier = other.knockbackMultiplier;
        stunTimeMultiplier = other.stunTimeMultiplier;

        supplyCrateCooldownReduction = other.supplyCrateCooldownReduction;
        styleMultiplier = other.styleMultiplier;
        corruptionMultiplier = other.corruptionMultiplier;

        repMultiplier = other.repMultiplier;
        staminaUsageMultiplier = other.staminaUsageMultiplier;
        

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
    public float runAwayDist = 2f;
    public float comfortDist = 3f; // distance away from 'runAway' where you stop moving!
    public float runTowardsDist = 10f;
    public float cautionRange = 30f;
    public float aggroRange = 20f;
    public float timeBetweenMoves = 0.5f;
    public bool isBoss = false;
    public void EnemyConstruct(EnemyStats other)
    {
        runAwayDist = other.runAwayDist;
        runTowardsDist = other.runTowardsDist;
        cautionRange = other.cautionRange;
        aggroRange = other.aggroRange;
        currentHP = other.maxHP;
        currentStamina = other.maxStamina;
        comfortDist = other.comfortDist;
        isBoss = other.isBoss;
    }
    public EnemyStats() {}
    public EnemyStats(EnemyStats other)
    {
        EnemyConstruct(other);
        Construct(other);
    }
}

public enum BaseStatsEnum
{
    maxHP,turnSpeed,walkSpeed,canSprint,sprintSpeed,
    sprintStaminaCost,maxStamina,staminaRegen,startStaminaRegen,
    startStaminaRegenFromZero,overflowStaminaThreshold,baseEXP,
    regenHP,startRegenHP,parryAdder,parryDmgMultiplier,popularityMultiplier,
    knockbackMultiplier,stunTimeMultiplier,supplyCrateCooldownReduction,styleMultiplier,
    corruptionMultiplier,repMultiplier,staminaUsageMultiplier
}