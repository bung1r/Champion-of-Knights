using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Abilities/Ranged")]
[Serializable]
public class RangedAbility : AbilityBase
{
    public RangedHitboxData hitboxData = new RangedHitboxData();
    public GameObject owner;

}
