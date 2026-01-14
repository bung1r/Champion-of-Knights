using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Inventory/Items/StatConsumable")]
[Serializable]

public class StatConsumable : Item
{
    // public StatModifierType statModifierType = StatModifierType.TempBuff;
    // public ModifierTypes generalModifierType = ModifierTypes.Multiplicative;
    // public List<StatModifierList> statModifierList; 
    public StatModifier statModifier;
    public override void Perform(StatManager statManager)
    {
        if (prefab == null) {
            Debug.LogError("No prefab assigned to BombItem!");
            return;
        }
        StatModifier newStatModifier = new StatModifier(statModifier);
        
        statManager.AddStatModifier(newStatModifier);
    }
} 