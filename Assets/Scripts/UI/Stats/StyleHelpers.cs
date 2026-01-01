using System;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;

public struct StyleBonuses
{
    
}

public class StyleEntry
{
    public GameObject styleObj;
    public float timeCreated;
    public StyleBonusTypes bonusType;
    public StyleBonus styleBonus;
    public int mult = 1;
    public StyleEntry(StyleBonusDatabase database, GameObject obj, StyleBonusTypes type, int otherMult)
    {
        styleObj = obj;
        timeCreated = Time.time;
        bonusType = type;
        mult = otherMult;
        styleBonus = database.Get(type);
        if (styleObj.TryGetComponent<TextMeshProUGUI>(out var text))
        {
            text.text = $"+{styleBonus.name}";
            if (mult > 1)
            {
                text.text += $" x{mult}";
            }
        }
    }
}