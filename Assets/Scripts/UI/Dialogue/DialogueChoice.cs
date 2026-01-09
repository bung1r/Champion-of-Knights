using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;

[Serializable]
public class DialogueChoice
{
    public string choiceText;
    public DialogueEffects effects;
}

[Serializable]
public class DialogueEffects
{
    public bool hasEffects = false;
    public bool endDialogue = true;
    public FullDialogue nextDialogue; // optional
    public float reputation = 0;
    public float loyalviewers = 0;
    public float sponsers = 0;
    public float money = 0;
    public float corruption = 0;
    public Item givenItem;
}