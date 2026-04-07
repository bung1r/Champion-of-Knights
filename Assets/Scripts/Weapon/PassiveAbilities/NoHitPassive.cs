using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New No Hit Passive", menuName = "Passive Abilities/No Hit")]
public class NoHitPassive : PassiveAbilityBase
{
    public float maxDamageGain = 1.2f; 
    public float timeToReachMax = 30f;

    public override void OnUpdate()
    {
        base.OnUpdate();
    }
    public override void OnTakeDamage(float damageTaken)
    {
        base.OnTakeDamage(damageTaken);
    }
}
public class NoHitRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public NoHitPassive passiveBase;
    private float lastUpdate = -999f;
    private float lastTakenDamage = 0f;
    public override void Init(GameObject owner)
    {
        lastTakenDamage = Time.time;
    }   
    public override void OnTakeDamage(float damageTaken)
    {
        if (damageTaken > 0) lastTakenDamage = Time.time;
    }
    public override void OnUpdate()
    {
        if (Time.time - lastUpdate < 0.2f) return; // Only update every 0.2 seconds to save on performance
        lastUpdate = Time.time;
        float timeSinceDamage = Time.time - lastTakenDamage;
        float damageGain = Mathf.Clamp((timeSinceDamage / passiveBase.timeToReachMax) * passiveBase.maxDamageGain, 0, passiveBase.maxDamageGain);
        statManager.AddMultiplier(new DamageMultiplier
        {
            amount = damageGain,
            lifeTime = Mathf.Infinity,
            type = DamageMultiplierTypes.Additive,
            timeCreated = Time.time,
            source = "NoHitPassive"
        }, "set");
    }

    public NoHitRuntime(){}
    public NoHitRuntime(NoHitPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}