using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkilltreeManager : MonoBehaviour
{
    public PlayerStatManager statManager;
    public List<SkilltreeNode> allNodes = new List<SkilltreeNode>();
    public SkilltreeNode originNode; // make sure to assign this in inspector, or have it as the first element of allNodes
    private Camera cam;
    public void Start()
    {
        if (originNode == null) originNode = allNodes[0];
        cam = Camera.main;
        statManager = FindObjectOfType<PlayerStatManager>();
        FullInitTree(originNode);
    }
    public void UnlockNode(SkilltreeNode node) 
    {
        if (!node.almostCanBeUnlocked && !node.canBeUnlocked) {return;}
        if (node.isUnlocked) {Debug.Log("Node is already unlocked!"); AudioManager.Instance.PlayWrongBuzzerSFX(cam.transform); return;}
        if (!node.canBeUnlocked || node.almostCanBeUnlocked) {Debug.Log("Node cannot be unlocked yet!"); AudioManager.Instance.PlayWrongBuzzerSFX(cam.transform); return;}
        if (statManager.GetSkillPoints() < 1) {Debug.Log("No skill points to spend!"); AudioManager.Instance.PlayWrongBuzzerSFX(cam.transform); return;}
        
        
        // checks the branch type and applies relevant bonuses
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
        
        // checks the node type and applies relevant bonuses
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
        
        AudioManager.Instance.PlayBuyNodeSFX(cam.transform);
        statManager.stats.skillPoints -= 1;
        node.isUnlocked = true;
        node.UpdateNodeVisual();
        InitTree(node);
        Debug.Log($"{node.nodeName} successfully purchased at {Time.time}");
    }
    // recursively initializes the skill tree from a given node
    public void InitTree(SkilltreeNode node)
    {
        foreach (SkilltreeNode connectedNode in node.connectedNodes)
        {
            if (node.isUnlocked && connectedNode.isUnlocked == false)
            {
                connectedNode.canBeUnlocked = true;
                connectedNode.almostCanBeUnlocked = false;
            } else if (node.canBeUnlocked && connectedNode.isUnlocked == false)
            {
                connectedNode.almostCanBeUnlocked = true;
            } else
            {
                return;
            }
            connectedNode.UpdateNodeVisual();
            InitTree(connectedNode);
        }
    }

    public void FullInitTree(SkilltreeNode node)
    {
        foreach (SkilltreeNode connectedNode in node.connectedNodes)
        {
            if (node.isUnlocked && connectedNode.isUnlocked == false)
            {
                connectedNode.canBeUnlocked = true;
                connectedNode.almostCanBeUnlocked = false;
            } else if (node.canBeUnlocked && connectedNode.isUnlocked == false)
            {
                connectedNode.almostCanBeUnlocked = true;
            } 
            connectedNode.UpdateNodeVisual();
            FullInitTree(connectedNode);
        }
    }
}

