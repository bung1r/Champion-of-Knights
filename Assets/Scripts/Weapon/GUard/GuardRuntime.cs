using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class GuardRuntime : AbilityRuntime
{
    public GuardStats guardStats;
    public GameObject owner;
    public ResistanceEntry currentEntry;
    private Stats stats;
    private int guardNum = 0;
    public void ConstructGuard(GuardAbility other, StatManager manager)
    {
        owner = manager.gameObject;
        guardStats = new GuardStats(other.guardStats);
    }
    public GuardRuntime() {}
    public GuardRuntime(GuardAbility other, StatManager manager)
    {
        ConstructBase(other, manager);
        ConstructGuard(other, manager);
    }

    public override bool Use()
    {
        return false;
    }
    public override bool CanUse()
    {
        if (stats == null) stats = statManager.GetStats();
        if (!stats.inAttackAnim && !stats.isGuarding)
        {
            return true;
        }
        return false;
     }
    public override void BeginUse()
    {
        if (!CanUse()) return;
        AudioManager.Instance.PlayGuardSFX(owner.transform);
        currentEntry = new ResistanceEntry(guardStats);
        stats.resistances.AddEntry(currentEntry);
        stats.isGuarding = true;
        guardNum += 1;
        HandleParry();
    }
    public override void WhileUse()
    {
        
    }
    public override void EndUse()
    {
        stats.resistances.RemoveEntry(currentEntry);
        stats.isGuarding = false;
        stats.isParrying = false;
    }
    // public IEnumerator HandleParryRoutine()
    // {
    //     yield return new WaitForSeconds()
    // }
    async public void HandleParry()
    {
        float thisGuardNum = guardNum;
        await Task.Delay((int)(guardStats.parryStart * 1000));
        if (thisGuardNum != guardNum) return;
        stats.isParrying = true;
        await Task.Delay((int)((guardStats.parryEnd - guardStats.parryStart) * 1000));
        if (thisGuardNum == guardNum && stats.isParrying) stats.isParrying = false;
    }
}
