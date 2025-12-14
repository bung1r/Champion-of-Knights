using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ChargedRangedRuntime : RangedRuntime
{
    ChargedAbilityStats chargedStats;
    public void ConstructChargedMelee(ChargedRangedAbility other, StatManager manager)
    {
        hitboxData = new RangedHitboxData(other.hitboxData);
        owner = manager.gameObject;
        chargedStats = new ChargedAbilityStats(other.chargedStats);
    }
    public ChargedRangedRuntime() {}
    public ChargedRangedRuntime(ChargedRangedAbility other, StatManager manager)
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
