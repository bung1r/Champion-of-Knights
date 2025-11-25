using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Abilities/Charged Ranged")]
[Serializable]
public class ChargedRangedAbility : ChargedAbilityBase
{
    public RangedHitboxData hitboxData = new RangedHitboxData();
    public GameObject owner;
}


