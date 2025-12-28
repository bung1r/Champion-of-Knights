using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatManager : StatManager
{
    public EnemyStatsSO statsSO;
    
    public EnemyStats stats;
    void Awake()
    {
        stats = statsSO.CreateRuntime();
        setEnemyStats(stats);
    }

}
