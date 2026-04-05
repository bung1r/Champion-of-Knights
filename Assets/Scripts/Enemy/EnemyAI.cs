using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data.SqlTypes;
using System.Net;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyStatManager), typeof(BoxCollider), typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, BarrelHandler
{
    public GameObject target;

    public EnemyStatManager statManager;
    public List<EnemyAbility> enemyAbilities = new List<EnemyAbility>();
    private List<AbilityRuntime> enemyAbilityRuntimes = new List<AbilityRuntime>();
    [HideInInspector] public EnemyStats stats;
    // public Animator animator;
    public List<Transform> Barrels = new List<Transform>();
    public List<Transform> barrels { get => Barrels; set => Barrels = value; }
    public BoxCollider hitbox;
    public int lastBarrelFiredIndex {get; set;} = 0;
    [NonSerialized] public float timeEndLastAttack;
    public bool retreating = false;
    public bool runTowards = false;
    public LayerMask targetLayer;
    [HideInInspector] public Rigidbody rb;
    private float distance;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    private float lastUpdatePos = -999f;
    private float posUpdateInterval = 0.2f;
    public AbilityRuntime currentAbility = null;
    // Start is called before the first frame update
    private EnemyAI thisScript;
    public void Start()
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
    public void Update()
    {
        target = SearchForTarget();
    }
    public void FixedUpdate()
    {
        PreFixedUpdate();
        CoreFixedUpdate();
        PostFixedUpdate();
    }
    public void TurnToTarget()
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
    // calc short for calculator, by the way.
    public virtual float CalcSpeed(float speed)
    {
        float divider = 0;
        if (stats.isGuarding && currentAbility is EnemyGuardRuntime guard)
        {
            divider += guard.guardStats.speedReduction;
        }


        if (divider == 0) divider = 1f;
        if (stats.inAttackAnim) divider *= 3f;
        return speed/divider;
    }
    public virtual void SmartMove()
    {
        if (target == null) return;
        if (rb == null) return;
        if (stats.stunTime > 0) {
            if (currentAbility != null)
            {
                currentAbility.Cancel();
                currentAbility = null;
            }
            return;
        }

        if (navMeshAgent.enabled == false) {
            rb.isKinematic = true;
            navMeshAgent.enabled = true;
        }    

        // update the target position every so often
        if (Time.time - lastUpdatePos > posUpdateInterval)
        {
            navMeshAgent.SetDestination(target.transform.position);
            lastUpdatePos = Time.time;
        }

        // stop if within comfort distance
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance <= stats.comfortDist)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.updateRotation = false;
            navMeshAgent.angularSpeed = stats.turnSpeed;
        } else
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.updateRotation = true;
            navMeshAgent.speed = CalcSpeed(stats.walkSpeed);
        }

        if (navMeshAgent.isStopped)
        {
            stats.isWalking = false;
            Vector3 lookdir = target.transform.position - transform.position;
            lookdir.y = 0;
            // stuff so the rotation happens smoothly, wow!
            Quaternion targetRotation = Quaternion.LookRotation(lookdir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                stats.turnSpeed/100 * Time.fixedDeltaTime
            );
        } else
        {
            stats.isWalking = true;
        }
    }
    public virtual bool TryAttack()
    {
        List<int> includedIndexes = new List<int>();
        for (int i = 0; i < enemyAbilityRuntimes.Count; i++)
        {
            includedIndexes.Add(i);
        }
        return TryAttack(includedIndexes);
    }
    public virtual bool TryAttack(List<int> includedIndexes)
    {
        if (target == null) return false;
        if (stats.inAttackAnim)
        {
            timeEndLastAttack = Time.time;
            return false;
        } 
        if (stats.isGuarding) return false;
        if (retreating) return false;
        if (runTowards) return false;
        if (Time.time - timeEndLastAttack < stats.timeBetweenMoves) return false;
        if (stats.stunTime > 0) return false;
        bool choose = false;
        List<float> weights = new List<float>();
        List<AbilityRuntime> possibleAbilities = new List<AbilityRuntime>();
        foreach (int i in includedIndexes) possibleAbilities.Add(enemyAbilityRuntimes[i]);   
        foreach (AbilityRuntime abilityRuntime in possibleAbilities)
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
            currentAbility = WeightedRandom.Choose(possibleAbilities, weights);      

            // use the ability
            currentAbility.BeginUse();
            currentAbility.Use();

            return true;
        } 

        return false;
    }
    
    public virtual bool TryAttack(int a=-1, int b=-1, int c=-1, int d=-1, int e=-1) {
        List<int> temp = new List<int>();
        if (a!=-1) temp.Add(a);
        if (b!=-1) temp.Add(b);
        if (c!=-1) temp.Add(c);
        if (d!=-1) temp.Add(d);
        if (e!=-1) temp.Add(e);
        return TryAttack(temp);
    }
    public float CalculatePriority(EnemyAbilityData enemyAbilityData) 
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

    public virtual void PreStart() {}
    public virtual void PostStart() {}
    public virtual void PreFixedUpdate() {}
    public virtual void CoreFixedUpdate()
    {
        if (target!= null) distance = Vector3.Distance(transform.position, target.transform.position);
        TurnToTarget();
        SmartMove();
        TryAttack();
    }
    public virtual void PostFixedUpdate() {}
}
