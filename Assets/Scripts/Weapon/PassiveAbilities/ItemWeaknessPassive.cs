using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Item Weakness Passive", menuName = "Passive Abilities/Item Weakness")]
public class ItemWeaknessPassive : PassiveAbilityBase
{
    public float itemDamageMultiplier = -1f;
    public override void OnUseItem()
    {
        base.OnUseItem();
    }
    public override void Init(GameObject owner)
    {
        base.Init(owner);
    }
}
public class ItemWeaknessRuntime : PassiveAbilityRuntime
{
    public StatManager statManager;
    public ItemWeaknessPassive passiveBase;
    public override void Init(GameObject owner)
    {
        statManager.AddMultiplier(new DamageMultiplier
        {
            amount = -passiveBase.itemDamageMultiplier,
            lifeTime = Mathf.Infinity,
            type = DamageMultiplierTypes.Additive,
            timeCreated = Time.time,
            source = "ItemWeaknessPassive"
        });
    }
    public override void OnUseItem()
    {
        statManager.AddMultiplier(new DamageMultiplier
            {
                amount = passiveBase.itemDamageMultiplier,
                lifeTime = 7f, // Lasts for 7 seconds after using an item
                type = DamageMultiplierTypes.Additive,
                timeCreated = Time.time,
                source = "ItemWeaknessPassiveInf"
            });
    }    
    public ItemWeaknessRuntime(){}
    public ItemWeaknessRuntime(ItemWeaknessPassive passive, StatManager manager)
    {
        statManager = manager;
        passiveBase = passive;
    }
}