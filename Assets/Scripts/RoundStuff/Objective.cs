using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectiveScaling
{
    public ObjectiveTypes objectiveType;
    public int baseAmount = 0;
    public float roundAddFactor = 0f;
    public float roundMultFactor = 1f;
    public float roundExpoFactor = 1f; // should never be needed, but you never know 
    public float max = 1000000f;
    public float min = 0f;
    public float variation = 0.1f;
    public string objectiveDescription = "Description here."; 
    public int roundTo = 1; // any multiple of 10. roundTo = 100 means 3842 turns into 3800
    public bool oneTimeComplete = true; // for objectives that only need to be done once, like ViewersOneTime
    public Objective CalculateObjective()
    {
        float amount = baseAmount + (roundAddFactor * RoundManager.Instance.currentRound);
        amount *= 1 + Math.Max((roundMultFactor - 1f) * RoundManager.Instance.currentRound, 0f);
        amount = Mathf.Pow(amount, roundExpoFactor);
        amount *= UnityEngine.Random.Range(1 - variation, 1 + variation);
        amount = Mathf.Floor(amount/roundTo) * roundTo;
        amount = Mathf.Clamp(amount, min, max);
        return new Objective(this, objectiveType, (int)amount);
    }
}

[Serializable]
public class Objective
{
    public ObjectiveTypes objectiveType;
    public int targetAmount;
    public int currentAmount;
    public bool isOneTime = false;
    public Objective(ObjectiveScaling objScale, ObjectiveTypes type, int target)
    {
        objectiveType = type;
        targetAmount = target;
        currentAmount = 0;
        isOneTime = objScale.oneTimeComplete;
    }
    public string UIString(string foundationString)
    {
        if (objectiveType == ObjectiveTypes.StyleLevel)
        {
            StyleGrades rarg1 = (StyleGrades)targetAmount;
            StyleGrades rarg2 = (StyleGrades)currentAmount;
            string updatedString = string.Format(foundationString, rarg1);
            updatedString = string.Concat(updatedString, $" {rarg2}/{rarg1}");
            if (IsComplete()) updatedString = "<s>" + updatedString + "</s>";
            return updatedString;
        } else
        {
            string updatedString = string.Format(foundationString, targetAmount);
            updatedString = string.Concat(updatedString, $" {currentAmount}/{targetAmount}");
            if (IsComplete()) updatedString = "<s>" + updatedString + "</s>";
            return updatedString;
        }
    }

    public bool IsComplete()
    {
        return isOneTime && currentAmount >= targetAmount;
    }
}

[Serializable]
public enum ObjectiveTypes
{
    Collect, Kill, Parry, Viewers, ViewersOneTime, Multikill, StyleLevel
}