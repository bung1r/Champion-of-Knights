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

        // Basically, goes down more the higher the styleLevel. 
        AddStyle(-1 * (Mathf.Pow((stats.styleLevel + 1) * 6, 0.75f) * Time.deltaTime));
        statsUIManager.UpdateStyle(stats.currentStyle, stats.maxStyle, stats.totalStyle, stats.viewers, stats.styleLevel);
        
    } 
    public void CalculateViewers()
    {
        float stable = (stats.styleLevel + stats.currentStyle / stats.maxStyle) * 400;
        float viewerGain = 0.2f * (stable - stats.viewers);
        stats.viewers += viewerGain * Time.deltaTime;
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

