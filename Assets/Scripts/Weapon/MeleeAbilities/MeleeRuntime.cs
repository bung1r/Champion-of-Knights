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

public class MeleeRuntime : AbilityRuntime
{
    public MeleeHitboxData hitboxData = new MeleeHitboxData();
    public GameObject owner;

    // This begins all the code for the constructions
    public virtual void ConstructMelee(MeleeAbility other, StatManager manager)
    {
        hitboxData = new MeleeHitboxData(other.hitboxData);
        owner = manager.gameObject;
        hitboxData.transform = owner.transform;
    }
    public MeleeRuntime() {}
    public MeleeRuntime(MeleeAbility other, StatManager manager)
    {
        ConstructBase(other, manager);
        ConstructMelee(other, manager);
    }

    // the actual performing code, pretty important if you ask me. 
    async public override void Perform()
    {
        if (spin != 0f) statManager.Spin(spin);
        if (forward != 0f) statManager.Forward(forward);
        List<DamageMultiplier> allDamageMultipliers = statManager.GetAllDamageMultipliers();
        float critAmt = 1f;
        if (UnityEngine.Random.Range(1,100) <= critRate) {
            // Debug.Log("Oh wow, the crit actually worked?");
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
        List<Transform> hitTransforms = new List<Transform>();
        foreach (Collider hit in HitboxHits)
        {
            if (hit.transform.root.gameObject == owner) continue;
            if (hit.transform.root.gameObject.layer == owner.layer) continue;
            if (hit.transform.root.TryGetComponent<IDamageable>(out var damageable))
            {
                if (hitTransforms.Contains(hit.transform.root)) continue;
                hitTransforms.Add(hit.transform.root);
                DamageData data = new DamageData {baseDamage = realDamage, type = damageData.type, source = owner, abilityBase = abilityBase};
                damageable.TakeDamage(data);
            }
        }
    }
}