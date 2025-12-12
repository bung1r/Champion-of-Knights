using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;


[Serializable]
public class EnemyChargedRangedRuntime : ChargedRangedRuntime, EnemyAbilityI
{
    public EnemyAbilityData enemyAbilityData {get; set;}
    public void ConstructEnemy(EnemyAbility other)
    {
        enemyAbilityData = new EnemyAbilityData(other.abilityData);
    }

    public EnemyChargedRangedRuntime(EnemyAbility other, StatManager statManager) 
    {
        ConstructBase(other.ability, statManager);
        ConstructChargedMelee((ChargedRangedAbility)other.ability, statManager);
        ConstructEnemy(other);
    }
}

