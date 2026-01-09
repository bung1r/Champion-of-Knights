using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ChoiceButton : MonoBehaviour
{
    public DialogueChoice choiceData;
    public TextMeshProUGUI choiceText;
    public void Start()
    {
        // sets up the button event on runtime
        GetComponent<Button>().onClick.AddListener(OnChoiceSelected);
    }
    public void Setup(DialogueChoice data)
    {
        choiceData = data;
        choiceText.text = data.choiceText;
    }

    public void OnChoiceSelected()
    {
        Debug.Log("Hello!");
        DialogueManager.Instance.SelectChoice(choiceData);
    }
    
}