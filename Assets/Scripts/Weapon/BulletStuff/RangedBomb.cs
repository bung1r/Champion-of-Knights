using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
[CreateAssetMenu(menuName = "Combat/Bullet Payload/Explosion")]
public class RangedBomb : BulletPayload
{
    public float stunTime = 0.2f;
    public float knockback = 4f;
    public float variation = 0.05f;
    // public bool damageRelativeToBullet = false; // higher bullet dmg = higher explosion dmg
    public DamageData bombDamageData;
    public MeleeHitboxData hitboxData;

    // self is THE BULLET ITSELF
    public override void OnAllDestroy(Transform self, Transform target, DamageData damageData, Vector3 hitPoint)
    {
        Collider[] hits = hitboxData.GetHits(self.gameObject);
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.layer == damageData.source.layer) continue;
            if (hit.transform.root.TryGetComponent<IDamageable>(out var damageable))
            {
                DamageData data = new DamageData(bombDamageData);
                data.source = damageData.source;
                data.baseDamage *= UnityEngine.Random.Range(1f - variation, 1f + variation);
                damageable.TakeDamage(data);
            }
            if (hit.transform.root.TryGetComponent<StatManager>(out var statManager) && hit.transform.root.TryGetComponent<Rigidbody>(out var rb))
            {
                statManager.BasicStun(stunTime);
                statManager.Knockback(hitPoint, knockback);
            }
        }
    }
    public override void OnNormalDestroy(Transform self, Transform target, DamageData damageData, Vector3 hitPoint){}
    public override void OnPierce(Transform self, Transform target, DamageData damageData, Vector3 hitPoint){}

}