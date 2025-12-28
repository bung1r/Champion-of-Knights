using System;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class AbilityUIManager : MonoBehaviour
{
    public List<AbilityUI> abilityUIs = new List<AbilityUI>(); // assign in inspector
    private PlayerCombat playerCombat;
    public void AssignPlayerCombat(PlayerCombat combat)
    {
        playerCombat = combat;
        playerCombat.AssignAbilityUIManager(this);
    }   
    public void AssignAbilities(List<AbilityRuntime> abilities)
    {
        for (int i = 0; i < abilityUIs.Count; i++)
        {
            if (i < abilities.Count)
            {
                abilityUIs[i].AssignAbilityUI(abilities[i]);
            }
            else
            {
                abilityUIs[i].AssignAbilityUI(null);
            }
        }
    }

    public void AssignAbilityAtIndex(AbilityRuntime ability, int index)
    {
        if (index < 0 || index >= abilityUIs.Count) return;
        abilityUIs[index].AssignAbilityUI(ability);
    }

    
}
