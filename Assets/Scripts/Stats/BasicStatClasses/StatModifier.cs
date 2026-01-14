using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
public class StatModifier 
{
    public StatModifierType statModifierType = StatModifierType.TempBuff;
    public ModifierTypes generalModifierType = ModifierTypes.Additive;
    public float duration;
    public List<StatModifierList> statList = new List<StatModifierList>();
    [HideInInspector] public float timeCreated;
    public Dictionary<BaseStatsEnum, float> statDict = new Dictionary<BaseStatsEnum, float>
        {
            
            // {BaseStatsEnum.baseEXP, 0},
            // {BaseStatsEnum.maxHP, 0},
            // {BaseStatsEnum.sprintSpeed, 0},
            // {BaseStatsEnum.staminaRegen, 0},
            // {BaseStatsEnum.walkSpeed, 0},
            // {BaseStatsEnum.sprintStaminaCost, 0},
            // {BaseStatsEnum.maxStamina, 0}
        };

    public void EditModifier(StatModifier modifyData)
    {
        if (modifyData.generalModifierType == ModifierTypes.Additive)
        {

            foreach (var entry in modifyData.statDict)
            {
                if (statDict.ContainsKey(entry.Key)) {
                    statDict[entry.Key] += entry.Value;
                } else
                {
                    statDict.Add(entry.Key, entry.Value);
                }
            }
            
        } 
        
    }
    public StatModifier() {

    }
    public StatModifier(StatModifier other)
    {
        statDict = other.statList.ToDictionary(stat => stat.baseStatsEnum, stat => stat.value);
        statModifierType = other.statModifierType;
        generalModifierType = other.generalModifierType;
        duration = other.duration;
        timeCreated = Time.time;
    }
    public StatModifier(List<StatModifierList> statModifierList)
    {
        timeCreated = Time.time;
        foreach (StatModifierList statModifierList1 in statModifierList)
        {
            statDict.Add(statModifierList1.baseStatsEnum, statModifierList1.value);
        }
    }
    public StatModifier(Dictionary<BaseStatsEnum, float> otherStatDict)
    {
        statDict = otherStatDict;
    }

    public StatModifier(Dictionary<BaseStatsEnum, float> otherStatDict, StatModifierType otherStatModifierType)
    {
        statDict = otherStatDict;
        statModifierType = otherStatModifierType;
    }

}

public enum StatModifierType
{
    TempBuff, NodeBuff
}

[Serializable]
public class StatModifierList
{
    public BaseStatsEnum baseStatsEnum;
    public float value;
}