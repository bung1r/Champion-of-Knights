using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{

    [Tooltip("The specific gear that drives this one")]
    public Gear drivenGear;

    [Tooltip("All gears connected to this one")]
    public List<Gear> connectedGears = new List<Gear>();
    [Tooltip("Any connected components!")]
    public List<InteractableComponent> connectedComponents = new List<InteractableComponent>();

    [Tooltip("Teeth count — affects relative rotation speed")]
    public int teethCount = 20;

    [Tooltip("If true, this gear can drive others")]
    public bool isDriver = false;

    [Tooltip("Angular speed in degrees per second if driver")]
    public float rotationSpeed = 60f;

    private float currentAngle;
    public float rotationAmount;
    public bool constraintReached = false;

    void Update()
    {
        if (isDriver)
        {
            // Rotate this gear
            // if (constraintReached) return;
            SetDrivenRotation(rotationSpeed);
            // currentAngle += rotationSpeed * Time.deltaTime;
            // transform.localRotation = Quaternion.Euler(0, currentAngle, 0);

            // Drive connected gears
            foreach (Gear g in connectedGears)
            {
                if (g.constraintReached)
                {
                    rotationSpeed *= -1;
                }
                // Calculate opposite rotation direction and speed ratio
                float ratio = (float)teethCount / g.teethCount;
                g.SetDrivenRotation(-rotationSpeed * ratio);
            }
        }
        else
        {
            if (drivenGear == null) return;
            if (drivenGear.rotationAmount != 0 || drivenGear.isDriver == true)
            {

                foreach (Gear g in connectedGears)
                {
                    if (g.constraintReached)
                    {
                        constraintReached = true;
                        rotationAmount = 0;
                        return;
                    }
                    // Calculate opposite rotation direction and speed ratio
                    float ratio2 = (float)teethCount / g.teethCount;
                    g.SetDriveRotationNotDriver(-rotationAmount * ratio2);
                    // Debug.Log($"{ratio2}, {-rotationAmount * ratio2}, {rotationAmount}");

                }
                
                foreach (InteractableComponent c in connectedComponents)
                {
                    if (c.constraintReached)
                    {
                        constraintReached = true;
                        rotationAmount = 0;
                        return;
                    }
                    float dist = RotatationToDistance(rotationAmount, transform.localScale.x/2);
                    c.MoveComponent(dist);
                }



            }
            // Turns this gear based off the 'driven gear'


            // Turns any connected gears


        }
    }

    // void LateUpdate()
    // {
    //     rotationAmount = 0;
    // }

    public void SetDrivenRotation(float speed)
    {
        rotationAmount = speed * Time.deltaTime;
        currentAngle += rotationAmount;

        transform.localRotation = Quaternion.Euler(0, currentAngle, 0);
    }

    public void SetDriveRotationNotDriver(float speed)
    {
        rotationAmount = speed;
        currentAngle += rotationAmount;
        transform.localRotation = Quaternion.Euler(0, currentAngle, 0);

    }

    float RotatationToDistance(float rotationAmt, float radius)
    {
        float circum = 2f * Mathf.PI * radius;
        float percentTraveled = rotationAmount / 360;
        float totalDist = percentTraveled * circum;
        return totalDist;
    }

}