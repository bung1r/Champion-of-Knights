using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName = "WeaponActions/Melee")]
public class MeleeAction : WeaponAction
{
    //define the values for this action
    public float damage = 15f;
    public float range = 1f;
    public float hitDelay = 0.15f;
    public float radius = 0.6f; // arc radius or sphere radius. Leave blank if animation event
    public float critRate = 10f;
    public float critDamageMultiplier = 2f;
    public float variation = 0.2f;


    public override void Execute(GameObject owner, Animator animator)
    {
        owner.GetComponent<MonoBehaviour>().StartCoroutine(DoMeleeHit(owner));
    }

    private IEnumerator DoMeleeHit(GameObject owner)
    {
        yield return new WaitForSeconds(hitDelay);
        Vector3 origin = owner.transform.position + Vector3.up * 0.5f; // chest height
        Vector3 forward = owner.transform.forward;
        Vector3 center = origin + forward * range;
        Collider[] hits = Physics.OverlapSphere(center, radius);

        
        //Calculates the real damage from the basedamage
        float realDamage = damage;
        realDamage *= UnityEngine.Random.Range(1f - variation, 1f + variation);
        if (critRate > UnityEngine.Random.Range(0f, 100f))
        {
            realDamage *= critDamageMultiplier;
            Debug.Log("Wow! What a Critical Hit! Congrats!");
        }

        foreach (var c in hits)
        {
            if (c.gameObject == owner) continue;
            if (c.TryGetComponent<IDamageable>(out var damageable))
            {
                DamageData data = new DamageData { baseDamage = realDamage, type = DamageType.Physical, source = owner };
                damageable.TakeDamage(data);
            }
        }
    }
}
