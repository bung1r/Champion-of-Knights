using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class RangedRuntime : AbilityRuntime
{
    public RangedHitboxData hitboxData = new RangedHitboxData();
    public GameObject owner;

    // This begins all the code for the constructions
    public virtual void ConstructRanged(RangedAbility other, StatManager manager)
    {
        hitboxData = new RangedHitboxData(other.hitboxData);
        owner = manager.gameObject;
    }
    public RangedRuntime() {}
    public RangedRuntime(RangedAbility other, StatManager manager)
    {
        ConstructBase(other, manager);
        ConstructRanged(other, manager);
    }

    // the actual performing code, pretty important if you ask me. 
    async public override void Perform()
    {
        List<DamageMultiplier> allDamageMultipliers = statManager.GetAllDamageMultipliers();

        float critAmt = 1f;
        if (UnityEngine.Random.Range(1,100) <= critRate) {
            Debug.Log("the attack crit!");
            critAmt = critMultiplier;
        }
        
        // define additive multipliers and multiplicated multipliers.
        float additiveMultiplier = 1f;
        float multiplicativeMultiplier = 1f;
        foreach (DamageMultiplier damageMultiplier in allDamageMultipliers)
        {
            if (damageMultiplier.type == DamageMultiplierTypes.Additive)
            {
                additiveMultiplier += damageMultiplier.amount;
            } 

            if (damageMultiplier.type == DamageMultiplierTypes.Multiplicative)
            {
                multiplicativeMultiplier *= damageMultiplier.amount;   
            }
        }
        float realDamage = damageData.baseDamage * critAmt * additiveMultiplier * multiplicativeMultiplier * UnityEngine.Random.Range(1f - variation, 1f + variation);
        await Task.Delay((int)(hitboxTimeDelay * 1000));
        Collider[] HitboxHits = hitboxData.GetHits(owner);
        foreach (Collider hit in HitboxHits)
        {
            if (hit.gameObject == owner) continue;
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                DamageData data = new DamageData {baseDamage = realDamage, type = damageData.type, source = owner};
                damageable.TakeDamage(data);
            }
        }

    }
}