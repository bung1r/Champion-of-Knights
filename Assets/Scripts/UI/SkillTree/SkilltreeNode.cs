using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SkilltreeNode : MonoBehaviour
{
    public SkilltreeManager skilltreeManager;
    public string nodeName;
    public NodeTypes nodeType = NodeTypes.None;
    public BranchTypes branchType = BranchTypes.None;
    public List<SkilltreeNode> connectedNodes = new List<SkilltreeNode>();  
    public bool isUnlocked = false;
    public bool canBeUnlocked = false;
    public AbilityBase abilityUnlock;
    public List<StatModifierList> statModifiers = new List<StatModifierList>();
    [NonSerialized] public StatModifier statDict;
    void Start()
    {
        if (nodeType == NodeTypes.StatNode && statModifiers.Count > 0)
        {
            statDict = new StatModifier(statModifiers);
            statDict.statModifierType = StatModifierType.NodeBuff;
        }
    }
    public void OnClickNode()
    {
        skilltreeManager.UnlockNode(this);
    }

}

public enum NodeTypes
{
    None, UnlockAbility, UnlockPassive, StatNode
}

public enum BranchTypes
{
    None, Survival, Combat, Popularity
}

