using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DamageData
{
    public float baseDamage;
    public DamageType type;
    public GameObject source;
}

public enum DamageType
{
    Physical, Magical, Poison, Holy, Crushing, Mental,
}
