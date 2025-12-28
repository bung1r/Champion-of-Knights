using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

[CreateAssetMenu(menuName = "Style/Style Bonus Database")]
[Serializable]
public class StyleBonusDatabase : ScriptableObject
{
    public List<StyleBonus> bonuses = new List<StyleBonus>();

    private Dictionary<StyleBonusTypes, StyleBonus> lookup = new Dictionary<StyleBonusTypes, StyleBonus>();

    void OnEnable()
    {
        lookup = bonuses.ToDictionary(b => b.type);
    }

    public StyleBonus Get(StyleBonusTypes type)
        => lookup[type];
}