using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;


[Serializable]
public class ComplexAbilityRuntime : AbilityRuntime
{
    [Header("Complex Ability Stuff, Other stuff doesn't matter!!")]
    public List<AbilityRuntime> abilityRuntimes = new List<AbilityRuntime>();
    public List<ComplexAbilityValue> abilityBases = new List<ComplexAbilityValue>();
    public ComplexAbilityRuntime(ComplexAbility other, StatManager manager)
    {
        abilityBases = other.abilityBases;
        ConstructBase(other, manager);
        foreach (ComplexAbilityValue abilityValue in other.abilityBases)
        {
            AbilityRuntime runtimeInstance = abilityValue.abilityBase.CreateRuntimeInstance(abilityValue.abilityBase, manager);
            runtimeInstance.hitboxTimeDelay = 0f; // we handle delay ourselves
            abilityRuntimes.Add(runtimeInstance);
        }
    }

    async public override void Perform()
    {
        for (int i = 0; i < abilityRuntimes.Count; i++)
        {
            await System.Threading.Tasks.Task.Delay((int)(abilityBases[i].delay * 1000));
            abilityRuntimes[i].Perform();
        }
    }
}