using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ChargedMeleeRuntime : MeleeRuntime
{
    ChargedAbilityStats chargedStats;
    public void ConstructChargedMelee(ChargedMeleeAbility other, StatManager manager)
    {
        hitboxData = new MeleeHitboxData(other.hitboxData);
        owner = manager.gameObject;
        chargedStats = new ChargedAbilityStats(other.chargedStats);
    }
    public ChargedMeleeRuntime() {}
    public ChargedMeleeRuntime(ChargedMeleeAbility other, StatManager manager)
    {
        ConstructBase(other, manager);
        ConstructChargedMelee(other, manager);
    }

     public override bool Use()
    {
        return false;
    }
    public override void BeginUse()
    {
        if (!CanUse()) return;
        ChargedHelper.BeginUse(this, chargedStats);
        statManager.BeginAttack();
    }
    public override void WhileUse()
    {
       ChargedHelper.WhileUse(this, chargedStats);
    }
    public override void EndUse()
    {
        // basic checking
        ChargedHelper.EndUse(this, chargedStats);
        statManager.EndAttack();
    }
}
