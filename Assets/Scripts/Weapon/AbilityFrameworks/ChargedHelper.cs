using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;
[Serializable]
public static class ChargedHelper
{
    public static void BeginUse(AbilityRuntime runtime, ChargedAbilityStats chargedStats)
    {
        StatManager statManager = runtime.statManager;
        if (!runtime.CanUse()) return;
        if (chargedStats.consumeStamAtBeginning) statManager.UseStamina(runtime.staminaCost);
        chargedStats.isCharging = true;
        chargedStats.startedCharging = Time.time;
    }

    public static void WhileUse(AbilityRuntime runtime, ChargedAbilityStats chargedStats)
    {
        StatManager statManager = runtime.statManager;
        // basic checking
        if (!chargedStats.isCharging) return;
        if (!statManager.CanUseStamina(chargedStats.rampCost * Time.deltaTime)) runtime.EndUse();
        //checks whether or not stamina should be used depending on we are still charging.
        if (Time.time - chargedStats.startedCharging < chargedStats.maxRampTime)
        {
            statManager.UseStamina(chargedStats.rampCost * Time.deltaTime);
        } else
        {
            if (chargedStats.fireWhenMaxed) runtime.EndUse();
        }


    }

    public static void EndUse(AbilityRuntime runtime, ChargedAbilityStats chargedStats)
    {
        StatManager statManager = runtime.statManager;
        // basic checking
        if (!chargedStats.isCharging) return;

        // preliminary variables for calculation
        float timeCharged = Math.Min(Time.time - chargedStats.startedCharging, chargedStats.maxRampTime);
        float percent = timeCharged/chargedStats.maxRampTime; 
        DamageMultiplier multiplier = new DamageMultiplier();
            multiplier.type = DamageMultiplierTypes.Multiplicative;
            multiplier.lifeTime = 0f; // no lifetime means it's immedaitely destroyed.
    
        if (percent>=1)
        {
            // if percentage is 100%, straight up set it to the max multiplier
            multiplier.amount = chargedStats.maxDmgMultiplier;
        } else if (chargedStats.linearScaling)
        {
            // if the scaling is linear, set the amount of the multiplier to calculated amounts
            multiplier.amount = (percent * (chargedStats.dmgMultiplier - 1)) + 1;
        } else
        {
            multiplier.amount = 1f;
        }
       
        // add the multiplier
        statManager.AddMultiplier(multiplier);


        // perform the action 
        runtime.Perform();

        // finalize stuff
        chargedStats.isCharging = false;
        chargedStats.percentCharged = 0f;
        runtime.lastUsedTime = Time.time;
    }
}

[Serializable]
public class ChargedAbilityStats
{
    public bool consumeStamAtBeginning = true; // consume stamina at the very beginning (normal)
    public float rampCost = 0f; // cost per second as you charge
    public float maxRampTime = 5f; // stop ramping up after this time
    public float dmgMultiplier = 3f; // dmgMultiplier at we approach max.
    public float maxDmgMultiplier = 5f; // dmgMultiplier at max. Good things come to those who wait. 
    public bool linearScaling = true; // basically means "If you hold it for a medium amount, will it still deal more damage?"
    public bool fireWhenMaxed = false; // when maxed charge, immediately do it. 
    [NonSerialized] public float percentCharged = 0f;
    [NonSerialized] public bool isCharging = false;
    [NonSerialized] public float startedCharging = -999f;
    public ChargedAbilityStats() {}
    public ChargedAbilityStats(ChargedAbilityStats other)
    {
        consumeStamAtBeginning = other.consumeStamAtBeginning;
        rampCost = other.rampCost;
        maxRampTime = other.maxRampTime;
        dmgMultiplier = other.dmgMultiplier;
        linearScaling = other.linearScaling;
        fireWhenMaxed = other.fireWhenMaxed;
        maxDmgMultiplier = other.maxDmgMultiplier;
    }
}


