using System;
using UnityEngine;

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