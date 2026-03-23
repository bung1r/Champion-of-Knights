using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

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
    public Transform followMouse; //must be assigned
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
    public Coroutine InteractCoroutine;
    public void ShowInteractPrompt(string flavorText = "F to interact")
    {
        if (flavorText == fToInteract.text) return;
        if (InteractCoroutine != null)
        {
            StopCoroutine(InteractCoroutine);
            InteractCoroutine = null;
        }


        InteractCoroutine = StartCoroutine(HandleInteractVisibility(flavorText));
        
    }
    public IEnumerator HandleInteractVisibility(string flavorText, float time=1.5f)
    {
        fToInteract.gameObject.SetActive(true);
        fToInteract.text = flavorText;
        fToInteract.alpha = 1f;
        yield return new WaitForSeconds(time);
        float fadeDuration = 0.5f;
        while (fToInteract.alpha > 0f)
        {
            yield return null;
            fToInteract.alpha -= 1/fadeDuration * Time.deltaTime;
        }
        fToInteract.alpha = 0f;
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
