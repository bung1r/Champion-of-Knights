using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;


[Serializable]
[CreateAssetMenu(menuName = "Enemy/Ability")]
public class EnemyAbility : ScriptableObject
{
    public AbilityBase ability;
    public EnemyAbilityData abilityData;
    public virtual AbilityRuntime CreateRuntimeInstance(EnemyAbility other, EnemyStatManager manager)
    {
        if (other.ability is MeleeAbility)
        {
            return new EnemyMeleeRuntime(other, manager);
        } else if (other.ability is ChargedMeleeAbility)
        {
            return new EnemyChargedMeleeRuntime(other, manager);
        } else if (other.ability is RangedAbility)
        {
            return new EnemyRangedRuntime(other, manager);
        } else if (other.ability is ChargedRangedAbility)
        {
            return new EnemyChargedRangedRuntime(other, manager);
        }
        return new EnemyMeleeRuntime(other, manager);
    }
}




public interface EnemyAbilityI
{
    EnemyAbilityData enemyAbilityData {set; get;}
}

[Serializable]
public class EnemyAbilityData
{
    public float currentPriority = 0;
    public float basePriority = 10;
    public float minAttackRange = 0;
    public float maxAttackRange = 10;
    public float priorityAtMin = 0;
    public float priorityAtMax = 0;
    public bool linearInterpolation = false;
    public EnemyAbilityData(EnemyAbilityData other)
    {
        basePriority = other.basePriority;
        minAttackRange = other.minAttackRange;
        maxAttackRange = other.maxAttackRange;
        priorityAtMin = other.priorityAtMin;
        priorityAtMax = other.priorityAtMax;
        linearInterpolation = other.linearInterpolation;
    }
}