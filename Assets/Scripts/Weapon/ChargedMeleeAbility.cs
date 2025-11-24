using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Abilities/Charged Melee")]
[Serializable]
public class ChargedMeleeAbility : MeleeAbility
{

}

public class ChargedMeleeRuntime : MeleeRuntime
{
    public ChargedMeleeRuntime() {}
    public ChargedMeleeRuntime(MeleeAbility other, StatManager manager)
    {
        ConstructBase(other, manager);
        ConstructMelee(other, manager);
    }

}

