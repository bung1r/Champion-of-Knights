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
                Matrix4x4.TRS(sphere.center, Quaternion.identity, Vector3.one * sphere.radius * 2f),
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
                Matrix4x4.TRS(box.center, Quaternion.identity, box.boxSize * 2f),
                material,
                0
            );
            tempBoxes.Add(box);
        }
        allBoxes = tempBoxes;
    }
    public void DrawSphere(Vector3 center, float radius, float lifeTime)
    {
        allSpheres.Add(new SphereHitboxData(center, radius, lifeTime));
    }

    public void DrawCube(Vector3 center, Vector3 boxSize, float lifeTime)
    {
        allBoxes.Add(new BoxHitboxData(center, boxSize, lifeTime));
    }
}

[Serializable]
public class SphereHitboxData
{
    public Vector3 center;
    public float radius;
    public float lifeTime;
    public float timeCreated;
    public SphereHitboxData(Vector3 othercenter, float otherradius, float otherlifeTime)
    {
        center = othercenter;
        radius = otherradius;
        lifeTime = otherlifeTime;
        timeCreated = Time.time;
    }
}

[Serializable]
public class BoxHitboxData
{
    public Vector3 center;
    public Vector3 boxSize;
    public float lifeTime;
    public float timeCreated;

    public BoxHitboxData(Vector3 othercenter, Vector3 otherboxSize, float otherlifeTime)
    {
        center = othercenter;
        boxSize = new Vector3(otherboxSize.x, otherboxSize.y, otherboxSize.z);
        lifeTime = otherlifeTime;
        timeCreated = Time.time;
    }
}