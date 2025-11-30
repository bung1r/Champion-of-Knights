using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;
    private StatManager statManager;
    private Stats stats;
    private int isWalkingHash;
    private int isRunningHash;
    private int inAttackAnimHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        statManager = gameObject.GetComponent<StatManager>();
        stats = statManager.GetStats();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        inAttackAnimHash = Animator.StringToHash("inAttackAnim");
    }

    // Update is called once per frame
    void Update()
    {
        // // Debug.Log($"{animator.GetBool(isWalkingHash)}, {animator.GetBool(isRunningHash)}, {animator.GetBool(inAttackAnimHash)}");
        // Debug.Log($"{animator.GetBool(inAttackAnimHash)}, {stats.inAttackAnim}, {Time.time}");
        animator.SetBool(inAttackAnimHash, stats.inAttackAnim);
        animator.SetBool(isWalkingHash, stats.isWalking);
        animator.SetBool(isRunningHash, stats.isRunning);
    }
    private void SetAllVarsFalse()
    {
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(isRunningHash, false);
        animator.SetBool(inAttackAnimHash, false);
    }
}
