using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponAction primaryAction;
    public WeaponAction secondaryAction;
    public Animator animator;
    public Transform muzzlePoint;

    private float nextPrimaryTime;
    private float nextSecondaryTime;

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void TryPrimary(GameObject owner)
    {
        if (primaryAction == null) return;
        if (Time.time < nextPrimaryTime) return;

        // allow the action to run
        primaryAction.Execute(owner, animator);
        nextPrimaryTime = Time.time + primaryAction.cooldown;
        PlayerStatManager stats = owner.GetComponent<PlayerStatManager>();
        stats.UseStamina(30);

    }

    public void TrySecondary(GameObject owner)
    {
        if (primaryAction == null) return;
        if (Time.time < nextPrimaryTime) return;

        // allow the action to run
        secondaryAction.Execute(owner, animator);
        nextPrimaryTime = Time.time + secondaryAction.cooldown;
    }



}
