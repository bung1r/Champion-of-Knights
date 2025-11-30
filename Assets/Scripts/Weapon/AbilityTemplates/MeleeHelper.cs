using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class MeleeHitboxData
{
    public Transform transform;
    public PhysicsHitboxShapes shape = PhysicsHitboxShapes.Sphere;
    public float radius = 10f;
    public Vector3 boxRadius = new Vector3(5f,5f,5f);
    // make sure the hitboxOffset has 0.5f for y for chest offset
    // unless you don't want that?
    public Vector3 hitboxOffset = new Vector3(0f,0.5f,1f);
    public Collider[] GetHits(GameObject owner)
    {
        Vector3 origin = owner.transform.position;
        Vector3 forward = owner.transform.forward;
        Vector3 up = owner.transform.up;
        Vector3 side = owner.transform.right;
        Vector3 center = origin + (forward * hitboxOffset.z) + (up * hitboxOffset.y) + (side * hitboxOffset.x);
        if (shape == PhysicsHitboxShapes.Sphere)
        {
            HitboxVisualizer.Instance.DrawSphere(transform, center, radius, 0.3f);
            return Physics.OverlapSphere(center, radius);
        } else
        {
            HitboxVisualizer.Instance.DrawCube(transform, center, boxRadius, 0.3f);
            return Physics.OverlapBox(center, boxRadius);
        }
        
    }

    public MeleeHitboxData() {}
    public MeleeHitboxData(MeleeHitboxData other)
    {
        transform = other.transform;
        shape = other.shape;
        radius = other.radius;
        boxRadius = new Vector3(other.boxRadius.x, other.boxRadius.y, other.boxRadius.z);
        hitboxOffset = new Vector3(other.hitboxOffset.x, other.hitboxOffset.y, other.hitboxOffset.z);
    }
}

[Serializable]
public enum PhysicsHitboxShapes
{
    Box, Sphere 
}