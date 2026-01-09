using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private Canvas dialogueCanvas;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueContentText;
    public GameObject choicesContainer;
    public GameObject choiceButtonPrefab;
    private FullDialogue dialogueInUse;
    private Talkable currentTalkingTo;
    private int currentDialogueIndex = 0;
    private PlayerStatManager player;
    private InventoryManager inventory;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = FindObjectOfType<PlayerStatManager>();
        inventory = player.GetComponent<InventoryManager>();
    }
    void Start()
    {
        dialogueCanvas = GetComponentInParent<Canvas>();
        HideDialogue();
    }

    public void ShowDialogue(string speakerName, Dialogue currentDialogue)
    {
        speakerNameText.text = speakerName;
        dialogueContentText.text = currentDialogue.dialogueLine;
        if (currentDialogue.choices.Count > 0)
        {
            // show choices
            foreach (Transform choiceButton in choicesContainer.transform)
            {
                Destroy(choiceButton.gameObject);
            }

            foreach (DialogueChoice choice in currentDialogue.choices)
            {
                GameObject choiceButtonObj = Instantiate(choiceButtonPrefab, choicesContainer.transform);
                ChoiceButton choiceButton = choiceButtonObj.GetComponent<ChoiceButton>();
                choiceButton.Setup(choice);
            }
        }
        else
        {
            // hide choices
            foreach (Transform choiceButton in choicesContainer.transform)
            {
                Destroy(choiceButton.gameObject);
            }
        }

        
        dialogueCanvas.enabled = true;
    }

    public void HideDialogue()
    {
        dialogueInUse = null;
        currentDialogueIndex = 0;
        currentTalkingTo = null;
        dialogueCanvas.enabled = false;
    }

    public void StartFullDialogue(FullDialogue dialogue, Talkable talkable = null)
    {
        if (dialogueInUse != null) return;
        currentTalkingTo = talkable;
        dialogueInUse = dialogue;
        currentDialogueIndex = 0;
        ShowCurrentDialogueLine();
    }   
    public void ProgressToNextDialogue()
    {
        if (dialogueInUse == null) return;
        Dialogue currentDialogue = dialogueInUse.dialogue[currentDialogueIndex];   
        if (currentDialogue.choices.Count > 0) return; // must make a choice first
        if (currentDialogue.effects.hasEffects)
        {
            HandleEffects(currentDialogue);
            return;
        }
        currentDialogueIndex++;
        ShowCurrentDialogueLine();
    }
    public void MadeChoice(DialogueChoice choice)
    {
        // stat changes and stuff like that
        if (choice.effects.nextDialogue != null)
        {
            dialogueInUse = choice.effects.nextDialogue;
            currentDialogueIndex = 0;
            ShowCurrentDialogueLine();
            return;
        } else
        {
            HideDialogue();
            dialogueInUse = null;
        }
    }
    public void ShowCurrentDialogueLine()
    {
        if (dialogueInUse != null && currentDialogueIndex < dialogueInUse.dialogue.Count)
        {
            Dialogue currentDialogue = dialogueInUse.dialogue[currentDialogueIndex];
            ShowDialogue(dialogueInUse.speakerName, currentDialogue);
            // Handle choices if any (not implemented here)
        }
        else
        {
            HideDialogue();
        }
    }

    public bool InDialogue()
    {
        return dialogueInUse != null;
    }

    public void HandleEffects(Dialogue currentDialogue)
    {
        // Implement stat changes and other effects here
        if (currentDialogue.effects.givenItem) 
            inventory.GiveItem(currentDialogue.effects.givenItem);
        


        if (currentDialogue.effects.endDialogue)
        {
            if (currentDialogue.effects.nextDialogue != null)
            {
                currentTalkingTo.dialogue = currentDialogue.effects.nextDialogue;
            }
            HideDialogue();
            return;
        } else
        {
            if (currentDialogue.effects.nextDialogue != null)
            {
                dialogueInUse = currentDialogue.effects.nextDialogue;
                currentDialogueIndex = 0;
                ShowCurrentDialogueLine();
                return;
            }

            currentDialogueIndex++;
            ShowCurrentDialogueLine();
        }
    }

    public void HandleChoiceEffects(DialogueChoice choice)
    {
        // Implement stat changes and other effects here
        if (choice.effects.givenItem) 
            inventory.GiveItem(choice.effects.givenItem);

        if (choice.effects.endDialogue)
        {
            if (choice.effects.nextDialogue != null)
            {
                currentTalkingTo.dialogue = choice.effects.nextDialogue;
            }
            HideDialogue();
            return;
        } else
        {
            if (choice.effects.nextDialogue != null)
            {
                dialogueInUse = choice.effects.nextDialogue;
                currentDialogueIndex = 0;
                ShowCurrentDialogueLine();
                return;
            }

            currentDialogueIndex++;
            ShowCurrentDialogueLine();
        }

        foreach (Transform choiceButton in choicesContainer.transform)
        {
            Destroy(choiceButton.gameObject);
        }
    }
    

    public void SelectChoice(DialogueChoice dialogueChoice)
    {
        if (dialogueInUse == null) return;
        HandleChoiceEffects(dialogueChoice);
    }
}