using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Components/Battery")]
public class BatterySO : ScriptableObject
{
    public float maxEnergy = Mathf.Infinity;
    public float maxRechargeRate = Mathf.Infinity;
    public float maxOutputRate = Mathf.Infinity;
}