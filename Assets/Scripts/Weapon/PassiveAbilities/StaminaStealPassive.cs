using System;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Stamina Steal Passive", menuName = "Passive Abilities/Stamina Steal")]
public class StaminaStealPassive : PassiveAbilityBase
{
    public float staminaOnHitMultiplier = 0.02f;
    public float staminaOnHitMin = 0.2f;
    public float staminaOnHitMax = 2f;
    public float staminaOnKillAmount = 4f;
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

public class StaminaStealRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public StaminaStealPassive passiveBase;
    public StaminaStealRuntime(){}
    public override void Init(GameObject owner)
    {

        base.Init(owner);
    }
    public override void OnHit(float damageDealt)
    {
        if (damageDealt > 0)
        {
            float staminaAmount = Mathf.Clamp(damageDealt * passiveBase.staminaOnHitMultiplier, passiveBase.staminaOnHitMin, passiveBase.staminaOnHitMax); // Steal for 2% of damage dealt
            statManager.UseStamina(-staminaAmount);
            // Debug.Log($"Stamina Steal: Gained {staminaAmount} Stamina!");
        }
        base.OnHit(damageDealt);
    }
    public override void OnKill()
    {
        float staminaAmount = passiveBase.staminaOnKillAmount; // Steal for a flat amount on kill
        statManager.UseStamina(-staminaAmount);
        base.OnKill();
    }
    public StaminaStealRuntime(StaminaStealPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}