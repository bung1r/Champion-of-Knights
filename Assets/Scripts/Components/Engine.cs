using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Engine : InteractableComponent
{   
    [Tooltip("How much power exerted at full power")]
    public float power = 100f;
    [Tooltip("0-1, percent of power used.")]
    public float powerUnleashed = 0f;
    private float highestPowerSignal = 0f;
    private bool disengageAll = false;
    public Battery battery;
    public bool alwaysOn = false;
    void Start()
    {
        canInteract = false;
    }

    void Update()
    {
        highestPowerSignal = 0f;
        if (alwaysOn) highestPowerSignal = 1f;
    }

    void LateUpdate()
    {
        powerUnleashed = highestPowerSignal;
        if (battery == null) return;
        if (battery.currentEnergy <= 0f) return;
        if (powerUnleashed > 0f)
        {
            float splitPower = connectedComponents.Count;
            battery.flowingEnergy = Mathf.Abs(power) * Time.deltaTime;
            battery.currentEnergy -= Mathf.Abs(power) * Time.deltaTime;
            if (battery.currentEnergy < 0f) return;
            disengageAll = false;
            foreach (InteractableComponent c in connectedComponents)
            {
                if (c.canInteract == false) continue;
                c.MoveComponent(power * powerUnleashed / splitPower);
            }
        } else
        {
            if (disengageAll) return; 
            foreach (InteractableComponent c in connectedComponents)
            {
                if (c.canInteract == false) continue;
                c.DisengageComponent(null);
            }
            disengageAll = true;
            battery.flowingEnergy = 0;
        }
    }
    // Note: Only calls from OTHER components.
    public override void MoveComponent(float signal, GameObject owner = null)
    {
        if (signal > highestPowerSignal)
        {
            highestPowerSignal = signal;
        }
    }

    public override void DisengageComponent(GameObject owner)
    {
    
    }
}