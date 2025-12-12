using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class EnemyMeleeRuntime : MeleeRuntime, EnemyAbilityI
{
    public EnemyAbilityData enemyAbilityData {get; set;}
    public void ConstructEnemyMelee(EnemyAbility other)
    {
        enemyAbilityData = new EnemyAbilityData(other.abilityData);
    }

    public EnemyMeleeRuntime(EnemyAbility other, StatManager statManager) 
    {
        ConstructBase(other.ability, statManager);
        ConstructMelee((MeleeAbility)other.ability, statManager);
        ConstructEnemyMelee(other);
    }
}