using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class GuardStats
{
    public float parryThreshold = 0.1f;
    public DamageType resistance = DamageType.All; 
    public float resistAmount = 2f; // divide damage by 2
    public float speedReduction = 2f; // divide speed by 2
    public float parryStart = 0f; // x seconds after guard is valid parrying
    public float parryEnd = 0.2f; // x seconds after guard parry time ends.
    public GuardStats() {}
    public GuardStats(GuardStats other)
    {
        parryThreshold = other.parryThreshold;
        resistance = other.resistance;
        resistAmount = other.resistAmount;
        speedReduction = other.speedReduction;
        parryStart = other.parryStart;
        parryEnd = other.parryEnd;
    }
}