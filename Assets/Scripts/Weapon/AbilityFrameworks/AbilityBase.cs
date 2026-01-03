using System;
using System.Collections;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public abstract class AbilityBase : ScriptableObject
{
    public string abilityName = "Default";
    public float staminaCost = 20f;
    public float baseCooldown = 10f;
    public float critRate = 0f;
    public float critMultiplier = 2f;
    public float variation = 0.2f;
    public float hitboxTimeDelay = 0.2f;
    public float attackLength = 1f;
    public float knockback = 0f;
    public float stunTime = 0.2f;
    public float forward = 0f; // boosts forward movement when using ability 
    public float spin = 0f; // spins when using ability (melee only prob)
    public int attackID = -1;
    [NonSerialized]public float lastUsedTime = 0f;
    public bool useOverflowStamina = true;
    public DamageData damageData = new DamageData();
    [NonSerialized] public StatManager statManager;
    public virtual AbilityRuntime CreateRuntimeInstance(AbilityBase other, StatManager manager)
    {
        if (other is MeleeAbility)
        {
            return new MeleeRuntime((MeleeAbility)other, manager);
        } else if (other is ChargedMeleeAbility)
        {
            return new ChargedMeleeRuntime((ChargedMeleeAbility)other, manager);
        } else if (other is RangedAbility)
        {
            return new RangedRuntime((RangedAbility)other, manager);
        } else if (other is ChargedRangedAbility)
        {
            return new ChargedRangedRuntime((ChargedRangedAbility)other, manager);
        } else if (other is GuardAbility)
        {
            return new GuardRuntime((GuardAbility)other, manager);
        } else if (other is ComplexAbility)
        {
            return new ComplexAbilityRuntime((ComplexAbility)other, manager);
        } else
        {
            Debug.Log("You have not implemented the ability correctly!");
            return new AbilityRuntime(other, manager);
        }
        
    }
    
}

public class AbilityRuntime : IAbility
{
    public string abilityName = "Default";
    public float staminaCost = 20f;
    public float baseCooldown = 10f;
    public float variation = 0.2f;
    public float lastUsedTime = -999f;
    public bool useOverflowStamina = true;
    public float critRate = 0f;
    public float critMultiplier = 2f;
    public float hitboxTimeDelay = 0.2f;
    public float attackLength = 1f;
    public float knockback = 0f;
    public float stunTime = 0.2f;
    public float spin = 0f;
    public float forward = 0f;
    public int attackID = -1;
    public DamageData damageData = new DamageData();
    public StatManager statManager;
    public AbilityBase abilityBase;
    public void ConstructBase(AbilityBase other, StatManager manager)
    {
        abilityName = other.abilityName;
        staminaCost = other.staminaCost;
        baseCooldown = other.baseCooldown;
        variation = other.variation;
        useOverflowStamina = other.useOverflowStamina;
        damageData = new DamageData(other);    
        statManager = manager;
        abilityBase = other;
        critRate = other.critRate;
        critMultiplier = other.critMultiplier;
        hitboxTimeDelay = other.hitboxTimeDelay;
        attackLength = other.attackLength;
        knockback = other.knockback;
        stunTime = other.stunTime;
        forward = other.forward;
        spin = other.spin;
        attackID = other.attackID;
    }
    public AbilityRuntime() {}  
    public AbilityRuntime(AbilityBase other, StatManager manager)
    {
        ConstructBase(other, manager);
    }


    public virtual bool CanUse()
    {
        if (Time.time - lastUsedTime > baseCooldown && statManager.CanUseStamina(staminaCost, useOverflowStamina) && !statManager.GetInAttack())
        {
            return true;
        }
        return false;
    }
    public virtual bool Use()
    {
        if (!CanUse()) return false;
        lastUsedTime = Time.time;
        statManager.UseStamina(staminaCost);
        statManager.BeginAttack(abilityBase);
        Perform();
        BasicEndUse();
        return true;
    }
    public virtual void BeginUse() {} // this is for a charge (hold down)
    public virtual void WhileUse() {} // this is for anything while a charged move is happening
    public virtual void EndUse() {} 
    public virtual float GetCooldownRemaining()
    {
        return Mathf.Max(0, baseCooldown - (Time.time - lastUsedTime));
    }
    async public virtual void BasicEndUse()
    {
        await Task.Delay((int)(attackLength * 1000));
        statManager.EndAttack();
    }
    public virtual void Perform() {Debug.Log("Does this work? I hope not!");}

    // enemy only things. it's inefficient, but the only thing I can think of. 
}

public interface IAbility 
{
    bool CanUse();
    bool Use(); // instant use 
    void BeginUse(); // this is for a charge (hold down)
    void EndUse(); // this is for ending a charged move
    float GetCooldownRemaining();
}