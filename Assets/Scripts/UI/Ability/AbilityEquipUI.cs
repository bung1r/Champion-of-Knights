using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityEquipUI : MonoBehaviour
{
    public int slot = 1; // Q is 1, E is 2, R is 3
    private AbilityBase equippedAbility;
    private AbilityUISelect selectedAbilityUI;
    private PlayerCombat playerCombat;
    private AbilityEquipUIManager abilityUIManager;
    [SerializeField] private TextMeshProUGUI abilityText;
    [SerializeField] private Button unequipButton;
    [SerializeField] private Button equipButton;
    void Start()
    {
        unequipButton.onClick.AddListener(UnequipAbility);
        equipButton.onClick.AddListener(EquipAbility);
    }
    public void AssignPlayerCombat(AbilityEquipUIManager manager, PlayerCombat pc)
    {
        abilityUIManager = manager;
        playerCombat = pc;

        foreach (SkillSlotAndAbility skillSlot in playerCombat.otherUsableAbilities)
        {
            if (skillSlot.skillSlot == slot)
            {
                if (skillSlot.abilityBase == null) {
                    abilityText.text = "None";
                    continue;
                }
                equippedAbility = skillSlot.abilityBase;
                abilityText.text = equippedAbility.abilityName;
            }
        }

        if (equippedAbility != null && selectedAbilityUI == null)
        {
            selectedAbilityUI = abilityUIManager.AbilityToAbilitySelectUI(equippedAbility);
            selectedAbilityUI.SetChosen(true);
        }
    }
    public void UnequipAbility()
    {
        if (equippedAbility == null) return;
        selectedAbilityUI.SetChosen(false);
        selectedAbilityUI = null;
        equippedAbility = null;
        abilityText.text = "None";
        playerCombat.UnequipAbilityAtSlot(slot);
    }
    public void EquipAbility()
    {
        AbilityUISelect selectedUI = abilityUIManager.GetSelectedAbilityUI();
        
        if (selectedUI != null && selectedUI.ability != equippedAbility)
        {
            EquipAbility(selectedUI);
            abilityUIManager.SetSelectedAbilityUI(null);
        } 
    }
    public void EquipAbility(AbilityUISelect abilityUI)
    {
    
        selectedAbilityUI = abilityUI;
        selectedAbilityUI.SetChosen(true);
        equippedAbility = abilityUI.ability;
        abilityText.text = abilityUI.ability.abilityName;
        playerCombat.EquipAbilityAtSlot(abilityUI.ability, slot);
    }
}