using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
public class StatManager : MonoBehaviour, IDamageable
{
    // Start is called before the first frame update
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
        if (_stats.currentHP <= 0) Die();
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
    
    // what happens when we die? Oh no!
    public void Die() {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }

    protected virtual void PreUpdate() {}
    // will contain everything that happens in this script
    protected virtual void CoreUpdate()
    {
        RegenerateStamina();
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
