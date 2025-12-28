using System;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class AbilityUI : MonoBehaviour
{
    private AbilityRuntime abilityRuntime; // the runtime instance of the ability
    [SerializeField] private TextMeshProUGUI skillText;  // assign this in the inspector 
    [SerializeField] private Transform cooldownOverlay; // assign this in the inspector 
    public void AssignAbilityUI(AbilityRuntime ability)
    {
        if (ability == null)
        {
            skillText.text = "None";
            abilityRuntime = null;
            return;
        } else
        {
            abilityRuntime = ability;
            skillText.text = abilityRuntime.abilityName;
        }
    }

    public void Update()
    {
        if (abilityRuntime == null) return;
        float cooldownPercent = abilityRuntime.GetCooldownRemaining()/abilityRuntime.baseCooldown;
        if (abilityRuntime.baseCooldown == 0) cooldownPercent = 0;
        cooldownOverlay.localScale = new Vector3(1, cooldownPercent, 1);
    }
}
