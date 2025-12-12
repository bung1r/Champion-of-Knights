using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class EnemyRangedRuntime : RangedRuntime, EnemyAbilityI
{
    public EnemyAbilityData enemyAbilityData {get; set;}
    public void ConstructEnemyRanged(EnemyAbility other)
    {
        enemyAbilityData = new EnemyAbilityData(other.abilityData);
    }

    public EnemyRangedRuntime(EnemyAbility other, StatManager statManager) 
    {
        ConstructBase(other.ability, statManager);
        ConstructRanged((RangedAbility)other.ability, statManager);
        ConstructEnemyRanged(other);
    }
}

