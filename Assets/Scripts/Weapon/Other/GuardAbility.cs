using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Abilities/Guard")]
[Serializable]
public class GuardAbility : AbilityBase
{
    
    [HideInInspector] public GameObject owner;
    [Header("Only stats that matter in guards")]
    public GuardStats guardStats;
}
