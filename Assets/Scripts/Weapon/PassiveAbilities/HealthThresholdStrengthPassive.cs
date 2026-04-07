using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Health Threshold Strength Passive", menuName = "Passive Abilities/Health Threshold Strength")]
public class HealthThresholdStrengthPassive : PassiveAbilityBase
{
    public float maxDamageGain = 1f;
    public float timeToReachMax = 30f;
    public float aboveThisHPRatio = 0.7f;

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
public class HealthThresholdStrengthRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public HealthThresholdStrengthPassive passiveBase;
    private float lastUpdate = -999f;
    private float lastWentBelowThreshold = 0f;
    public override void Init(GameObject owner)
    {
        lastWentBelowThreshold = Time.time;
    }   
    public override void OnUpdate()
    {
        if (Time.time - lastUpdate < 0.2f) return; // Only update every 0.2 seconds to save on performance
        if (statManager.GetStats().currentHP < statManager.GetStats().maxHP * passiveBase.aboveThisHPRatio)
        {
            lastWentBelowThreshold = Time.time;
        }
        
        lastUpdate = Time.time;
        float timeSinceDamage = Time.time - lastWentBelowThreshold;
        float damageGain = Mathf.Clamp((timeSinceDamage / passiveBase.timeToReachMax) * passiveBase.maxDamageGain, 0, passiveBase.maxDamageGain);
        statManager.AddMultiplier(new DamageMultiplier
        {
            amount = damageGain,
            lifeTime = Mathf.Infinity,
            type = DamageMultiplierTypes.Additive,
            timeCreated = Time.time,
            source = "HealthThresholdStrengthPassive"
        }, "set");
    }

    public HealthThresholdStrengthRuntime(){}
    public HealthThresholdStrengthRuntime(HealthThresholdStrengthPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}