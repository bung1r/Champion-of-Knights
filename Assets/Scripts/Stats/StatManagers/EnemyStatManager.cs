using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyStatManager : StatManager
{
    public EnemyStatsSO statsSO;
    
    public EnemyStats stats;
    void Awake()
    {
        stats = statsSO.CreateRuntime();
        setEnemyStats(stats);
    }

    public override void PostStart()
    {
        if (TryGetComponent<NavMeshAgent>(out var agent) && stats != null)
        {
            agent.speed = stats.walkSpeed;
            agent.angularSpeed = stats.turnSpeed;
        }
    } }
