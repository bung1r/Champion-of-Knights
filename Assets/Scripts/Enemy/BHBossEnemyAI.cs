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
public class BHBossEnemyAI : EnemyAI
{
    // the switching code fr
    private BHBossState prevState = BHBossState.Nothing;
    public BHBossState currentState = BHBossState.AuraFarm; // will become the initial state



    [SerializeField] private int restAbilityIndex;
    [SerializeField] private int guardAbilityIndex;
    [SerializeField] private List<int> meleeAbilityIndexes = new List<int>();
    [SerializeField] private List<int> rangedAbilityIndexes = new List<int>();


    public override void CoreFixedUpdate()
    {
        HandleState();
        SmartMove();
    }

    void HandleState()
    {
        if (target == null) return;

        // default target thingy
        if (navMeshAgent.enabled)
        {
            navMeshAgent.SetDestination(target.transform.position);
        }

        // switch case switch case nyeheheh
        switch (currentState)
        {
            case BHBossState.AuraFarm:
                AuraFarm();
                break;
            case BHBossState.Chase:
                Chase();
                break; 
            case BHBossState.AttackRetreat:
                AttackRetreat();
                break;
            case BHBossState.Attack:
                Attack();
                break;
            case BHBossState.Rest:
                Rest();
                break;
        }
    }

    public override void PostStart()
    {
        navMeshAgent.updateRotation = false;
    }
    // short for calculator, by the way
    public override float CalcSpeed(float speed)
    {
        float calcs = base.CalcSpeed(speed);
        if (isAuraFarming) calcs /= 2;
        return calcs;
    }
    public override void SmartMove()
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
        
        navMeshAgent.updateRotation = false;

