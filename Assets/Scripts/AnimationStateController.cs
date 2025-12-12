using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    private StatManager statManager;
    private Stats stats;
    private int isWalkingHash;
    private int isRunningHash;
    private int inAttackAnimHash;
    private int isGuardingHash;
    private int isParryingHash;
    private int changeStatesHash;
    
    private bool prevIsWalking = false;
    private bool prevInAttackAnim = false;
    private bool prevIsRunning = false;
    private bool prevIsGuarding = false;
    private bool prevIsParrying = false;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        statManager = gameObject.GetComponent<StatManager>();
        stats = statManager.GetStats();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isGuardingHash = Animator.StringToHash("isGuarding");
        inAttackAnimHash = Animator.StringToHash("inAttackAnim");
        changeStatesHash = Animator.StringToHash("changeStates");
        isParryingHash = Animator.StringToHash("isParrying");
    }

    // Update is called once per frame
    void Update()
    {
        // // Debug.Log($"{animator.GetBool(isWalkingHash)}, {animator.GetBool(isRunningHash)}, {animator.GetBool(inAttackAnimHash)}");
        // Debug.Log($"{animator.GetBool(inAttackAnimHash)}, {stats.inAttackAnim}, {Time.time}");
        if (prevIsWalking!=stats.isWalking || prevIsRunning!=stats.isRunning || prevInAttackAnim!=stats.inAttackAnim || prevIsGuarding!=stats.isGuarding || prevIsParrying!=stats.isParrying)
        {
            // Debug.Log($"What a change at {Time.time}");

            // set the booleans of the animation things to whatever the statManager is
            animator.SetBool(inAttackAnimHash, stats.inAttackAnim);
            animator.SetBool(isWalkingHash, stats.isWalking);
            animator.SetBool(isRunningHash, stats.isRunning);
            animator.SetBool(isGuardingHash, stats.isGuarding);
            animator.SetBool(isParryingHash, stats.isParrying);

            // update the previous so it can actually update
            prevIsWalking = stats.isWalking;
            prevIsRunning = stats.isRunning;
            prevInAttackAnim = stats.inAttackAnim;
            prevIsGuarding = stats.isGuarding;
            prevIsParrying = stats.isParrying;

            // trigger to signal an actual change, beyotch!
            animator.SetTrigger(changeStatesHash);
        }
        
    }
}
