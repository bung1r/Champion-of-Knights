using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Life Steal Passive", menuName = "Passive Abilities/Life Steal")]
public class LifeStealPassive : PassiveAbilityBase
{
    public float healOnHitMultiplier = 0.02f;
    public float healOnHitMin = 0.2f;
    public float healOnHitMax = 2f;
    public float healOnKillAmount = 4f;
    public override void Init(GameObject owner)
    {
        
    }
    public override void OnHit(float damageDealt)
    {
        
    }
    public override void OnKill()
    {
        
    }
}

public class LifeStealRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public LifeStealPassive passiveBase;
    public LifeStealRuntime(){}
    public override void Init(GameObject owner)
    {

        base.Init(owner);
    }
    public override void OnHit(float damageDealt)
    {
        if (damageDealt > 0)
        {
            float healAmount = Mathf.Clamp(damageDealt * passiveBase.healOnHitMultiplier, passiveBase.healOnHitMin, passiveBase.healOnHitMax); // Heal for 2% of damage dealt
            statManager.TakeDamage(new DamageData {baseDamage = -healAmount, type = DamageType.Fixed, source = null}, true);
            // Debug.Log($"Life Steal: Healed for {healAmount} HP!");
        }
        base.OnHit(damageDealt);
    }
    public override void OnKill()
    {
        float healAmount = passiveBase.healOnKillAmount; // Heal for a flat amount on kill
        statManager.TakeDamage(new DamageData {baseDamage = -healAmount, type = DamageType.Fixed, source = null}, true);
        base.OnKill();
    }
    public LifeStealRuntime(LifeStealPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}