using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions.Must;

[Serializable]
[CreateAssetMenu(fileName = "New Stand Still Passive", menuName = "Passive Abilities/Stand Still Buff")]
public class StandStillPassive : PassiveAbilityBase
{
    public float standStillTime = 1f;
    public float regenHPBuff = 1f;
    public float regenStamBuff = 2f;
    public override void OnUpdate()
    {
        base.OnUpdate();
    }
}
public class StandStillRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public StandStillPassive passiveBase;
    private float lastUpdate = -999f;
    private float lastMoved = 0f;
    private Rigidbody rb;
    public override void Init(GameObject owner)
    {
        lastMoved = Time.time;
        rb = statManager.GetComponent<Rigidbody>();
    }   
    public override void OnUpdate()
    {
        if (rb == null) { rb = statManager.GetComponent<Rigidbody>();return;}
        if ((rb.velocity.x > -0.01f && rb.velocity.x < 0.01f)&&(rb.velocity.y > -0.01f && rb.velocity.y < 0.01f)&&(rb.velocity.z > -0.01f && rb.velocity.z < 0.01f))
        {

        } else
        {
            lastMoved = Time.time;
        }

        if (Time.time - lastUpdate < 0.2f) return; // Only update every 0.2 seconds to save on performance
        lastUpdate = Time.time;
        
        Dictionary<BaseStatsEnum, float> thisStatDict = new Dictionary<BaseStatsEnum, float>
        {
            {BaseStatsEnum.regenHP, passiveBase.regenHPBuff},
            {BaseStatsEnum.staminaRegen, passiveBase.regenStamBuff}
        };

        if (Time.time - lastMoved > passiveBase.standStillTime)
        {
            statManager.AddStatModifier(new StatModifier{
                duration = 0.2f, 
                timeCreated = Time.time, 
                statModifierType = StatModifierType.TempBuff, 
                generalModifierType = ModifierTypes.Additive,
                statDict = thisStatDict
            });

        }
    }

    public StandStillRuntime(){}
    public StandStillRuntime(StandStillPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}