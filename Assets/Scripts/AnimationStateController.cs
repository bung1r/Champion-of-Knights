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
    private int attackIDHash;
    private int upperBodyChangeHash;
    
    private bool prevIsWalking = false;
    private bool prevInAttackAnim = false;
    private bool prevIsRunning = false;
    private bool prevIsGuarding = false;
    private bool prevIsParrying = false;
    private int prevAttackID = -1;
    void Start()
    {
        // basic gettings
        animator = gameObject.GetComponentInChildren<Animator>();
        statManager = gameObject.GetComponent<StatManager>();
        stats = statManager.GetStats();

        // get the hashes to improve efficiency 
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isGuardingHash = Animator.StringToHash("isGuarding");
        inAttackAnimHash = Animator.StringToHash("inAttackAnim");
        changeStatesHash = Animator.StringToHash("changeStates");
        isParryingHash = Animator.StringToHash("isParrying");
        attackIDHash = Animator.StringToHash("attackID");
        upperBodyChangeHash = Animator.StringToHash("upperBodyChange");
    }

    // Update is called once per frame
    void Update()
    {
        // // Debug.Log($"{animator.GetBool(isWalkingHash)}, {animator.GetBool(isRunningHash)}, {animator.GetBool(inAttackAnimHash)}");
        // Debug.Log($"{animator.GetBool(inAttackAnimHash)}, {stats.inAttackAnim}, {Time.time}");
        
        if (prevInAttackAnim != stats.inAttackAnim  || prevInAttackAnim!=stats.inAttackAnim || prevIsGuarding!=stats.isGuarding || prevIsParrying!=stats.isParrying)
        {
            animator.SetTrigger(upperBodyChangeHash);
        }
        
        if (prevIsWalking!=stats.isWalking || prevIsRunning!=stats.isRunning || prevInAttackAnim!=stats.inAttackAnim || prevIsGuarding!=stats.isGuarding || prevIsParrying!=stats.isParrying)
        {
            // Debug.Log($"What a change at {Time.time}");

            // set the booleans of the animation things to whatever the statManager is
            animator.SetBool(inAttackAnimHash, stats.inAttackAnim);
            animator.SetBool(isWalkingHash, stats.isWalking);
            animator.SetBool(isRunningHash, stats.isRunning);
            animator.SetBool(isGuardingHash, stats.isGuarding);
            animator.SetBool(isParryingHash, stats.isParrying);
            animator.SetInteger(attackIDHash, stats.attackID);

            // update the previous so it can actually update
            prevIsWalking = stats.isWalking;
            prevIsRunning = stats.isRunning;
            prevInAttackAnim = stats.inAttackAnim;
            prevIsGuarding = stats.isGuarding;
            prevIsParrying = stats.isParrying;
            prevAttackID = stats.attackID;
            
            // trigger to signal an actual change, beyotch!
            animator.SetTrigger(changeStatesHash);
        }
        
    }
}
