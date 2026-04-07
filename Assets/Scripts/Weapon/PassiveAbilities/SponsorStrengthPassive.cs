using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Sponsor Strength Passive", menuName = "Passive Abilities/Sponsor Strength")]
public class SponsorStrengthPassive : PassiveAbilityBase
{
    public float sponsorStrengthPow = 0.05f;
    public float sponsorStrengthMax = 2f;
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
public class SponsorStrengthRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public SponsorStrengthPassive passiveBase;
    private float lastUpdate = -999f;
    public override void OnUpdate()
    {
        if (Time.time - lastUpdate < 0.2f) return; // Only update every 0.2 seconds to save on performance
        lastUpdate = Time.time;
        if (statManager is PlayerStatManager playerStats)
        {
            float sponsors = playerStats.stats.sponsers;
            float strengthGain = Mathf.Pow(sponsors, passiveBase.sponsorStrengthPow) - 1;
            strengthGain = Mathf.Clamp(strengthGain, 0, passiveBase.sponsorStrengthMax);
            statManager.AddMultiplier(new DamageMultiplier
            {
                amount = strengthGain,
                lifeTime = Mathf.Infinity,
                type = DamageMultiplierTypes.Additive,
                timeCreated = Time.time,
                source = "SponsorStrengthPassive"
            }, "set");
        }

        passiveBase.OnUpdate();
    }
    public SponsorStrengthRuntime(){}
    public SponsorStrengthRuntime(SponsorStrengthPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}