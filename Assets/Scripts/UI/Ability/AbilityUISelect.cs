using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUISelect : MonoBehaviour
{
    public AbilityBase ability;
    private AbilityEquipUIManager abilityUIManager;
    private Button selectButton;
    private TextMeshProUGUI abilityText;
    public bool chosen = false;
    void Start()
    {
        selectButton = GetComponent<Button>();
        if (abilityText == null) abilityText = GetComponentInChildren<TextMeshProUGUI>();
        selectButton.onClick.AddListener(SelectAbility);
    }
    public void SetAbility(AbilityBase newAbility) {
        if (abilityText == null) abilityText = GetComponentInChildren<TextMeshProUGUI>();
        ability = newAbility;
        abilityText.text = ability.abilityName;
    }
    public void AssignUIManager(AbilityEquipUIManager manager)
    {
        abilityUIManager = manager;
    }
    public void SelectAbility()
    {
        if (chosen) return;
        abilityUIManager.SetSelectedAbilityUI(this);
    }
    public void SetChosen(bool isChosen)
    {
        chosen = isChosen;
        if (chosen)
        {
            GetComponent<Image>().color = Color.red;
        } 
        else 
        {
            GetComponent<Image>().color = Color.white;
        }
    }
}