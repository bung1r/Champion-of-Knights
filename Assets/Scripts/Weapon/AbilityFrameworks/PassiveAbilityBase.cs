using System;
using UnityEngine;

[Serializable]
public class PassiveAbilityBase : ScriptableObject
{
    public virtual void Init(GameObject owner)
    {
        
    }
    public virtual void OnHit(float damageDealt)
    {
        
    }
    public virtual void OnKill()
    {
        
    }
    public virtual void OnTakeDamage(float damageTaken)
    {
        
    }
    public virtual void OnDeath()
    {
        
    }
    public virtual void OnParry()
    {
        
    }
    public virtual void OnUseItem()
    {
        
    }
    public virtual void OnUpdate()
    {
        
    }
    public virtual PassiveAbilityRuntime CreateRuntimeInstance(PassiveAbilityBase other, StatManager manager)
    {
        if (this is LifeStealPassive)
        {
            return new LifeStealRuntime((LifeStealPassive)other, manager);
        } else if (this is StaminaStealPassive)
        {
            return new StaminaStealRuntime((StaminaStealPassive)other, manager);
        } else if (this is ViewershipStrengthPassive)
        {
            return new ViewershipStrengthRuntime((ViewershipStrengthPassive)other, manager);
        } else if (this is SponsorStrengthPassive)
        {
            return new SponsorStrengthRuntime((SponsorStrengthPassive)other, manager);
        } else if (this is StyleStrengthPassive)
        {
            return new StyleStrengthRuntime((StyleStrengthPassive)other, manager);
        } else if (this is ItemStrengthPassive)
        {
            return new ItemStrengthRuntime((ItemStrengthPassive)other, manager);
        } else if (this is ItemWeaknessPassive)
        {
            return new ItemWeaknessRuntime((ItemWeaknessPassive)other, manager);
        } else if (this is NoHitPassive)
        {
            return new NoHitRuntime((NoHitPassive)other, manager);
        } else if (this is HealthThresholdStrengthPassive)
        {
            return new HealthThresholdStrengthRuntime((HealthThresholdStrengthPassive)other, manager);
         } else if (this is ParryStrengthPassive)
        {
            return new ParryStrengthRuntime((ParryStrengthPassive)other, manager);
         } else if (this is InverseCorruptionPassive)
        {
            return new InverseCorruptionRuntime((InverseCorruptionPassive)other, manager);
        }
        else
        {
            Debug.Log("You have not implemented the passive ability correctly!");
        }
        return new PassiveAbilityRuntime();
    }
}

public class PassiveAbilityRuntime
{
    public virtual void Init(GameObject owner)
    {
        
    }
    public virtual void OnHit(float damageDealt)
    {
        
    }
    public virtual void OnKill()
    {
        
    }
    public virtual void OnTakeDamage(float damageTaken)
    {
        
    }
    public virtual void OnDeath()
    {
        
    }
    public virtual void OnParry()
    {
        
    }
    public virtual void OnUseItem()
    {
        
    }
    public virtual void OnUpdate()
    {
        
    }
}