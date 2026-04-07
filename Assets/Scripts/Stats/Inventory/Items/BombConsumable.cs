using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/BombPotion")]
[Serializable]
public class BombConsumable : Item
{
    public DamageData damageData;
    public MeleeHitboxData hitboxData;
    public float knockback = 0f;
    public float stunTime = 0f;
    public override void Perform(StatManager statManager)
    {
        GameObject owner = statManager.gameObject;
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
                DamageData data = new DamageData {baseDamage = damageData.baseDamage, type = damageData.type, source = owner, abilityBase = null};
                damageable.TakeDamage(data);
            }

            if (hit.transform.root.TryGetComponent<StatManager>(out var enemyStatManager))
            {
                enemyStatManager.Knockback(owner.transform.position, knockback);
                enemyStatManager.BasicStun(stunTime);
            }
        }
    }
}