using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class StatsUIManager : MonoBehaviour
{
    [HideInInspector] public PlayerStatManager statManager;
    public StyleHUDManager styleHUDManager; // must be assigned
    public InventoryManagerGUI inventoryManagerGUI; // must be assigned
    public AbilityUIManager abilityUIManager;
    public StatsUIBar healthBar; //must be assigned
    public StatsUIBar stamBar; //must be assigned
    public StatsUIBar styleBar; //must be assigned
    public void Start()
    {
        statManager = FindObjectOfType<PlayerStatManager>();
        statManager.AssignUIManager(this);
        inventoryManagerGUI.AssignUIManager(statManager);  
        abilityUIManager.AssignPlayerCombat(statManager.GetPlayerCombat());
        RoundManager.Instance.AssignStatUIManager(this);
    }
    public void UpdateHP(float currHP, float maxHP)
    {
        healthBar.UpdateBar(currHP, maxHP);
    }
    public void UpdateStam(float currStam, float maxStam)
    {
        stamBar.UpdateBar(currStam, maxStam);
    }
    public void UpdateStyle(float currStyle, float maxStyle, float totalStyle, float viewers, float styleLevel)
    {
        styleBar.UpdateBar(currStyle, maxStyle);
        styleHUDManager.UpdateText(totalStyle, viewers, styleLevel);
    }
    public void AddStyleEntry(StyleBonusTypes bonusType)
    {
        styleHUDManager.AddEntry(bonusType);
    }
    public StyleBonusDatabase GetDatabase() => styleHUDManager.bonusDatabase;
}
