using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "Abilities/Melee")]
[Serializable]
public class MeleeAbility : AbilityBase
{
    public MeleeHitboxData hitboxData = new MeleeHitboxData();
    public GameObject owner;

}
