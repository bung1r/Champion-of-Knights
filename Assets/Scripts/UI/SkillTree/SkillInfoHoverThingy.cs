using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoHoverThingy : MonoBehaviour
{
    public SkilltreeManager skilltreeManager;
    public SkilltreeNode prevNode;
    public RectTransform rectTransform;
    public TextMeshProUGUI nameText; // assign in inspector
    public TextMeshProUGUI descText; // assign in inspector
    public TextMeshProUGUI costText; // assign in inspector
    public IEnumerator BeginExpansion(SkilltreeNode node)
    {
        rectTransform.anchoredPosition = node.transform.localPosition + new Vector3(11f, 20f);
        rectTransform.localScale = new Vector3(0f,0f,1f);
        descText.text = "";

        if (node.almostCanBeUnlocked == false)
        {
            nameText.text = node.nodeName;
            costText.text = "Cost: 1 Skill Point";
            // make the description automatically
            if (node.desc == "")
            {
                List<string> stringList = new List<string>();
                foreach (var damageMultiplier in node.damageMultipliers)
                {
                    string coolString = $"a {damageMultiplier.amount * 100}% Damage Buff";
                    stringList.Add(coolString);
                }

                foreach (var statModifier in node.statModifiers)
                {
                    string coolString = null;
                    if (statModifier.baseStatsEnum == BaseStatsEnum.maxHP)
                    {
                        coolString = $"{statModifier.value} Max HP";
                    } else if (statModifier.baseStatsEnum == BaseStatsEnum.sprintSpeed)
                    {
                        coolString = $"{statModifier.value} Sprint Speed";
                    } else if (statModifier.baseStatsEnum == BaseStatsEnum.staminaRegen)
                    {
                        coolString = $"{statModifier.value} HP Regen/sec";
                    }else if (statModifier.baseStatsEnum == BaseStatsEnum.regenHP)
                    {
                        coolString = $"{statModifier.value} HP Regen/sec";
                    } else if (statModifier.baseStatsEnum == BaseStatsEnum.walkSpeed)
                    {
                        coolString = $"{statModifier.value} Walk Speed";
                    } else if (statModifier.baseStatsEnum == BaseStatsEnum.maxStamina)
                    {
                        coolString = $"{statModifier.value} Max Stamina";
                    } 


                    if (coolString != null) stringList.Add(coolString);
                }
        
                if (stringList.Count > 0)
                {
                    string fullString = "Desc: Gain ";
                    int strIndex = 0;
                    foreach (string str in stringList)
                    {
                        fullString += str;
                        if (strIndex == stringList.Count - 1)
                        {
                            fullString += ".";
                        } else if (strIndex == stringList.Count - 2) {
                            if (stringList.Count > 2)
                            {
                                fullString += ", and ";
                            } else
                            {
                                fullString += " and ";
                            }
                        } else
                        {
                            fullString += ", ";
                        }
    
                        strIndex++;
                    }
                    node.desc = fullString;
                    descText.text = node.desc;
                } else
                {
                    node.desc = "Desc: None";
                    descText.text = node.desc;
                }
            } else
            {
                descText.text = node.desc;
            }
        } else
        {
            nameText.text = "???";
            descText.text = "Desc: ???";
            costText.text = "Cost: ???";
        }
        
        
        while (rectTransform.localScale.x < 0.99f)
        {
            yield return null;
            float temp = Mathf.Lerp(rectTransform.localScale.x, 1f, 0.3f);
            rectTransform.localScale = new Vector3(temp,temp,0f);
        }
        rectTransform.localScale = new Vector3(1f,1f,1f);
    }
    public IEnumerator ReverseExpansion()
    {
        while (rectTransform.localScale.x > 0.01f)
        {
            yield return null;
            float temp = Mathf.Lerp(rectTransform.localScale.x, 0f, 0.3f);
            rectTransform.localScale = new Vector3(temp,temp,0f);
        }
        rectTransform.localScale = new Vector3(0f,0f,1f);
    }
    public Coroutine ExpansionCoroutine;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        skilltreeManager = GetComponentInParent<SkilltreeManager>();
    }
    void Start()
    {
        rectTransform.localScale = new Vector3(0f,0f,1f);
    }
    public void HoverNodeChange()
    {
        if (ExpansionCoroutine != null) {
            StopCoroutine(ExpansionCoroutine);
            ExpansionCoroutine = null;
        }

        SkilltreeNode node = skilltreeManager.hoverNode;
        if (node == null)
        {
            ExpansionCoroutine = StartCoroutine(ReverseExpansion());
        } else
        {
            ExpansionCoroutine = StartCoroutine(BeginExpansion(node));
        }


        prevNode = node;
   }

}