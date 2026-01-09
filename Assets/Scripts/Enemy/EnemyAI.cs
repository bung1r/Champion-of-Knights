using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStatManager))]
public class EnemyAI : MonoBehaviour, BarrelHandler
{
    public GameObject target;
    public EnemyStatManager statManager;
    public List<EnemyAbility> enemyAbilities = new List<EnemyAbility>();
    private List<AbilityRuntime> enemyAbilityRuntimes = new List<AbilityRuntime>();
    private EnemyStats stats;
    public Animator animator;
    public List<Transform> Barrels = new List<Transform>();
    public List<Transform> barrels { get => Barrels; set => Barrels = value; }
    public BoxCollider hitbox;
    public int lastBarrelFiredIndex {get; set;} = 0;
    [NonSerialized] public float timeEndLastAttack;
    public float moveSpeed = 2f;
    public float rotationSpeed = 20f;
    public float searchRadius = 100f;
    public bool retreating = false;
    public bool runTowards = false;
    public LayerMask targetLayer;
    private Rigidbody rb;
    private float distance;
    private NavMeshAgent navMeshAgent;
    // Start is called before the first frame update
    private EnemyAI thisScript;
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        thisScript = GetComponent<EnemyAI>();
        statManager = gameObject.GetComponent<EnemyStatManager>();
        stats = statManager.stats;
        foreach (EnemyAbility ability in enemyAbilities)
        {
            enemyAbilityRuntimes.Add(ability.CreateRuntimeInstance(ability, statManager));
        }
        hitbox = GetComponent<BoxCollider>();
        if (hitbox == null) hitbox = GetComponentInChildren<BoxCollider>();

        float dia = Math.Min(hitbox.size.x, hitbox.size.z);
        navMeshAgent.radius = dia / 2f * 0.9f; // slightly smaller than hitbox
        
    }

    // Update is called once per frame
    void Update()
    {
        target = SearchForTarget();
    }
    void FixedUpdate()
    {
        if (target!= null) distance = Vector3.Distance(transform.position, target.transform.position);
        TurnToTarget();
        SmartMove();
        TryAttack();
    }
    void TurnToTarget()
    {
        // if (target == null) return;
        // if (stats.stunTime > 0) return;
        // // gets a vector between the enemy and target, then sets y to 0.
        // Vector3 lookdir = target.transform.position - transform.position;
        // lookdir.y = 0;
        // // stuff so the rotation happens smoothly, wow!
        // Quaternion targetRotation = Quaternion.LookRotation(lookdir, Vector3.up);
        // transform.rotation = Quaternion.Slerp(
        //     transform.rotation,
        //     targetRotation,
        //     stats.turnSpeed * Time.fixedDeltaTime
        // );


    }
    void SmartMove()
    {
        if (target == null) return;
        if (rb == null) return;
        if (stats.stunTime > 0) return;

        // Vector3 dir = (target.transform.position - transform.position).normalized;
        // dir.y = 0;
        // if (distance < stats.runAwayDist)
        // {
        //     retreating = true;
        //     dir = (transform.position - target.transform.position).normalized;
        //     dir.y = 0;
        //     if (statManager.CanUseStamina(stats.sprintStaminaCost * Time.fixedDeltaTime))
        //     {
        //         stats.isRunning = true;
        //         statManager.UseStamina(stats.sprintStaminaCost * Time.fixedDeltaTime);
        //         rb.MovePosition(transform.position + dir * stats.sprintSpeed * Time.fixedDeltaTime);
        //     } else
        //     {
        //         stats.isRunning = false;
        //         stats.isWalking = true;
        //         rb.MovePosition(transform.position + dir * stats.walkSpeed * Time.fixedDeltaTime);
        //     }  
        // } else if (distance > stats.runTowardsDist) 
        // {
        //     if (statManager.CanUseStamina(stats.sprintStaminaCost * Time.fixedDeltaTime))
        //     {
        //         stats.isRunning = true;
        //         statManager.UseStamina(stats.sprintStaminaCost * Time.fixedDeltaTime);
        //         rb.MovePosition(transform.position + dir * stats.sprintSpeed * Time.fixedDeltaTime);
        //     } else
        //     {
        //         stats.isRunning = false;
        //         stats.isWalking = true;
        //         rb.MovePosition(transform.position + dir * stats.walkSpeed * Time.fixedDeltaTime);
        //     }  
        // } else if (distance <= stats.comfortDist) // if it's comfortable, don't move it
        // {
        //     stats.isRunning = false;
        //     stats.isWalking = false;
        //     retreating = false; 
        // } else
        // {
        //     stats.isRunning = false;
        //     stats.isWalking = true;
        //     retreating = false;
        //     rb.MovePosition(transform.position + dir * stats.walkSpeed * Time.fixedDeltaTime);
        // }
        if (navMeshAgent.enabled == false) navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(target.transform.position);
    }
    void TryAttack()
    {
        if (target == null) return;
        if (stats.inAttackAnim)
        {
            timeEndLastAttack = Time.time;
            return;
        } 
        if (retreating) return;
        if (runTowards) return;
        if (Time.time - timeEndLastAttack < stats.timeBetweenMoves) return;
        if (stats.stunTime > 0) return;
        bool choose = false;
        List<float> weights = new List<float>();
        foreach (AbilityRuntime abilityRuntime in enemyAbilityRuntimes)
        {
            if (abilityRuntime is EnemyAbilityI enemyAbility)
            {
                if (CalculatePriority(enemyAbility.enemyAbilityData) > 0)
                {
                    choose = true;
                }
                weights.Add(CalculatePriority(enemyAbility.enemyAbilityData));

            }
        }
        
        if (choose)
        {
            AbilityRuntime chosenAbility = WeightedRandom.Choose(enemyAbilityRuntimes, weights);      

            // use the ability
            chosenAbility.BeginUse();
            chosenAbility.Use();
        } 
    }
    
    float CalculatePriority(EnemyAbilityData enemyAbilityData) 
    {
        // float distance = Vector3.(transform.position, target.transform.position);
        if (enemyAbilityData.minAttackRange > distance || enemyAbilityData.maxAttackRange < distance) return 0;
        if (enemyAbilityData.linearInterpolation)
        {
            float ratio = (distance - enemyAbilityData.minAttackRange)/(enemyAbilityData.maxAttackRange - enemyAbilityData.minAttackRange);
            if (ratio > 0.5)
            {
                return Mathf.Lerp(enemyAbilityData.basePriority, enemyAbilityData.priorityAtMax, (ratio-0.5f)*2f);
            } else if (ratio < 0.5)
            {
                return Mathf.Lerp(enemyAbilityData.basePriority, enemyAbilityData.priorityAtMin, 1f - (ratio * 2f));
            } else
            {
                return enemyAbilityData.basePriority;
            }
        } else
        {
            return enemyAbilityData.basePriority;
        }
    }
    
    GameObject SearchForTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, stats.aggroRange, targetLayer);
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
