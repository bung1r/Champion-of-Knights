using System;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;

public struct StyleBonuses
{
    
}
public enum StyleBonusTypes
{
    Parry, Dodge, DuoParry, TriParry, MegaParry, Ricoshot, Multihit, Multikill
}

public class StyleEntry
{
    public GameObject styleObj;
    public float timeCreated;
    public StyleBonusTypes bonusType;
    public StyleBonus styleBonus;
    public StyleEntry(StyleBonusDatabase database, GameObject obj, StyleBonusTypes type)
    {
        styleObj = obj;
        timeCreated = Time.time;
        bonusType = type;
        styleBonus = database.Get(type);
        if (styleObj.TryGetComponent<TextMeshProUGUI>(out var text))
        {
            Debug.Log("Does this work?");
            text.text = $"+{styleBonus.name}";
        }
    }
}