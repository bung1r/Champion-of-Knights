using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
public class StatManager : MonoBehaviour, IDamageable
{

    private Stats _stats;
    private Stats _baseStats;
    private float lastUsedStamina = 0;
    private float lastSavedStamina = -1;
    private Resistances resistances;
    public void Start()
    {
        PreStart();
        CoreStart();
        PostStart();
    }
    public void Update()
    {
        PreUpdate();
        CoreUpdate();
        PostUpdate();
    }
    // the taking damage logic, very cool. 
    public virtual void TakeDamage(DamageData damage)
    {

        float finalDamage = damage.baseDamage/_stats.resistances.Get(damage.type);
        _stats.currentHP -= finalDamage;
        Debug.Log($"{gameObject.name} took {finalDamage} {damage.type} damage!");
        if (_stats.currentHP <= 0) Die(damage);
    }
    public virtual void RegenerateStamina()
    {
        lastUsedStamina += Time.deltaTime;

        // if stamina is lower than before, you used it
        if (_stats.currentStamina < lastSavedStamina)
        {
            lastUsedStamina = 0;
        } 

        /* if enough time has passed without losing stamina, 
        regenerate it (also don't let it get above max) */
        if ((_stats.startStaminaRegen < lastUsedStamina && _stats.currentStamina > 0) || (_stats.currentStamina == 0 && _stats.startStaminaRegenFromZero < lastUsedStamina))
        {
            _stats.currentStamina += _stats.staminaRegen * Time.deltaTime;
            _stats.currentStamina = Mathf.Min(_stats.currentStamina, _stats.maxStamina);
        }

        lastSavedStamina = _stats.currentStamina;
    }
    // iterates through multipliers and destroys ones that shouldn't be there anymore. 
    public virtual void CheckMultipliers()
    {
        List<DamageMultiplier>multiplierList = GetAllDamageMultipliers();
        for (int i = multiplierList.Count - 1; i >= 0; i--)
        {
            DamageMultiplier value = multiplierList[i];
            if (Time.time - value.lifeTime > value.lifeTime) multiplierList.RemoveAt(i);
        }
    }
    // this will return false you do not have enough stamina to use
    public void UseStamina(float staminaCost)
    {
        // basic checking, also checks overflow as well. 
            // makes sure no negative shinanegans happen. 
        _stats.currentStamina = Mathf.Clamp(_stats.currentStamina - staminaCost, 0, _stats.maxStamina);
    }

    public bool CanUseStamina(float staminaCost, bool allowOverflow = false)
    {   
        float temp = 0;
        if (allowOverflow) temp -= _stats.overflowStaminaThreshold;
        if (_stats.currentStamina - staminaCost >= temp && _stats.currentStamina>0)
        {
            return true;
        }
        return false;
    }
    public List<DamageMultiplier> GetAllDamageMultipliers() => _stats.damageMultipliers;
    // need to add a multiplier?
    public void AddMultiplier(DamageMultiplier damageMultiplier)
    {
        _stats.damageMultipliers.Add(damageMultiplier);
    }
    
    public void AddStatModifier(StatModifier modifier)
    {
        if (modifier.statModifierType == StatModifierType.TempBuff)
        {
            
            _stats.statModifiers.Add(modifier);
        } else if (modifier.statModifierType == StatModifierType.NodeBuff)
        {
            bool foundIt = false;
            foreach (StatModifier statModifier in _stats.statModifiers)
            {
                if (statModifier.statModifierType == StatModifierType.NodeBuff)
                {
                    statModifier.EditModifier(modifier);
                    foundIt = true;
                }
            }
            if (!foundIt)
            {
                _stats.statModifiers.Add(modifier);
            }
        }
        // apply the stat changes
        ApplyStatModifiers();
    }
    // Apply the stat Modifiers
    public void ApplyStatModifiers()
    {
        Dictionary<BaseStatsEnum, float> statDict = new Dictionary<BaseStatsEnum, float>
        {
            {BaseStatsEnum.baseEXP, _baseStats.baseEXP},
            {BaseStatsEnum.maxHP, _baseStats.maxHP},
            {BaseStatsEnum.sprintSpeed, _baseStats.sprintSpeed},
            {BaseStatsEnum.staminaRegen, _baseStats.staminaRegen},
            {BaseStatsEnum.walkSpeed, _baseStats.walkSpeed},
            {BaseStatsEnum.sprintStaminaCost, _baseStats.sprintStaminaCost},
            {BaseStatsEnum.maxStamina, _baseStats.maxStamina}
        };

        // complicated :(
        foreach (StatModifier entry in _stats.statModifiers)
        {
            // if additive, add the stat modifier
            if (entry.generalModifierType == ModifierTypes.Additive)
            {
                foreach (KeyValuePair<BaseStatsEnum, float> kvPair in entry.statDict)
                {
                    statDict[kvPair.Key] += kvPair.Value;
                }
            } else if (entry.generalModifierType == ModifierTypes.Multiplicative)
            // if multiplicative, multiply the multiplied values then multiply at the end of calculation
            {
                
            } else
            {
                Debug.Log("You should never be setting, lol!");
            }
        }

        
        _stats.baseEXP = statDict[BaseStatsEnum.baseEXP];
        _stats.maxHP = statDict[BaseStatsEnum.maxHP];
        _stats.sprintSpeed = statDict[BaseStatsEnum.sprintSpeed];
        _stats.staminaRegen = statDict[BaseStatsEnum.staminaRegen];
        _stats.walkSpeed = statDict[BaseStatsEnum.walkSpeed];
        _stats.sprintStaminaCost = statDict[BaseStatsEnum.sprintStaminaCost];
        _stats.maxStamina = statDict[BaseStatsEnum.maxStamina];
    }
    // what happens when we die? Oh no!
    public void Die(DamageData damage) {
        if (damage.source.TryGetComponent<PlayerStatManager>(out var playerStat))
        {
            Debug.Log($"{gameObject.name} has died! {damage.source.name} gained {_stats.baseEXP} EXP");
            playerStat.AddEXP(_stats.baseEXP);
        }
        Destroy(gameObject);
    }

    protected virtual void PreUpdate() {}
    // will contain everything that happens in this script
    protected virtual void CoreUpdate()
    {
        RegenerateStamina();
        CheckMultipliers();
    }
    protected virtual void PostUpdate() {}

    protected virtual void PreStart() {}
    // will contain everything in the start that happens in this script
    protected virtual void CoreStart()
    {

    }
    protected virtual void PostStart() {}
    // setters for the stats, don't worry about it!
    public void setEnemyStats(EnemyStats stats)
    {
        _stats = stats;
        _baseStats = new EnemyStats(stats);
    }
    public void setPlayerStats(PlayerStats stats)
    {
        _stats = stats;
        _baseStats = new PlayerStats(stats);
    }
   
}
