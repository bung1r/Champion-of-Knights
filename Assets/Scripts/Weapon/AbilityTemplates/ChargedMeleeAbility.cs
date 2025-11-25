using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Abilities/Charged Melee")]
[Serializable]
public class ChargedMeleeAbility : ChargedAbilityBase
{
    public MeleeHitboxData hitboxData = new MeleeHitboxData();
    public GameObject owner;
}


