using System;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public abstract class AbilityBase : ScriptableObject
{
    [SerializeField]public float staminaCost = 20f;
    [SerializeField]public float baseCooldown = 10f;
    [SerializeField]public float variation = 0.2f;
    [NonSerialized]public float lastUsedTime = 0f;
    [SerializeField]public bool useOverflowStamina = true;
    [SerializeField]public DamageData damageData = new DamageData();
    [SerializeField]public StatManager statManager;
    public virtual AbilityRuntime CreateRuntimeInstance(AbilityBase other, StatManager manager)
    {
        if (other is MeleeAbility)
        {
            return new MeleeRuntime((MeleeAbility)other, manager);
        }
        return new AbilityRuntime(other, manager);
    }
    
}
public class AbilityRuntime : IAbility
{
    public float staminaCost = 20f;
    public float baseCooldown = 10f;
    public float variation = 0.2f;
    public float lastUsedTime = -999f;
    public bool useOverflowStamina = true;
    public DamageData damageData = new DamageData();
    public StatManager statManager;
    public void ConstructBase(AbilityBase other, StatManager manager)
    {
        staminaCost = other.staminaCost;
        baseCooldown = other.baseCooldown;
        variation = other.variation;
        useOverflowStamina = other.useOverflowStamina;
        damageData = new DamageData(other.damageData);    
        statManager = manager;
    }
    public AbilityRuntime() {}  
    public AbilityRuntime(AbilityBase other, StatManager manager)
    {
        ConstructBase(other, manager);
    }


    public virtual bool CanUse()
    {
        if (Time.time - lastUsedTime > baseCooldown && statManager.CanUseStamina(staminaCost, useOverflowStamina))
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
        Perform();
        return true;
    }
    public virtual void BeginUse() {} // this is for a charge (hold down)
    public virtual void EndUse() {} // this is for ending a charged move
    public virtual float GetCooldownRemaining()
    {
        return Mathf.Max(0, baseCooldown - (Time.time - lastUsedTime));
    }
    
    public virtual void Perform() {Debug.Log("Does this work? I hope not!");}
}
public interface IAbility 
{
    bool CanUse();
    bool Use(); // instant use 
    void BeginUse(); // this is for a charge (hold down)
    void EndUse(); // this is for ending a charged move
    float GetCooldownRemaining();
}