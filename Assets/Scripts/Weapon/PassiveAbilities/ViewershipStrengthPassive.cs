using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Viewership Strength Passive", menuName = "Passive Abilities/Viewership Strength")]
public class ViewershipStrengthPassive : PassiveAbilityBase
{
    public float viewershipStrengthPow = 0.05f;
    public float viewershipStrengthMax = 2f;
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
public class ViewershipStrengthRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public ViewershipStrengthPassive passiveBase;
    private float lastUpdate = -999f;
    public override void OnUpdate()
    {
        if (Time.time - lastUpdate < 0.2f) return; // Only update every 0.2 seconds to save on performance
        lastUpdate = Time.time;
        if (statManager is PlayerStatManager playerStats)
        {
            float viewers = playerStats.stats.viewers;
            float strengthGain = Mathf.Pow(viewers, passiveBase.viewershipStrengthPow) - 1;
            strengthGain = Mathf.Clamp(strengthGain, 0, passiveBase.viewershipStrengthMax);
            statManager.AddMultiplier(new DamageMultiplier
            {
                amount = strengthGain,
                lifeTime = Mathf.Infinity,
                type = DamageMultiplierTypes.Additive,
                timeCreated = Time.time,
                source = "ViewershipStrengthPassive"
            }, "set");
        }

        passiveBase.OnUpdate();
    }
    public ViewershipStrengthRuntime(){}
    public ViewershipStrengthRuntime(ViewershipStrengthPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}