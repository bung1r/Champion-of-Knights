using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Battery : MonoBehaviour
{
    public BatterySO batterySO;
    public float currentEnergy;
    public float flowingEnergy;
    public float startEnergy;
    void Start()
    {
        currentEnergy = batterySO.maxEnergy;
        if (startEnergy >= 0) currentEnergy = startEnergy;
    }
    public void ChargeBattery(float energyPerSec)
    {
        energyPerSec = Mathf.Min(energyPerSec, batterySO.maxRechargeRate);
        currentEnergy = Mathf.Clamp(currentEnergy + energyPerSec * Time.deltaTime, 0, batterySO.maxEnergy);
    }

}