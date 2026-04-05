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
public class EnemyGuardRuntime : GuardRuntime, EnemyAbilityI
{
    public EnemyAbilityData enemyAbilityData {get; set;}
    public void ConstructEnemyGuard(EnemyAbility other)
    {
        enemyAbilityData = new EnemyAbilityData(other.abilityData);
    }
    public async override void BeginUse()
    {
        if (!CanUse()) return;
        base.BeginUse();
        await Task.Delay((int)(abilityBase.attackLength * 1000));
        EndUse();
    }
    public EnemyGuardRuntime(EnemyAbility other, StatManager statManager) 
    {
        ConstructBase(other.ability, statManager);
        ConstructGuard((GuardAbility)other.ability, statManager);
        ConstructEnemyGuard(other);
    }
}