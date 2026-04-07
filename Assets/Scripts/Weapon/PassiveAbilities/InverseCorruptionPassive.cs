using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Inverse Corruption Passive", menuName = "Passive Abilities/Inverse Corruption")]
public class InverseCorruptionPassive : PassiveAbilityBase
{
    public float maxStrengthGain = 80f;
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
public class InverseCorruptionRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public InverseCorruptionPassive passiveBase;
    private float lastUpdate = -999f;
    private float lastSavedCorruption = 0f;
    public override void OnUpdate()
    {
        if (statManager is PlayerStatManager playerStats)
        {
            if (Time.time - lastUpdate < 0.2f) return; // Only update every 0.2 seconds to save on performance
            lastUpdate = Time.time;

            if (playerStats.stats.corruption != lastSavedCorruption)
            {
                float corruption = playerStats.stats.corruption;
                float strengthGain = Mathf.Clamp(passiveBase.maxStrengthGain - corruption * (passiveBase.maxStrengthGain / 100f), 0, passiveBase.maxStrengthGain);
                statManager.AddMultiplier(new DamageMultiplier
                {
                    amount = strengthGain,
                    lifeTime = Mathf.Infinity,
                    type = DamageMultiplierTypes.Additive,
                    timeCreated = Time.time,
                    source = "InverseCorruptionPassive"
                }, "set");
            }

            lastSavedCorruption = playerStats.stats.corruption;
        }
        
    }
    public InverseCorruptionRuntime(){}
    public InverseCorruptionRuntime(InverseCorruptionPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}