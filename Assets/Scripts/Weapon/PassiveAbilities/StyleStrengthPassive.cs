using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Style Strength Passive", menuName = "Passive Abilities/Style Strength")]
public class StyleStrengthPassive : PassiveAbilityBase
{
    public float styleStrengthPow = 0.05f;
    public float styleStrengthMax = 2f;
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
public class StyleStrengthRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public StyleStrengthPassive passiveBase;
    private float lastUpdate = -999f;
    public override void OnUpdate()
    {
        if (Time.time - lastUpdate < 0.2f) return; // Only update every 0.2 seconds to save on performance
        lastUpdate = Time.time;
        if (statManager is PlayerStatManager playerStats)
        {
            float style = playerStats.stats.totalStyle;
            float strengthGain = Mathf.Pow(style, passiveBase.styleStrengthPow) - 1;
            strengthGain = Mathf.Clamp(strengthGain, 0, passiveBase.styleStrengthMax);
            statManager.AddMultiplier(new DamageMultiplier
            {
                amount = strengthGain,
                lifeTime = Mathf.Infinity,
                type = DamageMultiplierTypes.Additive,
                timeCreated = Time.time,
                source = "StyleStrengthPassive"
            }, "set");
        }

        passiveBase.OnUpdate();
    }
    public StyleStrengthRuntime(){}
    public StyleStrengthRuntime(StyleStrengthPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}