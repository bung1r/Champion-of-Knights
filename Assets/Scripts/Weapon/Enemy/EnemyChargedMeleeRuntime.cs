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
public class EnemyChargedMeleeRuntime : ChargedMeleeRuntime, EnemyAbilityI
{
    public EnemyAbilityData enemyAbilityData {get;set;}
    public void ConstructEnemy(EnemyAbility other)
    {
        enemyAbilityData = new EnemyAbilityData(other.abilityData);
    }

    public EnemyChargedMeleeRuntime(EnemyAbility other, StatManager statManager) 
    {
        ConstructBase(other.ability, statManager);
        ConstructChargedMelee((ChargedMeleeAbility)other.ability, statManager);
        ConstructEnemy(other);
    }
}
