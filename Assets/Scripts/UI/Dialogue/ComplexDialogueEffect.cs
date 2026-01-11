using System;
using UnityEngine;

[Serializable]
public class ComplexDialogueEffect
{
    public DialogueEffectTypes effectType;
    public int intValue; // optional
    public float floatValue; // optional
    public string stringValue; // optional
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
}