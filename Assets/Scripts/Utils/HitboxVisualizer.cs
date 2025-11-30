using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HitboxVisualizer : MonoBehaviour
{
    public static HitboxVisualizer Instance { get; private set; }
    public Material material;
    public List<SphereHitboxData> allSpheres = new List<SphereHitboxData>();
    public List<BoxHitboxData> allBoxes = new List<BoxHitboxData>();
    public List<RaytraceData> allRaytraces = new List<RaytraceData>();
     
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
    }
    void Update()
    {
        List<SphereHitboxData> tempSpheres = new List<SphereHitboxData>();
        foreach (SphereHitboxData sphere in allSpheres)
        {
            if (Time.time - sphere.timeCreated > sphere.lifeTime) continue;
            Graphics.DrawMesh(
                Resources.GetBuiltinResource<Mesh>("Sphere.fbx"),
                Matrix4x4.TRS(sphere.center, sphere.quarternion, Vector3.one * sphere.radius * 2f),
                material,
                0
            );
            tempSpheres.Add(sphere);
        }
        allSpheres = tempSpheres;

        List<BoxHitboxData> tempBoxes = new List<BoxHitboxData>();
        foreach (BoxHitboxData box in allBoxes)
        {
            if (Time.time - box.timeCreated > box.lifeTime) continue;
            Graphics.DrawMesh(
                Resources.GetBuiltinResource<Mesh>("Cube.fbx"),
                Matrix4x4.TRS(box.center, box.quarternion, box.boxSize * 2f),
                material,
                0
            );
            tempBoxes.Add(box);
        }
        allBoxes = tempBoxes;
    
        List<RaytraceData> tempRays = new List<RaytraceData>();
        foreach (RaytraceData ray in allRaytraces)
        {
            if (Time.time - ray.timeCreated > ray.lifeTime) continue;
            Graphics.DrawMesh(
                Resources.GetBuiltinResource<Mesh>("Cube.fbx"),
                Matrix4x4.TRS(ray.center + (ray.dir * 100) , ray.quaternion, new Vector3(0.1f,0.1f,200f)),
                material,
                0
            );
            tempRays.Add(ray);
        }
        allRaytraces = tempRays;
    }
    public void DrawSphere(Transform transform, Vector3 center, float radius, float lifeTime)
    {
        allSpheres.Add(new SphereHitboxData(transform, center, radius, lifeTime));
    }

    public void DrawCube(Transform transform, Vector3 center, Vector3 boxSize, float lifeTime)
    {
        allBoxes.Add(new BoxHitboxData(transform, center, boxSize, lifeTime));
    }
    public void DrawRaytrace(Transform transform, Vector3 origin, Vector3 dir)
    {
        allRaytraces.Add(new RaytraceData(transform, origin, dir, 0.3f));
    }
}

[Serializable]
public class SphereHitboxData
{
    public Quaternion quarternion;
    public Vector3 center;
    public float radius;
    public float lifeTime;
    public float timeCreated;
    public SphereHitboxData(Transform otherTransform, Vector3 othercenter, float otherradius, float otherlifeTime)
    {
        quarternion = new Quaternion(otherTransform.rotation.w, otherTransform.rotation.x, otherTransform.rotation.y, otherTransform.rotation.z);
        center = othercenter;
        radius = otherradius;
        lifeTime = otherlifeTime;
        timeCreated = Time.time;
    }
}

[Serializable]
public class BoxHitboxData
{
    public Quaternion quarternion;
    public Vector3 center;
    public Vector3 boxSize;
    public float lifeTime;
    public float timeCreated;

    public BoxHitboxData(Transform otherTransform, Vector3 othercenter, Vector3 otherboxSize, float otherlifeTime)
    {
        quarternion = new Quaternion(otherTransform.rotation.x, otherTransform.rotation.y, otherTransform.rotation.z, otherTransform.rotation.w);
        center = othercenter;
        boxSize = new Vector3(otherboxSize.x, otherboxSize.y, otherboxSize.z);
        lifeTime = otherlifeTime;
        timeCreated = Time.time;
    }
}

public class RaytraceData
{
    public Quaternion quaternion;
    public Vector3 center;
    public Vector3 dir;
    public float lifeTime;
    public float timeCreated;
    public RaytraceData(Transform otherTransform, Vector3 otherCenter, Vector3 otherDir, float otherlifeTime)
    {
        quaternion = new Quaternion(otherTransform.rotation.x, otherTransform.rotation.y, otherTransform.rotation.z, otherTransform.rotation.w);
        center = new Vector3(otherCenter.x, otherCenter.y, otherCenter.z);
        dir = new Vector3(otherDir.x, otherDir.y, otherDir.z);
        lifeTime = otherlifeTime;
        timeCreated = Time.time;
    }
}