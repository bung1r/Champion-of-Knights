using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

[Serializable]
public class Dialogue
{
    public string dialogueLine;
    public List<DialogueChoice> choices; // optional
    public DialogueEffects effects; // optional
}