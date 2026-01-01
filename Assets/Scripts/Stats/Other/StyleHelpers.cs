using System;
using UnityEngine;

[Serializable]
public class StyleBonus
{
    public StyleBonusTypes type;
    public float style;
    public string name;
    public bool canStack = false;
}

[Serializable]
public enum StyleGrades
{
    F, D, C, B, A, S, X, P
}

public enum StyleBonusTypes
{
    Parry, Dodge, DuoParry, TriParry, MegaParry, Ricoshot, Multihit, Multikill, Kill, NaturalCauses
}
