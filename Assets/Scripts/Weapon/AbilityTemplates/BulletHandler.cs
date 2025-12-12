using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class BulletHandler : MonoBehaviour
{
    public static BulletHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
    }
    void Start()
    {
        // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Player"), true);
        // Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Enemy"), true);
    }
    public void ShootBullet(GameObject bulletPrefab, Transform origin, float speed, BulletData data)
    {
        if (bulletPrefab == null)
        {
            Debug.Log("The Bullet prefab is null, somehow!");
        }
        else
        {
            GameObject bullet = Instantiate(bulletPrefab, origin.position + new Vector3(0,0.5f,0), origin.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            RangedBullet bulletScript = bullet.GetComponent<RangedBullet>();
            bulletScript.bulletData = data;
            rb.velocity = origin.forward * speed * 10;
        }
       
    }
}

[Serializable]
public class BulletData
{
    public RangedHitboxData rangedHitboxData;
    public DamageData damageData;
    public GameObject owner;
    public float lifeTime = 1;
    public BulletData(RangedHitboxData other1, DamageData other2, GameObject other3)
    {
        rangedHitboxData = new(other1);
        damageData = new(other2);
        owner = other3;
    }
}