using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class RangedBullet : MonoBehaviour
{
    [HideInInspector] public BulletData bulletData;
    [HideInInspector] public float timeCreated;
    [HideInInspector]public float pierceLeft;
    [HideInInspector]public int layer;
    [HideInInspector]public int groundLayer;
    [HideInInspector]public int obstacleLayer;
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
        groundLayer = LayerMask.NameToLayer("Ground");
        obstacleLayer = LayerMask.NameToLayer("Obstacle");
        Destroy(gameObject, bulletData.lifeTime);
    }

    void LateUpdate()
    {
        // to prevent explosions from going in the wrong direction because of clipping
        lastPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision == null) return;
        if (collision.gameObject == null) return;
        if (bulletData.damageData.source == null) return;
        if (collision.gameObject.layer == obstacleLayer) {
            if (payload != null) payload.OnAllDestroy(transform, collision.transform, bulletData.damageData, lastPos);
            if (payload != null) payload.OnNormalDestroy(transform, collision.transform, bulletData.damageData, lastPos);
            AudioManager.Instance.PlayHitWallSFX(transform);
            Destroy(gameObject);
            return;
        }

        if (collision.transform.root.TryGetComponent<PlayerStatManager>(out var plrStats))
        {
            // if the player is the target, give them a tiny delay to reflect
            BulletHandling(collision, 80);
        } else
        {
            BulletHandling(collision);
        }
    }

    // the only purpose is to allow the player one extra frame...
    async void BulletHandling(Collider collision, int delay = 0)
    {
        await Task.Delay(delay);
        if (collision == null) return;
        if (collision.gameObject == null) return;
        if (bulletData.damageData.source == null) return;
        if (collision.gameObject.layer == bulletData.damageData.source.layer) return;
        if (this == null) return;
        // if (!payload) return;
        // if (lastPos == null) return;

        if (collision.transform.root.TryGetComponent<IDamageable>(out var damageable))
        {
            if (collision.transform.root.TryGetComponent<PlayerStatManager>(out var plrStats))
            {
                if (plrStats.stats.isParrying)
                {
                    // reflect the bullet
                    if (rb == null) return;
                    rb.velocity = plrStats.transform.forward * bulletData.rangedHitboxData.speed * 25;
                    transform.rotation = Quaternion.LookRotation(rb.velocity);
                    bulletData.owner = plrStats.gameObject;
                    bulletData.damageData.source = plrStats.gameObject;
                    plrStats.OnParry();
                    return;
                } else
                {
                    if (transform != null && payload != null && collision != null && lastPos != null)
                    {
                        payload.OnPierce(transform, collision.transform, bulletData.damageData, lastPos);
                    } 
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
            if (this == null) return;
            if (gameObject == null) return;
            if (payload != null) payload.OnAllDestroy(transform, collision.transform, bulletData.damageData, lastPos);
            if (payload != null) payload.OnNormalDestroy(transform, collision.transform, bulletData.damageData, lastPos);
            Destroy(gameObject);
        }
    }
}