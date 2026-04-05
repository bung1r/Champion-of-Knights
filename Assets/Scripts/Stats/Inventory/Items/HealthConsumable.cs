using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/HealthPotion")]
[Serializable]
public class HealthPotion : Item
{
    public float healAmount = 30f;
    public override void Perform(StatManager statManager)
    {
        statManager.TakeDamage(new DamageData{
            baseDamage = -healAmount,
            type = DamageType.Fixed,
            source = null
        });
    }
}