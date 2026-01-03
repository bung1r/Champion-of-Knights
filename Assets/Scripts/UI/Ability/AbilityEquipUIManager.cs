using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AbilityEquipUIManager : MonoBehaviour
{
    [SerializeField] private AbilityEquipUI abilitySlot1; // Q
    [SerializeField] private AbilityEquipUI abilitySlot2; // E
    [SerializeField] private AbilityEquipUI abilitySlot3; // R
    [SerializeField] private Transform abilitySelectContainer;
    [SerializeField] private GameObject abilitySelectPrefab;
    [SerializeField] private TextMeshProUGUI selectedAbilityText;
    private Canvas parentCanvas;
    private PlayerCombat playerCombat;
    private AbilityBase selectedAbility;
    private AbilityUISelect selectedAbilityUI;
    public AbilityBase GetSelectedAbility() => selectedAbility;
    public AbilityUISelect GetSelectedAbilityUI() => selectedAbilityUI;
    public void SetSelectedAbility(AbilityBase ability)
    {
        selectedAbility = ability;
        selectedAbilityUI = null;
        if (ability != null)
            selectedAbilityText.text = "Selected: " + selectedAbility.abilityName;
        else
            selectedAbilityText.text = "Selected: None";
    }
    public void SetSelectedAbilityUI(AbilityUISelect abilityUI)
    {
        if (abilityUI != null)
        {
            selectedAbilityUI = abilityUI;
            selectedAbility = selectedAbilityUI.ability;
            selectedAbilityText.text = "Selected: " + selectedAbility.abilityName;
        } else
        {
            selectedAbilityUI = null;
            selectedAbility = null;
            selectedAbilityText.text = "Selected: None";
        }
    }   
    void Start()
    {
        playerCombat = FindObjectOfType<PlayerCombat>();
        abilitySlot1.AssignPlayerCombat(this, playerCombat);
        abilitySlot2.AssignPlayerCombat(this, playerCombat);
        abilitySlot3.AssignPlayerCombat(this, playerCombat);
        selectedAbilityText.text = "Selected: None";
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void UnlockAbility(AbilityBase ability, PlayerCombat pc = null)
    {
        if (playerCombat == null) playerCombat = pc;
        playerCombat.allUnlockedAbilities.Add(ability);
        GameObject abilitySelectObj = Instantiate(abilitySelectPrefab, abilitySelectContainer);
        AbilityUISelect abilityUISelect = abilitySelectObj.GetComponent<AbilityUISelect>();
        abilityUISelect.SetAbility(ability);
        abilityUISelect.AssignUIManager(this);
    }
    public AbilityUISelect AbilityToAbilitySelectUI(AbilityBase ability)
    {
        foreach (Transform child in abilitySelectContainer)
        {
            AbilityUISelect abilityUI = child.GetComponent<AbilityUISelect>();
            if (abilityUI.ability == ability)
            {
                return abilityUI;
            }
        }
        return null;
    }

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
}