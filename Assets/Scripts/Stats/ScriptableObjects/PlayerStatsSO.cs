using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

[CreateAssetMenu(menuName = "Player/Player Stats")]
[Serializable]
public class PlayerStatsSO : ScriptableObject
{
    public PlayerStats baseStats;
    
    public PlayerStats CreateRuntime()
    {
        return new PlayerStats(baseStats);
    }
}