using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Dialogue/Full Dialogue")]
public class FullDialogue : ScriptableObject
{
    public string speakerName;
    public List<Dialogue> dialogue;
}