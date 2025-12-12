using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class RangedHitboxData
{
    public float range = 10f;
    public int pierce = 1;

    public RangedHitboxShapes rangedHitboxType = RangedHitboxShapes.Raytrace;
    [Header("Physical Bullet Only")]
    public GameObject bulletPrefab;
    public float speed = 4f;
    
    
    // Note that this only works for Raytraces. Physical bullets are much more different!
    public Collider[] GetHits(GameObject owner)
    {
        // if (owner == null) return;
        Vector3 origin = owner.transform.position;
        Vector3 dir = new Vector3(owner.transform.forward.x, 0f, owner.transform.forward.z).normalized;
        Collider[] hits = Physics.RaycastAll(origin, dir, range).Select(h => h.collider).ToArray();
        HitboxVisualizer.Instance.DrawRaytrace(owner.transform, origin, dir);
        // Uncomment to check if anything is getting hit!
        // RaycastHit[] raycastHits = Physics.RaycastAll(origin, dir, range);
        // foreach (RaycastHit hit in raycastHits)
        // {
        //     Debug.Log(hit.collider.name);
        // }
        return hits;
    }

    public void ShootBullet(GameObject owner, BulletData data)
    {
        Transform origin = owner.transform; 
        BulletHandler.Instance.ShootBullet(bulletPrefab, origin, speed, data);
    }
    public RangedHitboxData() {}
    public RangedHitboxData(RangedHitboxData other)
    {
        rangedHitboxType = other.rangedHitboxType;
        range = other.range;
        pierce = other.pierce;
        bulletPrefab = other.bulletPrefab;
        speed = other.speed;
    }
}

public enum RangedHitboxShapes
{
    Physical, Raytrace
}