using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponAction : ScriptableObject
{
    //some statistics and shit fr fr
    public string actionName = "Attack";
    public string animationTrigger = "Attack"; // animator trigger to play
    public float cooldown = 0.5f;

    // Called by Weapon when this action is executed.
    // 'owner' is the GameObject executing the action (player).
    public abstract void Execute(GameObject owner, Animator animator);

    // Utility to attempt trigger and return IEnumerator if needed (optional)
    protected void TriggerAnimation(Animator animator)
    {
        if (animator != null && !string.IsNullOrEmpty(animationTrigger))
            animator.SetTrigger(animationTrigger);
    }
}
