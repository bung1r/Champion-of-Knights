using System;
using UnityEngine;

[Serializable]
public class ComplexDialogueEffect
{
    public DialogueEffectTypes effectType;
    public int intValue; // optional
    public float floatValue; // optional
    public string stringValue; // optional
    public Item itemValue; // optional  
}

[Serializable]
public enum DialogueEffectTypes
{
    Reputation,
    Corruption,
    Sponsor,
    Money,
    DoEnding,
    LoyalViewers,
    GiveItem,
    MidGameChoice,
    OpenVictoryScreen,
}