        if (navMeshAgent.enabled == false) {
            rb.isKinematic = true;
            navMeshAgent.enabled = true;
        }
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= stats.comfortDist && currentState != BHBossState.AttackRetreat)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.angularSpeed = stats.turnSpeed;
        } else
        {
            navMeshAgent.isStopped = false;

            if (stats.isRunning)
            {
                navMeshAgent.speed = CalcSpeed(stats.sprintSpeed);
            } else
            {
                navMeshAgent.speed = CalcSpeed(stats.walkSpeed);
            }
            
        }

        Vector3 lookdir = target.transform.position - transform.position;
            lookdir.y = 0;
            // stuff so the rotation happens smoothly, wow!
            Quaternion targetRotation = Quaternion.LookRotation(lookdir, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                stats.turnSpeed/50 * Time.fixedDeltaTime
            );

        
        if (navMeshAgent.isStopped)
        {
            stats.isWalking = false;
        } else
        {
            stats.isWalking = true;
        }
    }
    private float auraFarmDistanceMin = 6f;
    private float auraFarmDistanceMax = 11f;
    private float auraTime; // how long does my boy aura farm?
    private bool isAuraFarming;
    public void AuraFarm()
    {
        // state enter
        if (currentState != prevState)
        {
            auraTime = Rand(3f, 5f);
            isAuraFarming = true;
            prevState = currentState;
        }

        if (target == null) return;

        auraTime -= Time.fixedDeltaTime;

        float dist = Vector3.Distance(transform.position, target.transform.position);
        // before aura farming is over decisions
        if (dist > auraFarmDistanceMax)
        {
            int choice = Rand(1,4);
            if (choice == 1)
            {
                currentState = BHBossState.Attack;
            } else
            {
                currentState = BHBossState.Chase;
            }
        } else if (dist < auraFarmDistanceMin)
        {
            currentState = BHBossState.Attack;
        }


        if (auraTime <= 0)
        {
            if (dist > 8f)
            {
                currentState = BHBossState.Chase;
            } else
            {
                currentState = BHBossState.Attack;
            }

        }

        // state exit
        if (currentState != prevState)
        {
            isAuraFarming = false;
        }
    }
    public void Chase()
    {
        // state enter
        if (currentState != prevState)
        {

            statManager.stats.isRunning = true;
            prevState = currentState;
        }
        
        float dist = Vector3.Distance(transform.position, target.transform.position);

        if (dist < 6f)
        {
            currentState = BHBossState.Attack;
        }

        // state exit
        if (currentState != prevState)
        {
            statManager.stats.isRunning = false;
        }
    }
    private float meleeAttackRange = 5f;
    private int consecAttacks = 0;
    private int consecMeleeAttacks = 0;
    private int consecRangedAttacks = 0;
    public void Attack()
    {
        float dist = Vector3.Distance(transform.position, target.transform.position);
        // state enter
        if (currentState != prevState)
        {
            prevState = currentState;
        }

        bool succeed = false;
        if (dist <= meleeAttackRange)
        {
            succeed = TryAttack(meleeAbilityIndexes);
            if (succeed) {consecMeleeAttacks++; consecRangedAttacks = 0;}
        } else
        {
            succeed = TryAttack(rangedAbilityIndexes);
            if (succeed) {consecRangedAttacks++; consecMeleeAttacks = 0;}
        }

        if (succeed)
        {
            consecAttacks++;

            if (consecMeleeAttacks >= 3)
            {
                currentState = BHBossState.AttackRetreat;
            }
            
            if (Rand(consecAttacks, 11) == 11 && consecAttacks > 7)
            {
                currentState = BHBossState.Rest;
            }
        } 
  
    }
    
    private Vector3 targetPositionAttackRetreat;
    private float[] backDistances = {8f, 6f, 4f};
    private float[] frontDistances = {8f, 6f, 4f};
    // private Vector3 previousPosition;
    // private float timeSinceRetreat;
    public void AttackRetreat()
    {
        // state enter
        if (currentState != prevState)
        {
            Vector3 directionToPlayer = (target.transform.position - transform.position).normalized;
            // Move to a point 20m away from the player
            targetPositionAttackRetreat = Vector3.zero;
            // timeSinceRetreat = Time.fixedDeltaTime;

            NavMeshPath path = new NavMeshPath();

            // Randomly choose front or back
            bool goFront = Rand(0, 1) == 0;
            float[] distances = goFront ? frontDistances : backDistances;
            Vector3 dir = goFront ? directionToPlayer : -directionToPlayer;

            foreach (float dist in distances)
            {
                Vector3 testPos = target.transform.position + dir * dist;

                // Try to snap to nearby NavMesh
                if (NavMesh.SamplePosition(testPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    // Check if it's actually reachable
                    if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
                    {
                        if (path.status == NavMeshPathStatus.PathComplete)
                        {
                            // Debug.Log("This path is valid: " + hit.position);
                            targetPositionAttackRetreat = hit.position;
                            break;
                        }
                    }
                }
            }

        
            stats.isRunning = true;
            prevState = currentState;

            if (targetPositionAttackRetreat == Vector3.zero)
            {
                // if no valid position found, attack i guess
                consecMeleeAttacks = 0;
                consecRangedAttacks = 0;
                currentState = BHBossState.Attack;
            }
        }

        
        if (navMeshAgent.enabled)
        {
            // Debug.Log(targetPositionAttackRetreat);
            navMeshAgent.SetDestination(targetPositionAttackRetreat); 
        }

        if (Vector3.Distance(targetPositionAttackRetreat, transform.position) < 2f)
        {
            currentState = BHBossState.Attack;
        }

        // state exit
        if (currentState != prevState)
        {
            
            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(target.transform.position);
            }
            stats.isRunning = false;
        }
    }
    public void Retreat()
    {
        // state enter
        if (currentState != prevState)
        {
            prevState = currentState;
        }
    
    }

    private float restStart;
    private float restDuration;
    public void Rest()
    {
        // state enter
        if (currentState != prevState)
        {
            Debug.Log("Boss is resting, beware of the next attack!");
            consecAttacks = 0;
            consecMeleeAttacks = 0;
            consecRangedAttacks = 0;
            restDuration = enemyAbilities[restAbilityIndex].ability.attackLength;
            restStart = Time.fixedTime;
            prevState = currentState;
            if (currentAbility != null) currentAbility.Cancel();
            TryAttack(restAbilityIndex);
        }

        if (Time.fixedTime - restStart > restDuration + 0.5)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);

            if (dist > 10f)
            {
                int choice = Rand(1,4);
                if (choice == 1)
                {
                    currentState = BHBossState.Attack;
                } else
                {
                    currentState = BHBossState.Chase;
                }
            } else
            {
                currentState = BHBossState.Attack;
            }
        }
    
    
    }

    public void Phase2Entrance()
    {
        // state enter
        if (currentState != prevState)
        {
            prevState = currentState;
        }
    }

    public float Rand(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    public int Rand(int min, int max)
    {
        return UnityEngine.Random.Range(min, max + 1); // to make it inclusive
    }
}


public enum BHBossState
{
    AuraFarm, // walk slower towards the player, basically idle. 
    Attack, // attack??? you stupid??
    AttackRetreat, // retreat so the boss can do more ranged attacks
    Retreat, // not sure if i shall add
    Chase, // when the player is toofar
    Rest, // the boss is tired, deal extra dmg probs.
    Phase2Entrance, // depends
    Nothing, // a placeholder
}