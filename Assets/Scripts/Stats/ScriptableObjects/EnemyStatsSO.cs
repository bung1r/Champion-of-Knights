using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Enemy/Enemy Stats")]
[Serializable]
public class EnemyStatsSO : ScriptableObject
{
    public EnemyStats baseStats;
    
    public EnemyStats CreateRuntime()
    {
        return new EnemyStats(baseStats);
    }
}