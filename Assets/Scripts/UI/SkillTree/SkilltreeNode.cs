using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    public bool almostCanBeUnlocked = false;
    public AbilityBase abilityUnlock;
    public List<StatModifierList> statModifiers = new List<StatModifierList>();
    private TextMeshProUGUI nodeText;
    private Button nodeButton;
    [NonSerialized] public StatModifier statDict;
    void Awake()
    {
        nodeText = GetComponentInChildren<TextMeshProUGUI>();
        nodeButton = GetComponent<Button>();
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

    public void UpdateNodeVisual()
    {
        if (nodeButton == null) nodeButton = GetComponent<Button>();
        ColorBlock colors = nodeButton.colors;
        nodeText.text = nodeName;
        if (isUnlocked)
        {
            colors.normalColor = Color.white;
            colors.highlightedColor = Color.white;
            colors.pressedColor = Color.white;
            nodeText.color = Color.white;
        } else if (canBeUnlocked)
        {
            colors.normalColor = new Color(0.5f,0.5f,0.5f,0.9f);
            colors.highlightedColor = new Color(1f,1f,1f,0.9f);
            colors.pressedColor = new Color(0.8f,0.8f,0.8f,0.9f);
            nodeText.color = Color.gray;
        } else if (almostCanBeUnlocked)
        {
            colors.normalColor = new Color(0.5f,0.5f,0.5f,0.5f);
            colors.highlightedColor = new Color(1f,1f,1f,0.5f);
            nodeText.color = new Color(0.5f,0.5f,0.5f,0.5f);
            nodeText.text = "???";
        } else
        {
            colors.normalColor = new Color(0f,0f,0f,0f);
            colors.highlightedColor = new Color(0f,0f,0f,0f);
            colors.pressedColor = new Color(0f,0f,0f,0f);
            nodeText.color = new Color(0f,0f,0f,0f);
            nodeText.text = "???";
        }
        nodeButton.colors = colors;
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

