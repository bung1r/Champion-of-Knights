using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/AI Melee Action")]
public class AIMeleeAction : MeleeAction
{
    [Header("Priority Stats")]
    public float basePriority;
    public float minPriority;
    public float maxPriority;
    [HideInInspector]
    public Health healthScript;
    [HideInInspector]
    public EnemyAI enemyAIScript;
    public void getData(GameObject owner)
    {
        healthScript = owner.transform.GetComponent<Health>();
        enemyAIScript = owner.transform.GetComponent<EnemyAI>();
    }
    public virtual float EvaluatePriority(GameObject owner)
    {
        return basePriority;
    }
}


public enum Direction
{
    Right, Left, Up, Down
}