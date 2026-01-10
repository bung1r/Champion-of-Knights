using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class StatsUIManager : MonoBehaviour
{
    private Canvas parentCanvas;
    [HideInInspector] public PlayerStatManager statManager;
    public StyleHUDManager styleHUDManager; // must be assigned
    public InventoryManagerGUI inventoryManagerGUI; // must be assigned
    public Canvas escMenuCanvas;
    public AbilityUIManager abilityUIManager;
    public StatsUIBar healthBar; //must be assigned
    public StatsUIBar stamBar; //must be assigned
    public StatsUIBar styleBar; //must be assigned
    public StatsUIBar expBar; //must be assigned
    public TextMeshProUGUI levelText; // must be assigned
    public TextMeshProUGUI fToInteract; //assign ts

    public void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
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
    public void UpdateStyle(float currStyle, float maxStyle, float totalStyle, float viewers, float styleLevel, float reputation, float corruption)
    {
        styleBar.UpdateBar(currStyle, maxStyle);
        styleHUDManager.UpdateText(totalStyle, viewers, styleLevel, reputation, corruption);
    }
    
    public void UpdateEXP(float currEXP, float maxEXP, int level)
    {
        expBar.UpdateBar(currEXP, maxEXP);
        levelText.text = $"Level: {level}";
        // inventoryManagerGUI.UpdateLevelText(level);
    }
    public void AddStyleEntry(StyleBonusTypes bonusType, int mult = 1)
    {
        styleHUDManager.AddEntry(bonusType, mult);
    }
    public void ShowInteractPrompt(string flavorText = "F to interact")
    {
        fToInteract.text = flavorText;
        fToInteract.gameObject.SetActive(true);
    }
    public void HideInteractPrompt()
    {
        fToInteract.gameObject.SetActive(false);
    }
    public StyleBonusDatabase GetDatabase() => styleHUDManager.bonusDatabase;

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
    public void ToggleEscMenu()
    {
        escMenuCanvas.enabled = !escMenuCanvas.enabled;
    }   
}
