using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class PlayerStatManager : StatManager
{
    public PlayerStatsSO statsSO;
    
    public PlayerStats stats;
    private StyleBonusDatabase bonusDatabase;
    private PlayerCombat playerCombat;
    private PlayerMovement playerMovement;
    private float lastHP = 0f;
    private float lastMaxHP = 0f;
    private float lastStam = 0f;
    private float lastEXP = -99f;
    private int lastLevel = -1;
    private int numParriesThisFrame = 0;
    private int numEnemiesKilledThisFrame = 0;
    private StatsUIManager statsUIManager; 
    private SkilltreeManager skilltreeManager;
    void Awake()
    {
        playerCombat = GetComponent<PlayerCombat>();
        playerMovement = GetComponent<PlayerMovement>();
        stats = statsSO.CreateRuntime();
        setPlayerStats(stats);
        AdjustLevels();
        AwakeAdjustStyle();
    }   
    public void AssignUIManager(StatsUIManager uiManager)
    {
        statsUIManager = uiManager;
        bonusDatabase = statsUIManager.GetDatabase();
    }
    public void AssignSkillTreeManager(SkilltreeManager stm)
    {
        skilltreeManager = stm;
    }
    public void AddEXP(float exp)
    {
        stats.totalEXP += exp;
        if (stats.nextLevelEXP - exp <= 0)
        {
            AdjustLevels();
        } else
        {
            stats.nextLevelEXP -= exp;
            stats.currentEXP += exp;
        }
    }
    private float GetLevel(float currentLevel)
    {
        float temp = currentLevel;
        while (LevelToEXP(temp + 1) <= stats.totalEXP)
        {
            temp += 1;
            stats.skillPoints += 1;
        } 
        return temp;
    }
    private float LevelToEXP(float level)
    {   
        return 100 * (level * (level + 1) / 2);
    }
    public void AdjustLevels()
    {
        stats.level = GetLevel(stats.level);
        stats.nextLevelEXP = LevelToEXP(stats.level + 1) - stats.totalEXP;
        stats.currentEXP =  stats.totalEXP - LevelToEXP(stats.level);
    }
    public float GetSkillPoints() => stats.skillPoints;
    public void AddStyle(float amount)
    {
        stats.totalStyle += amount;
        stats.totalStyle = Mathf.Max(stats.totalStyle, 0); // can't go below 0, bro~!
        if (stats.totalStyle == 0) {
            amount = 0;
        }
        if (stats.currentStyle > stats.maxStyle)
        {
            AdjustStyle();
        }
        else if (stats.currentStyle < 0)
        {
            AdjustStyle();
        } 
        else
        {
            stats.currentStyle += amount;
        }
    }
    public void AwakeAdjustStyle()
    {
        stats.styleLevel = GetStyleLevel(stats.styleLevel);
        stats.currentStyle = stats.totalStyle - StyleLevelToStyle(stats.styleLevel);
        stats.maxStyle = StyleLevelToStyleRelative(stats.styleLevel + 1);
    }
    public void AdjustStyle()
    {
        stats.styleLevel = GetStyleLevel(stats.styleLevel);
        stats.currentStyle = stats.totalStyle - StyleLevelToStyle(stats.styleLevel);
        stats.maxStyle = StyleLevelToStyleRelative(stats.styleLevel + 1);
        AudioManager.Instance.PlayStyleMeterUpSFX(stats.styleLevel, transform);
    }
    public float GetStyleLevel(float currentLevel)
    {
        float temp = currentLevel;
        if (stats.currentStyle > stats.maxStyle)
        {
            while (StyleLevelToStyle(temp + 1) <= stats.totalStyle)
            {
                temp += 1;
            } 
        } else if (stats.currentStyle < 0)
        {
            while (StyleLevelToStyle(temp) > stats.totalStyle)
            {
                temp -= 1;
            } 
        }
        
        
        return temp;
    }
    
    public float StyleLevelToStyle(float level)
    {
        return (10 * Mathf.Pow(level, 2)) + (90 * level);
    }
    public float StyleLevelToStyleRelative(float level)
    {
        if (level == 0) return 0f;
        return 20f * level + 80f;
    }
    public void HandleStyleBonus(StyleBonusTypes bonusType, int mult = 1)
    {
        StyleBonus bonus = bonusDatabase.Get(bonusType);
        statsUIManager.AddStyleEntry(bonusType, mult);
        AddStyle(bonus.style * mult);
    }
    public void StyleUpdate()
    {
        if (RoundManager.Instance.currentRoundState != RoundStates.Active) {
            return;
        }
        // Basically, goes down more the higher the styleLevel. 
        AddStyle(-1 * (Mathf.Pow((stats.styleLevel + 1) * 6, 0.75f) * Time.deltaTime));
        
    } 
    public void CalculateViewers()
    {
        if (RoundManager.Instance.currentRoundState != RoundStates.Active) return;
        float viewerGainFactor = 10f * (1.5f + ((stats.reputation - (stats.corruption/2f)) / 100f)); // reputation affects viewer gain
        float viewerLossFactor = 10f * (1.5f + stats.corruption / 100f); // corruption affects viewer loss
        float idealStyle = Mathf.Sqrt(RoundManager.Instance.currentRound); // the 'stable' level, basically where you don't gain or lose viewers
        float fullStyleLevel = stats.styleLevel + stats.currentStyle / stats.maxStyle; // stylelevel with the fraction added on

        // maxixium possible viewers at that level
        float maxViewersPerLevel = fullStyleLevel * viewerGainFactor * 20f + 100f;
        maxViewersPerLevel += stats.loyalViewers * 2f;

        float diff = fullStyleLevel - idealStyle;
        float viewerGain = Mathf.Sign(diff) * Mathf.Sqrt(Mathf.Abs(diff));

        // slow down viewer gain as you approach max viewers
        if (viewerGain > 0 && stats.viewers + viewerGain > maxViewersPerLevel - 150f)
        {
            viewerGain = Mathf.Max((maxViewersPerLevel - stats.viewers)/50f, 0f);
        } 

        if (viewerGain>=0) viewerGain *= viewerGainFactor;
        else viewerGain *= viewerLossFactor;

        stats.viewers += viewerGain * Time.deltaTime;
        stats.viewers = Mathf.Max(stats.viewers, stats.loyalViewers);
        // stats.viewers = Mathf.Max(stats.viewers , 0f); 
    }
    public void UIHandler()
    {
        if (statsUIManager == null) return;
        if (lastHP != stats.currentHP || lastMaxHP != stats.maxHP)
        {
            statsUIManager.UpdateHP(stats.currentHP, stats.maxHP);
            lastHP = stats.currentHP;
        }
        if (lastStam != stats.currentStamina)
        {
            statsUIManager.UpdateStam(stats.currentStamina, stats.maxStamina);
            lastStam = stats.currentStamina;
        }
        if (lastEXP != stats.currentEXP || lastLevel != (int)stats.level)
        {
            statsUIManager.UpdateEXP(stats.currentEXP, stats.nextLevelEXP + stats.currentEXP, (int)stats.level);
            lastEXP = stats.currentEXP;
            lastLevel = (int)stats.level;
        }

        statsUIManager.UpdateStyle(stats.currentStyle, stats.maxStyle, stats.totalStyle, stats.viewers, stats.styleLevel, stats.reputation, stats.corruption);
    }
    public void ResetRoundStats()
    {
        stats.viewers = 0f;
        stats.styleLevel = 0f;
        stats.currentStyle = 0f;
        stats.totalStyle = 0f;
        statsUIManager.UpdateStyle(stats.currentStyle, stats.maxStyle, stats.totalStyle, stats.viewers, stats.styleLevel, stats.reputation, stats.corruption);
    }
    public override void PostUpdate()
    {
        UIHandler();
        CalculateViewers();
        StyleUpdate();
    }
    public InventoryManagerGUI GetInventoryManagerGUI() => statsUIManager.inventoryManagerGUI;
    public StatsUIManager GetStatsUIManager() => statsUIManager;
    public PlayerCombat GetPlayerCombat() => playerCombat;
    public SkilltreeManager GetSkilltreeManager() => skilltreeManager;
    public PlayerMovement GetPlayerMovement() => playerMovement;  
    public void OnParry()
    {
        AudioManager.Instance.PlayParrySFX(transform);
        RoundManager.Instance.OnParry();
        numParriesThisFrame += 1;
    }
    public void OnKill()
    {
        RoundManager.Instance.OnEnemyKilled();
        numEnemiesKilledThisFrame += 1;
    }
    public void OnCollectOrb()
    {
        
    }
    public void OnCollectItem()
    {
        
    }
    public void AddRep(int rep)
    {
        float floatRep = (float)rep;
        if (RoundManager.Instance.midGameChoice == "Honor") floatRep *= 1.3f;
        else if (RoundManager.Instance.midGameChoice == "Popularity") floatRep *= 1.3f;
        else if (RoundManager.Instance.midGameChoice == "Destruction") floatRep *= 0.9f;
        stats.reputation += Mathf.RoundToInt(floatRep);
        stats.reputation = Mathf.Clamp(stats.reputation, -100f, 100f);
    }
    public void AddCorruption(int corr)
    {
        float floatCorr = (float)corr;
        if (RoundManager.Instance.midGameChoice == "Destruction") floatCorr *= 1.1f;
        else if (RoundManager.Instance.midGameChoice == "Honor") floatCorr *= 0.9f;
        else if (RoundManager.Instance.midGameChoice == "Popularity") floatCorr *= 1.5f;

        stats.corruption += Mathf.RoundToInt(floatCorr);
        stats.corruption = Mathf.Clamp(stats.corruption, 0f, 100f);
    }  
    public void AddSponsers(int spon)
    {
        stats.sponsers += spon;
        stats.sponsers = Mathf.Max(stats.sponsers, 0f);
    }
    public void AddLoyalViewers(int loyal)
    {
        stats.loyalViewers += loyal;
        stats.loyalViewers = Mathf.Max(stats.loyalViewers, 0f);
    } 
    void LateUpdate()
    {
        if (numParriesThisFrame > 0) {
            playerCombat.ResetParry();
            if (numParriesThisFrame == 1)
            {
                HandleStyleBonus(StyleBonusTypes.Parry);
            } else if (numParriesThisFrame == 2)
            {
                HandleStyleBonus(StyleBonusTypes.DuoParry);
            } else if (numParriesThisFrame == 3)
            {
                HandleStyleBonus(StyleBonusTypes.TriParry);
            } else
            {
                HandleStyleBonus(StyleBonusTypes.MegaParry, numParriesThisFrame);
            }
            numParriesThisFrame = 0;
        }

        if (numEnemiesKilledThisFrame > 0)
        {
            if (numEnemiesKilledThisFrame == 1)
            {
                HandleStyleBonus(StyleBonusTypes.Kill);
            } else
            {
                HandleStyleBonus(StyleBonusTypes.Multikill, numEnemiesKilledThisFrame);
                RoundManager.Instance.OnMultiKill(numEnemiesKilledThisFrame);
            }
            numEnemiesKilledThisFrame = 0;
        }
    }
}

