using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Gear : InteractableComponent
{

    [Tooltip("All gears connected to this one")]
    public List<Gear> connectedGears = new List<Gear>();

    [Tooltip("Teeth count — affects relative rotation speed")]
    public int teethCount = 20;

    [Tooltip("If true, this gear can drive others")]
    public bool isDriver = false;

    [Tooltip("Angular speed in degrees per second if driver")]
    public float rotationSpeed = 60f;
    public float? savedTrueRotation;
    private float currentAngle;
    public float rotationAmount;
    public float savedConstraintRotation;
    void Update()
    {
        rotationSpeed = 0;
    }

    void LateUpdate()
    {
        
    }
    public void PropagateRotation(float rotationSpeed, HashSet<Gear> visited = null)
    {
        if (visited == null)
            visited = new HashSet<Gear>();

        if (visited.Contains(this))
        {
            if (Mathf.Sign(this.rotationSpeed) != Mathf.Sign(rotationSpeed))
            {
                // Opposing torque detected — stop both
                constraintReached = true;
                this.rotationSpeed = 0;
                rotationSpeed = 0;
            }
            return;
        }
        visited.Add(this);
       
        this.rotationSpeed = rotationSpeed;
        rotationAmount = rotationSpeed * Time.deltaTime;
        currentAngle += rotationAmount;
        transform.localRotation = Quaternion.Euler(0, currentAngle, 0);
        foreach (Gear g in connectedGears)
        {
            if (g.constraintReached) continue;

            float ratio = (float)teethCount / g.teethCount;
            g.PropagateRotation(-rotationSpeed * ratio, visited); // reverse direction
        }

        // Apply to connected components (doors, sliders, etc.)
        foreach (InteractableComponent c in connectedComponents)
        {
            float dist = RotatationToDistance(rotationAmount, transform.localScale.x / 2);
            c.MoveComponent(dist);

            if (c.constraintReached) {
                BackPropagateStop(new HashSet<Gear>());
            return;
            }   
        }
    }
    public void BackPropagateStop(HashSet<Gear> visited)
    {
        if (visited.Contains(this)) return;
        visited.Add(this);

        savedConstraintRotation = rotationSpeed;
        // rotationSpeed = 0;
        constraintReached = true;

        foreach (var g in connectedGears)
        {
            if (!g.constraintReached)
                g.BackPropagateStop(visited);
        }
    }
    public void ResetAllConstraints(HashSet<Gear> visited = null)
    {
        if (visited == null)
            visited = new HashSet<Gear>();

        if (visited.Contains(this)) return;
        visited.Add(this);

        constraintReached = false;

        foreach (Gear c in connectedGears)
        {
            c.ResetAllConstraints(visited);
        }
    }
    float RotatationToDistance(float rotationAmt, float radius)
    {
        float circum = 2f * Mathf.PI * radius;
        float percentTraveled = rotationAmount / 360;
        float totalDist = percentTraveled * circum;
        return totalDist;
    }

    // note: the 'distance' is actually the strength of the player
    public override void MoveComponent(float distance, GameObject owner = null)
    {
        // if (savedTrueRotation == null) savedTrueRotation = rotationSpeed;
        rotationSpeed += distance;
        // throw new NotImplementedException();
    }

    public override void DisengageComponent(GameObject owner)
    {
        if (savedTrueRotation == null) return;
        rotationSpeed = (float)savedTrueRotation;
        savedTrueRotation = null;
        
        // throw new System.NotImplementedException();
    }
}