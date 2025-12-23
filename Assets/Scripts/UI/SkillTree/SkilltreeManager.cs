using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkilltreeManager : MonoBehaviour
{
    public PlayerStatManager statManager;
    public List<SkilltreeNode> allNodes = new List<SkilltreeNode>();

    public void Start()
    {
        statManager = FindObjectOfType<PlayerStatManager>();
    }
    public void UnlockNode(SkilltreeNode node) 
    {
        if (node.isUnlocked) {Debug.Log("Node is already unlocked!"); return;}
        if (!node.canBeUnlocked) {Debug.Log("Node cannot be unlocked yet!"); return;}
        if (statManager.GetSkillPoints() < 1) {Debug.Log("No skill points to spend!"); return;}
        if (node.branchType == BranchTypes.Survival)
        {
            statManager.AddStatModifier(new StatModifier(new Dictionary<BaseStatsEnum, float>{
                {BaseStatsEnum.maxStamina, 10},
                {BaseStatsEnum.maxHP, 10},
            }, StatModifierType.NodeBuff));
        } else if (node.branchType == BranchTypes.Popularity)
        {
            
        } else if (node.branchType == BranchTypes.Combat)
        {
            
        } else if (node.branchType == BranchTypes.None)
        {
            
        } else
        {
            Debug.Log($"A branch with the name {Enum.GetName(typeof(BranchTypes), node.branchType)} has not yet been implemented");
        }
        if (node.nodeType == NodeTypes.StatNode)
        {
            if (node.statModifiers.Count > 0)
            {
                statManager.AddStatModifier(node.statDict);
            }
        } else if (node.nodeType == NodeTypes.UnlockAbility)
        {
            
        } else if (node.nodeType == NodeTypes.UnlockPassive)
        {
            
        } else
        {
            Debug.Log($"A node type with the name {Enum.GetName(typeof(BranchTypes), node.nodeType)} has not yet been implemented");
        }
        
        statManager.stats.skillPoints -= 1;
        node.isUnlocked = true;
        foreach (SkilltreeNode connectedNode in node.connectedNodes)
        {
            connectedNode.canBeUnlocked = true;
        }
        Debug.Log($"{node.nodeName} successfully purchased at {Time.time}");
    }
}

