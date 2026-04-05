using System;
using UnityEditor.SceneManagement;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Parry Strength Passive", menuName = "Passive Abilities/Parry Strength")]
public class ParryStrengthPassive : PassiveAbilityBase
{
    public float buffAmtOnParry = 0.2f;
    public float buffDuration = 1.5f;
    public override void OnParry()
    {
        base.OnParry();
     }
}
public class ParryStrengthRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public ParryStrengthPassive passiveBase;
    public override void OnParry()
    {
        statManager.AddMultiplier(new DamageMultiplier
            {
                amount = passiveBase.buffAmtOnParry,
                lifeTime = passiveBase.buffDuration,
                type = DamageMultiplierTypes.Additive,
                timeCreated = Time.time,
                source = "ParryStrengthPassive"
            });
    }
    public ParryStrengthRuntime(){}
    public ParryStrengthRuntime(ParryStrengthPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}