using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Abilities/Complex")]
[Serializable]
public class ComplexAbility : AbilityBase
{
    public List<ComplexAbilityValue> abilityBases = new List<ComplexAbilityValue>();

}


[Serializable]
public class ComplexAbilityValue 
{
    public AbilityBase abilityBase;
    public float delay;
}