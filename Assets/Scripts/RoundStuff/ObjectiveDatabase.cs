using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RoundManager/ObjectiveDatabase")]
public class ObjectiveDatabase : ScriptableObject
{
    public List<ObjectiveScaling> objectiveScalings = new List<ObjectiveScaling>();
    public Objective GetObjective(ObjectiveTypes type)
    {
        foreach (ObjectiveScaling objScale in objectiveScalings)
        {
            if (objScale.objectiveType == type)
            {
                return objScale.CalculateObjective();
            }
        }
        Debug.LogError("Objective type " + type + " not found in database!");
        return null;
    }
    public ObjectiveScaling GetObjectiveFoundation(ObjectiveTypes type)
    {
        foreach (ObjectiveScaling objScale in objectiveScalings)
        {
            if (objScale.objectiveType == type)
            {
                return objScale;
            }
        }
        Debug.LogError("Objective type " + type + " not found in database!");
        return null;
    }
 
}