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
public class EnemyComplexRuntime : ComplexAbilityRuntime, EnemyAbilityI
{
    public EnemyAbilityData enemyAbilityData {get; set;}
    public void ConstructAbilityData(EnemyAbility other)
    {
        enemyAbilityData = new EnemyAbilityData(other.abilityData);
    }
    
    public EnemyComplexRuntime(EnemyAbility other, StatManager statManager) 
    {
        ConstructBase(other.ability, statManager);
        ConstructComplex((ComplexAbility)other.ability, statManager);
        ConstructAbilityData(other);
        
    }
}