using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBehavior : MonoBehaviour
{
    private DamageData damageData;
    private MeleeHitboxData hitboxData;
    private GameObject owner;

    void Start()
    {
        StartCoroutine(ExplodeAfterDelay(2.0f));
    }
    public void Initialize(MeleeHitboxData hitboxData, DamageData damageData, GameObject owner)
    {
        this.hitboxData = hitboxData;
        this.damageData = damageData;
        this.owner = owner;
    }
    
    IEnumerator ExplodeAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Explosion logic here, applying damage to nearby objects
        Collider[] HitboxHits = hitboxData.GetHits(gameObject);
        List<Transform> hitTransforms = new List<Transform>();
        foreach (Collider hit in HitboxHits)
        {
            if (hit.transform.root.gameObject == owner) continue;
            if (hit.transform.root.gameObject.layer == owner.layer) continue;
            if (hit.transform.root.TryGetComponent<IDamageable>(out var damageable))
            {
                if (hitTransforms.Contains(hit.transform.root)) continue;
                hitTransforms.Add(hit.transform.root);
                DamageData data = new DamageData {baseDamage = damageData.baseDamage, type = damageData.type, source = owner, abilityBase = null};
                damageable.TakeDamage(data);
            }
        }

        Destroy(gameObject);
    }
    // Other bomb behavior methods here
}