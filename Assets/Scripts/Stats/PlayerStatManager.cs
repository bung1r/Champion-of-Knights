using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;


public class PlayerStatManager : StatManager
{
    public PlayerStats stats = new PlayerStats();
    protected override void PreStart()
    {

        setPlayerStats(stats);
    }
}

