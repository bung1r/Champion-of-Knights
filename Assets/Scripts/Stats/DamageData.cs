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

public enum DamageType
{
    Physical, Magical, Poison, Electric, Crushing, Mental, Ethereal
}
