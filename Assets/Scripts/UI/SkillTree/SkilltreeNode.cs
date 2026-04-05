using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[Serializable]
public class SkilltreeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkilltreeManager skilltreeManager;
    public string nodeName;
    public NodeTypes nodeType = NodeTypes.None;
    public BranchTypes branchType = BranchTypes.None;
    public List<SkilltreeNode> connectedNodes = new List<SkilltreeNode>();  
    public string desc;
    public int cost = 1;
    public bool isUnlocked = false;
    public bool canBeUnlocked = false;
    public bool almostCanBeUnlocked = false;
    public AbilityBase abilityUnlock;
    public PassiveAbilityBase passiveUnlock;
    public List<StatModifierList> statModifiers = new List<StatModifierList>();
    public List<DamageMultiplierLite> damageMultipliers = new List<DamageMultiplierLite>();
    public List<ResistanceEntry> resistances = new List<ResistanceEntry>();
    public bool trueUnlockable = true;
    public int corruptionCost = 0; // very niche, only for the popularity path, but it was easier to just add it here than make a whole new node type for it.
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        skilltreeManager.OnHoverNode(eventData, this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        skilltreeManager.OnHoverNode(eventData, this, true);
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

