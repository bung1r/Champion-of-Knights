using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageData
{
    [NonSerialized] public AbilityBase abilityBase;
    public float baseDamage;
    public DamageType type;
    public GameObject source;
    public DamageData(){}
    public DamageData(AbilityBase data)
    {
        abilityBase = data;
        type = data.damageData.type;
        baseDamage = data.damageData.baseDamage;
        source = data.damageData.source;
    }
    public DamageData(DamageData data)
    {
        abilityBase = data.abilityBase;
        type = data.type;
        baseDamage = data.baseDamage;
        source = data.source;
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
    Physical, Magical, Poison, Electric, Crushing, Mental, Ethereal, All, Fixed
}

public enum DamageMultiplierTypes
{
    Additive, Multiplicative
}