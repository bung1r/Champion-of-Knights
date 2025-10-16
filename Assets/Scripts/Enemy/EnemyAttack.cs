using System.Collections.Generic;
using System.Data.SqlTypes;

using UnityEngine;

[CreateAssetMenu(menuName = "AI/Behavior Set")]
public class AIBehaviorSet : ScriptableObject
{
    public List<AIActionPriority> behaviors;
}

[System.Serializable]
public class AIActionPriority
{
    public WeaponAction action;
    public float priority;
    public float minPriority = 0;
    public float maxPriority = 9999;
}