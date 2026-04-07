using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Item Strength Passive", menuName = "Passive Abilities/Item Strength")]
public class ItemStrengthPassive : PassiveAbilityBase
{
    public float itemDamageMultiplier = 0.5f;
    public override void OnUseItem()
    {
        base.OnUseItem();
    }
}
public class ItemStrengthRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public ItemStrengthPassive passiveBase;
    public override void OnUseItem()
    {
        statManager.AddMultiplier(new DamageMultiplier
            {
                amount = passiveBase.itemDamageMultiplier,
                lifeTime = 4f, // Lasts for 4 seconds after using an item
                type = DamageMultiplierTypes.Additive,
                timeCreated = Time.time,
                source = "ItemStrengthPassive"
            });
    }    
    public ItemStrengthRuntime(){}
    public ItemStrengthRuntime(ItemStrengthPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}