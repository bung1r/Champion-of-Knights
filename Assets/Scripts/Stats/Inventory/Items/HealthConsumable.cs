using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/HealthPotion")]
[Serializable]
public class HealthPotion : Item
{
    public float healAmount = 30f;
    public override void Perform(StatManager statManager)
    {
        Debug.Log("those who perform");
        statManager.TakeDamage(new DamageData{
            baseDamage = -healAmount,
            type = DamageType.Fixed,
            source = null
        });
    }
}