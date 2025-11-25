using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class PlayerStats : Stats
{
    public float totalEXP;
    public float level;
    public float currentEXP;
    public float nextLevelEXP;
    public PlayerStats() {}
    public PlayerStats(PlayerStats other)
    {
        Construct(other);
    }

}

