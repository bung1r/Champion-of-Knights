using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageData
{
    public float baseDamage;
    public DamageType type;
    public GameObject source;
    public DamageData(){}
    public DamageData(DamageData data)
    {
        type = data.type;
        baseDamage = data.baseDamage;
    }
}
public class DamageMultiplier
{
    public DamageMultiplierTypes type = DamageMultiplierTypes.Additive;
    public float amount = 0f;
    public float lifeTime = Mathf.Infinity;
    public float timeCreated;
    public DamageMultiplier()
    {
        timeCreated = Time.time;
    }
    public DamageMultiplier(DamageMultiplier other)
    {
        type = other.type;
        amount = other.amount;
        lifeTime = other.lifeTime;
        timeCreated = Time.time;
    }
}
public enum DamageType
{
    Physical, Magical, Poison, Electric, Crushing, Mental, Ethereal
}

public enum DamageMultiplierTypes
{
    Additive, Multiplicative
}