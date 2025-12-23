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
    [HideInInspector] public BulletData bulletData;
    [HideInInspector] public float timeCreated;
    [HideInInspector]public float pierceLeft;
    [HideInInspector]public int layer;
    [HideInInspector]public int groundLayer;
    [HideInInspector]public Rigidbody rb;
    private BulletPayload payload;
    private Vector3 lastPos;
    void Start()
    {
        if (bulletData.damageData.source == null)
        {
            Destroy(gameObject);
            return;
        }
        timeCreated = Time.time;
        pierceLeft = bulletData.rangedHitboxData.pierce;
        layer = bulletData.damageData.source.layer;
        // rb = GetComponent<Rigidbody>();
        payload = bulletData.rangedHitboxData.bulletPayload;
        Destroy(gameObject, bulletData.lifeTime);
    }

    void LateUpdate()
    {
        // to prevent explosions from going in the wrong direction because of clipping
        lastPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
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
                    // reflect the bullet
                    Debug.Log(rb == null);
                    rb.velocity = plrStats.transform.forward * bulletData.rangedHitboxData.speed * 12;
                    transform.rotation = Quaternion.LookRotation(rb.velocity);
                    bulletData.owner = plrStats.gameObject;
                    bulletData.damageData.source = plrStats.gameObject;
                    AudioManager.Instance.PlayParrySFX(plrStats.transform);
                    plrStats.OnParry();
                    return;
                } else
                {
                    if (payload != null) payload.OnPierce(transform, collision.transform, bulletData.damageData, lastPos);
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
            if (payload != null) payload.OnAllDestroy(transform, collision.transform, bulletData.damageData, lastPos);
            if (payload != null) payload.OnNormalDestroy(transform, collision.transform, bulletData.damageData, lastPos);
            Destroy(gameObject);
        }
    }
}