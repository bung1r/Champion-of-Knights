using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class EnemyStatManager : StatManager
{
    public EnemyStats stats = new EnemyStats();
    protected override void PreStart()
    {
        setEnemyStats(stats);
    }
}
