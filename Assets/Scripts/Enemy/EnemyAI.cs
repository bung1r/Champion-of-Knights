using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
public class EnemyAI : MonoBehaviour
{
    public GameObject target;
    public Animator animator;
    // public List<AIMeleeAction> attackActions;
    public float moveSpeed = 2f;
    public float rotationSpeed = 20f;
    public float searchRadius = 100f;
    public LayerMask targetLayer;
    public float timeBetweenAttacks = 2f;
    public bool inAttack = false;
    private float timeSinceLastAttack = Mathf.Infinity;
    private Rigidbody rb;
    // Start is called before the first frame update
    private Health healthScript;
    private EnemyAI thisScript;
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        healthScript = GetComponent<Health>();
        thisScript = GetComponent<EnemyAI>();

        // foreach (AIMeleeAction action in attackActions)
        // {
        //     action.healthScript = healthScript;
        //     action.enemyAIScript = thisScript;
        // }
    }

    // Update is called once per frame
    void Update()
    {
        target = SearchForTarget();

        
    }
    void FixedUpdate()
    {
        TurnToTarget();
        SmartMove();
        TryAttack();
    }
    void TurnToTarget()
    {
        if (target == null) return;

        // gets a vector between the enemy and target, then sets y to 0.
        Vector3 lookdir = target.transform.position - transform.position;
        lookdir.y = 0;
        // stuff so the rotation happens smoothly, wow!
        Quaternion targetRotation = Quaternion.LookRotation(lookdir, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );


    }
    void SmartMove()
    {
        if (target == null) return;
        if (rb == null) return;
        Vector3 dir = (target.transform.position - transform.position).normalized;
        dir.y = 0;
        rb.MovePosition(transform.position + dir * moveSpeed * Time.fixedDeltaTime);
    }
    
    void TryAttack()
    {
        if (inAttack) return;
        timeSinceLastAttack += Time.fixedDeltaTime;
        if (timeSinceLastAttack < timeBetweenAttacks) return;
        List<float> weights = new List<float>();
        // foreach (AIMeleeAction action in attackActions)
        // {
        //     weights.Add(action.EvaluatePriority());
        // }
        // AIMeleeAction chosenAction = WeightedRandom.Choose(attackActions, weights);
        // chosenAction.Execute(gameObject, null);
    }
    
    
    
    
    
    GameObject SearchForTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, searchRadius, targetLayer);
        // Store unique parent (root) transforms here
        HashSet<Transform> uniqueParents = new HashSet<Transform>();
        foreach (Collider hit in hits)
        {
            Transform root = hit.transform.root;
            uniqueParents.Add(root);
        }

        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (Transform parent in uniqueParents)
        {
            float dist = Vector3.Distance(transform.position, parent.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = parent.gameObject;
            }
        }

        return closest;
    }
}
