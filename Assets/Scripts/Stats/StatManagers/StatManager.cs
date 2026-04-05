using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public class StatManager : MonoBehaviour, IDamageable
{

    private Stats _stats;
    private Stats _baseStats;
    private float lastUsedStamina = 0;
    private float lastSavedStamina = -1;
    private float lastTakenDamage = -1f;
    private float lastSavedHP = -1f;
    [SerializeField] private AudioSource hitSFX; // completely optional
    private Resistances resistances;//
    public FloatEvent OnTakeDamage;
    public UnityEvent OnDeath;
    public UnityEvent OnParry;
    public FloatEvent OnHit;
    public UnityEvent OnKill;
    public UnityEvent OnUpdate;
    public UnityEvent OnUseItem;
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
    // simple getter
    public Stats GetStats() => _stats;
    public bool GetInAttack() => _stats.inAttackAnim;
    public virtual void BeginAttack(AbilityBase ability)
    {
        _stats.inAttackAnim = true;
        _stats.isRunning = false;
        _stats.isWalking = false;
        _stats.attackID = ability.attackID;
    }
    public virtual void EndAttack()
    {
        _stats.inAttackAnim = false;
    }
    // the taking damage logic, very cool. use when possilbe
    public virtual void TakeDamage(DamageData damage, bool bypassMax = false) 
    {
        if (_stats.isParrying && damage.type != DamageType.Fixed && damage.source != null)
        {

            OnParry.Invoke();
            damage.source.GetComponent<StatManager>()?.BasicStun(damage.abilityBase.stunTime/2);
            damage.source.GetComponent<StatManager>()?.Knockback(transform.position, damage.baseDamage/10f * _stats.knockbackMultiplier);

            // if (this is PlayerStatManager playerStatManager)
            // {
            //     damage.source.GetComponent<StatManager>()?.BasicStun(damage.abilityBase.stunTime/2);
            //     damage.source.GetComponent<StatManager>()?.Knockback(playerStatManager.transform.position, damage.baseDamage/10f);
            // }
            
            return; // you parried, congrats!
        }
        // calc damage then take damage. 
        float finalDamage;
        if (damage.type == DamageType.Fixed)
        {
            finalDamage = damage.baseDamage;
        } else
        {
            finalDamage = damage.baseDamage/_stats.resistances.Get(damage.type)/_stats.resistances.Get(DamageType.All);
        }
       

        _stats.currentHP -= finalDamage;
        if (_stats.currentHP > _stats.maxHP && !bypassMax) _stats.currentHP = _stats.maxHP;
        damage.source?.GetComponent<StatManager>()?.OnHit.Invoke(finalDamage);
        OnTakeDamage.Invoke(finalDamage);
        Debug.Log($"{gameObject.name} took {finalDamage} {damage.type} damage!");
        
        // checks whether a custom hit SFX is assigned, if not use the default one.
        if (finalDamage >= 0)
        {
            if (hitSFX != null) AudioManager.Instance.PlaySourceAtPointWithPitch(hitSFX, transform.position, 0.2f);
            else AudioManager.Instance.PlayHitSFX(transform, 0.2f);
        } else {
           if (finalDamage < -10.1f) // must be a big enough heal so life steal doesn't trigger ts like hell
            {
                AudioManager.Instance.PlayHealSFX(transform);
            }
        }
        
        
        // apply stun and knockback if applicable
        if (damage.abilityBase != null)
        {
            if (_stats.stunTime < damage.abilityBase.stunTime) {
                BasicStun(damage.abilityBase.stunTime);
                Knockback(damage);
            }
        }

        // die if we have no HP left
        if (_stats.currentHP <= 0)
        {
            if (RoundManager.Instance.JOURNALISTMODE && this is PlayerStatManager)
            {
                _stats.currentHP = 1;
                return;
            }
            Die(damage);
        }
    }
    public virtual void Knockback(DamageData damage)
    {
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
        }
        if (damage.abilityBase.knockback > 0 || damage.abilityBase.knockback < 0)
        {
            Vector3 vector = transform.position - damage.source.transform.position;
            vector.y = 0;
            if (damage.source != null && damage.source.TryGetComponent<StatManager>(out var statManager))
            {
                vector = vector.normalized * (damage.abilityBase.knockback * statManager.GetStats().knockbackMultiplier);
            } else
            {
                vector = vector.normalized * damage.abilityBase.knockback;
            }
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = false;
                rb.AddForce(vector, ForceMode.Impulse);  
            }
        }  
    }
    public virtual void Knockback(Vector3 source, float force)
    {
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
        }
        Vector3 vector = transform.position - source;
        vector.y = 0;
        vector = vector.normalized * force;
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.AddForce(vector, ForceMode.Impulse);  
        }
    }
    public virtual void BasicStun(float stunTime)
    {
        stunTime *= _stats.stunTimeMultiplier;
        if (_stats.stunTime < stunTime) {
            _stats.stunTime = stunTime;
        }
    }
    public virtual void StunHandler()
    {
        _stats.stunTime = Mathf.Max(_stats.stunTime - Time.deltaTime, 0);
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
    public virtual void RegenerateHP()
    {
        if (lastSavedHP > _stats.currentHP)
        {
            lastTakenDamage = Time.time;
        }

        if (Time.time - lastTakenDamage > _stats.startRegenHP)
        {
             // regen HP

            _stats.currentHP += _stats.regenHP * Time.deltaTime;
            _stats.currentHP = Mathf.Min(_stats.currentHP, _stats.maxHP);
            
        }

        lastSavedHP = _stats.currentHP;
    }
    
    // iterates through multipliers and destroys ones that shouldn't be there anymore. 
    public virtual void CheckMultipliers()
    {
        List<DamageMultiplier>multiplierList = GetAllDamageMultipliers();
        for (int i = multiplierList.Count - 1; i >= 0; i--)
        {
            DamageMultiplier value = multiplierList[i];
            if (Time.time - value.timeCreated > value.lifeTime) multiplierList.RemoveAt(i);
        }
    }
    // this will return false you do not have enough stamina to use
    public void UseStamina(float staminaCost)
    {
        // basic checking, also checks overflow as well. 
            // makes sure no negative shinanegans happen. 
        staminaCost *= _stats.staminaUsageMultiplier;
        _stats.currentStamina = Mathf.Clamp(_stats.currentStamina - staminaCost, 0, _stats.maxStamina);
    }

    public bool CanUseStamina(float staminaCost, bool allowOverflow = false)
    {   
        staminaCost *= _stats.staminaUsageMultiplier;
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
    public void AddMultiplier(DamageMultiplier damageMultiplier, string addType = "add")
    {
        foreach (DamageMultiplier dm in _stats.damageMultipliers)
        {
            if (dm.source == damageMultiplier.source && dm.lifeTime == Mathf.Infinity && dm.type == damageMultiplier.type)
            {
                if (dm.type == DamageMultiplierTypes.Additive)
                {
                    if (addType == "add")
                    {
                        dm.amount += damageMultiplier.amount;
                    } else if (addType == "set")
                    {
                        dm.amount = damageMultiplier.amount;
                    }
                    return;
                } else if (dm.type == DamageMultiplierTypes.Multiplicative)
                {
                    if (addType == "add")
                    {
                        dm.amount *= damageMultiplier.amount;
                    } else if (addType == "set")
                    {
                        dm.amount = damageMultiplier.amount;
                    }
                    return;
                }
            }
        }
        
        _stats.damageMultipliers.Add(damageMultiplier);
    }
    
    [HideInInspector] public List<PassiveAbilityRuntime> passiveRuntimeAbilities = new List<PassiveAbilityRuntime>();
    public void AddPassive(PassiveAbilityBase passive)
    {
        _stats.passiveAbilities.Add(passive);
        PassiveAbilityRuntime runtime = passive.CreateRuntimeInstance(passive, this);
        runtime.Init(gameObject);
        OnTakeDamage.AddListener(runtime.OnTakeDamage);
        OnHit.AddListener(runtime.OnHit);
        OnKill.AddListener(runtime.OnKill);
        OnParry.AddListener(runtime.OnParry);
        OnDeath.AddListener(runtime.OnDeath);
        OnUpdate.AddListener(runtime.OnUpdate);
        OnUseItem.AddListener(runtime.OnUseItem);
        passiveRuntimeAbilities.Add(runtime);
    }
    public void RemovePassive(PassiveAbilityBase passive)
    {
        int index = _stats.passiveAbilities.IndexOf(passive);
        if (index >= 0)
        {
            PassiveAbilityRuntime runtime = passiveRuntimeAbilities[index];
            OnTakeDamage.RemoveListener(runtime.OnTakeDamage);
            OnHit.RemoveListener(runtime.OnHit);
            OnKill.RemoveListener(runtime.OnKill);
            OnParry.RemoveListener(runtime.OnParry);
            OnDeath.RemoveListener(runtime.OnDeath);
            OnUpdate.RemoveListener(runtime.OnUpdate);
            OnUseItem.RemoveListener(runtime.OnUseItem);
            _stats.passiveAbilities.RemoveAt(index);
            passiveRuntimeAbilities.RemoveAt(index);
        }
    }
    public void Spin(float force)
    {
        if (!TryGetComponent<Rigidbody>(out var rb)) return;
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
            EnableAgentWithDelay(0.3f);
        }
        Debug.Log("MY BOENS");
        Debug.Log(force);
        rb.AddTorque(Vector3.up * force, ForceMode.Impulse);
    }
    public void Forward(float force)
    {
        if (!TryGetComponent<Rigidbody>(out var rb)) return;
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = false;
            EnableAgentWithDelay(0.3f);
        }
        Vector3 forwardDir = transform.forward;
        forwardDir.y = 0;
        forwardDir = forwardDir.normalized * force;
        rb.AddForce(forwardDir, ForceMode.Impulse);
    }
    async public void EnableAgentWithDelay(float delay)
    {
        await Task.Delay((int)(delay * 1000));
        if (TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.enabled = true;
            if (TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                
            }
        }
    }

    public void AddStatModifier(StatModifier modifier)
    {
        if (modifier.statModifierType == StatModifierType.TempBuff)
        {
            _stats.statModifiers.Add(modifier);
            Debug.Log("Added Temp Buff");
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
        // // apply the stat changes
        // ApplyStatModifiers();
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
            {BaseStatsEnum.maxStamina, _baseStats.maxStamina},
            {BaseStatsEnum.regenHP, _baseStats.regenHP},
            {BaseStatsEnum.startRegenHP, _baseStats.startRegenHP},
            {BaseStatsEnum.parryAdder, _baseStats.parryAdder},
            {BaseStatsEnum.parryDmgMultiplier, _baseStats.parryDmgMultiplier},
            {BaseStatsEnum.popularityMultiplier, _baseStats.popularityMultiplier},
            {BaseStatsEnum.knockbackMultiplier, _baseStats.knockbackMultiplier},
            {BaseStatsEnum.stunTimeMultiplier, _baseStats.stunTimeMultiplier},
            {BaseStatsEnum.supplyCrateCooldownReduction, _baseStats.supplyCrateCooldownReduction},
            {BaseStatsEnum.styleMultiplier, _baseStats.styleMultiplier},
            {BaseStatsEnum.corruptionMultiplier, _baseStats.corruptionMultiplier},
            {BaseStatsEnum.repMultiplier, _baseStats.repMultiplier},
            {BaseStatsEnum.staminaUsageMultiplier, _baseStats.staminaUsageMultiplier},
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
            } 
        }

        foreach (StatModifier entry in _stats.statModifiers)
        {
            if (entry.generalModifierType == ModifierTypes.Multiplicative)
            {
                Debug.Log(entry.statDict.Count);
                foreach (KeyValuePair<BaseStatsEnum, float> kvPair in entry.statDict)
                {
                    
                    statDict[kvPair.Key] *= kvPair.Value;
                    Debug.Log(statDict[kvPair.Key]);
                }
            }
        } 
        
        _stats.baseEXP = statDict[BaseStatsEnum.baseEXP];
        _stats.maxHP = statDict[BaseStatsEnum.maxHP];
        _stats.sprintSpeed = statDict[BaseStatsEnum.sprintSpeed];
        _stats.staminaRegen = statDict[BaseStatsEnum.staminaRegen];
        _stats.walkSpeed = statDict[BaseStatsEnum.walkSpeed];
        _stats.sprintStaminaCost = statDict[BaseStatsEnum.sprintStaminaCost];
        _stats.maxStamina = statDict[BaseStatsEnum.maxStamina];
        _stats.regenHP = statDict[BaseStatsEnum.regenHP];
        _stats.startRegenHP = statDict[BaseStatsEnum.startRegenHP];
        _stats.parryAdder = statDict[BaseStatsEnum.parryAdder];
        _stats.parryDmgMultiplier = statDict[BaseStatsEnum.parryDmgMultiplier];
        _stats.popularityMultiplier = statDict[BaseStatsEnum.popularityMultiplier];
        _stats.knockbackMultiplier = statDict[BaseStatsEnum.knockbackMultiplier];
        _stats.stunTimeMultiplier = statDict[BaseStatsEnum.stunTimeMultiplier];
        _stats.supplyCrateCooldownReduction = statDict[BaseStatsEnum.supplyCrateCooldownReduction];
        _stats.styleMultiplier = statDict[BaseStatsEnum.styleMultiplier];
        _stats.corruptionMultiplier = statDict[BaseStatsEnum.corruptionMultiplier];
        _stats.repMultiplier = statDict[BaseStatsEnum.repMultiplier];
        _stats.staminaUsageMultiplier = statDict[BaseStatsEnum.staminaUsageMultiplier];
    }
    public void CheckStatModifiers()
    {
        // if (this is PlayerStatManager psm)
        // {
        //     Debug.Log($"{_stats.statModifiers.Count == 1} at {Time.time}");
        // }
        if (_stats.statModifiers.Count == 0) return;
        for (int i = _stats.statModifiers.Count - 1; i >= 0; i--)
        {
            StatModifier modifier = _stats.statModifiers[i];
            if (modifier.statModifierType == StatModifierType.TempBuff && Time.time - modifier.timeCreated > modifier.duration)
            {
                _stats.statModifiers.RemoveAt(i);
            }
        }
        ApplyStatModifiers();
    }
    // what happens when we die? Oh no!
    public virtual void Die(DamageData damage) {
        if (damage.source != null)
        {
            damage.source.GetComponent<StatManager>()?.OnKill.Invoke();
            if (damage.source.TryGetComponent<PlayerStatManager>(out var playerStat))
            {
                OnDeath.Invoke();
                Debug.Log($"{gameObject.name} has died! {damage.source.name} gained {_stats.baseEXP} EXP");
                playerStat.AddEXP(_stats.baseEXP);
            }


        }
        
        GlobalPrefabs.Instance.DeathVFX(transform);
        AudioManager.Instance.PlayDeathSFX(transform);
        Destroy(gameObject);
    }

    public virtual void PreUpdate() {}
    // will contain everything that happens in this script
    protected virtual void CoreUpdate()
    {
        OnUpdate?.Invoke();
        StunHandler();
        RegenerateStamina();
        RegenerateHP();
        CheckMultipliers();
        CheckStatModifiers();
    }
    public virtual void PostUpdate() {}

    public virtual void PreStart() {}
    // will contain everything in the start that happens in this script
    public virtual void CoreStart()
    {
        foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
        {
            if (_stats.resistances.Get(damageType) > 0) continue;
            _stats.resistances.AddEntry(new ResistanceEntry
            {
                type = damageType,
                resistance = 1f,
            });
            // Debug.Log("Default resist aded");
        }
    }
    public virtual void PostStart() {}
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
