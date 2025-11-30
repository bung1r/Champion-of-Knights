using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class RangedHitboxData
{
    public Material raytraceMaterial;
    public RangedHitboxShapes rangedHitboxType = RangedHitboxShapes.Raytrace;
    public float range = 10;
    //
    public Collider[] GetHits(GameObject owner)
    {
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

    public RangedHitboxData() {}
    public RangedHitboxData(RangedHitboxData other)
    {
        rangedHitboxType = other.rangedHitboxType;
        range = other.range;
    }
}

public enum RangedHitboxShapes
{
    Physical, Raytrace
}