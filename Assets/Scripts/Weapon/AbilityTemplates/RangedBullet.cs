using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;

public class RangedBullet : MonoBehaviour
{
    public BulletData bulletData;
    public float timeCreated;
    public float pierceLeft;
    public int layer;
    public int groundLayer;
    private Rigidbody rb;
    void Start()
    {
        timeCreated = Time.time;
        pierceLeft = bulletData.rangedHitboxData.pierce;
        layer = bulletData.damageData.source.layer;
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, bulletData.lifeTime);
    }

    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collision)
    {
        // don't do anything if the source is equal to the target (enemy vs. enemy will not trigger)
        if (collision.gameObject == null) return;
        if (bulletData.damageData.source == null) return;
        if (collision.gameObject.layer == bulletData.damageData.source.layer) return;
        if (collision.transform.root.TryGetComponent<IDamageable>(out var damageable))
        {
            if (collision.transform.root.TryGetComponent<PlayerStatManager>(out var plrStats))
            {
                if (plrStats.stats.isParrying)
                {
                    rb.velocity = plrStats.transform.forward * bulletData.rangedHitboxData.speed * 12;
                    bulletData.owner = plrStats.gameObject;
                    bulletData.damageData.source = plrStats.gameObject;
                } else
                {
                    damageable.TakeDamage(bulletData.damageData);
                    pierceLeft -= 1;
                }
            } else
            {
                damageable.TakeDamage(bulletData.damageData);
                pierceLeft -= 1;
            }
        }

        // if it cannot pierce any longer, just destroy
        if (pierceLeft <= 0)
        {
            Destroy(gameObject);
        }
    }
}