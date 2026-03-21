using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

[Serializable]
public class SkilltreeManager : MonoBehaviour, IBeginDragHandler, IDragHandler

{
    private PlayerStatManager statManager;
    private PlayerCombat playerCombat;
    private Canvas parentCanvas;
    public List<SkilltreeNode> allNodes = new List<SkilltreeNode>();
    public SkilltreeNode originNode; // make sure to assign this in inspector, or have it as the first element of allNodes
    public SkilltreeNode hoverNode;
    public SkillInfoHoverThingy skillInfoHoverThingy; // assign in inspector
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI skillPointsText;
    private Camera cam;
    public RectTransform skillTreeRoot;   
    private Vector2 lastMousePosition;
    public void Start()
    {
        if (originNode == null) originNode = allNodes[0];
        cam = Camera.main;
        statManager = FindObjectOfType<PlayerStatManager>();
        
        parentCanvas = GetComponentInParent<Canvas>();
        FullInitTree(originNode);
        RoundManager.Instance.AssignSkillTreeManager(this);
        if (skillTreeRoot == null) skillTreeRoot = GetComponent<RectTransform>();
        if (statManager)
        {
            statManager.AssignSkillTreeManager(this);
        }
        playerCombat = statManager.GetPlayerCombat();
    }
    public void Update()
    {
        if (statManager)
        {
            levelText.text = $"Level: {statManager.stats.level}";
            skillPointsText.text = $"Skill Points: {statManager.GetSkillPoints()}";
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (skillTreeRoot == null) return;

        Vector2 delta = eventData.position - lastMousePosition;
        skillTreeRoot.anchoredPosition += delta;
        lastMousePosition = eventData.position;
        ClampToCanvas();
    }

    private void ClampToCanvas()
    {
        Vector2 pos = skillTreeRoot.anchoredPosition;
 
        float contentWidth = skillTreeRoot.rect.width;
        float contentHeight = skillTreeRoot.rect.height;

        contentWidth -= 1920;
        contentHeight -= 1080;

        float xCoordinate = Mathf.Clamp(pos.x, -contentWidth/2 - 50, contentWidth/2 + 50);
        float yCoordinate = Mathf.Clamp(pos.y, -contentHeight/2 - 50, contentHeight/2 + 50);

        skillTreeRoot.anchoredPosition = new Vector2(xCoordinate, yCoordinate);
    }
    public void OnHoverNode(PointerEventData eventData, SkilltreeNode node, bool remove=false)
    {
        SkilltreeNode tempNode = node;
        if (remove)
        {
            if (hoverNode == tempNode) tempNode = null;
            if (hoverNode == null) tempNode = null;
        } else
        {
            tempNode = node;
            if (node.isUnlocked) tempNode = null;
            if (node.almostCanBeUnlocked == false && node.canBeUnlocked == false) tempNode = null;
        }
        
        if (tempNode != hoverNode)
        {
            hoverNode = tempNode;
            skillInfoHoverThingy.HoverNodeChange();
        }

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
            // statManager.AddStatModifier(new StatModifier(new Dictionary<BaseStatsEnum, float>{
            //     {BaseStatsEnum.maxStamina, 10},
            //     {BaseStatsEnum.maxHP, 10},
            // }, StatModifierType.NodeBuff));

            
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
            if (node.damageMultipliers.Count > 0)
            {
                foreach (DamageMultiplierLite dm in node.damageMultipliers)
                {
                    statManager.AddMultiplier(new DamageMultiplier(dm));
                }
            }
            if (node.resistances.Count > 0)
            {
                foreach (ResistanceEntry re in node.resistances)
                {
                    statManager.GetStats().resistances.AddEntry(new ResistanceEntry(re));
                }
            }
        } else if (node.nodeType == NodeTypes.UnlockAbility)
        {
            if (node.abilityUnlock != null)
            {
                playerCombat.UnlockAbility(node.abilityUnlock);
            }
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
                continue;
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
    public async void EnableAfterDelay(float delaySeconds)
    {
        await Task.Delay((int)(delaySeconds * 1000));
        if (parentCanvas == null) return;
        parentCanvas.enabled = true;
    }
    public async void DisableAfterDelay(float delaySeconds)
    {
        await Task.Delay((int)(delaySeconds * 1000));
        if (parentCanvas == null) return; 
        parentCanvas.enabled = false;
    }
}

