using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private Canvas dialogueCanvas;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI dialogueContentText;
    public GameObject choicesContainer;
    public GameObject choiceButtonPrefab;
    public Image bgImage;
    private FullDialogue dialogueInUse;
    private Talkable currentTalkingTo;
    private int currentDialogueIndex = 0;
    private PlayerStatManager player;
    private InventoryManager inventory;
    private float nextLineDelay = 0.3f;
    private float lastContinuedDialogueLine = 0f;
    private float nextDialogueDelay = 0.8f;
    private float lastLeftDialogue = 0f;
    private float typingSpeed = 0.03f;
    private Coroutine typingCoroutine;

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
    IEnumerator TypeTextRoutine(string fullText)
    {
        dialogueContentText.maxVisibleCharacters = 0;

        while (dialogueContentText.maxVisibleCharacters < fullText.Length)
        {
            dialogueContentText.maxVisibleCharacters++;
            // Wait for the specified delay before showing the next character
            yield return new WaitForSeconds(typingSpeed);
        }

        dialogueContentText.maxVisibleCharacters = fullText.Length;
        typingCoroutine = null;
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
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeTextRoutine(currentDialogue.dialogueLine));
        
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
        lastLeftDialogue = Time.time;
        dialogueInUse = null;
        currentDialogueIndex = 0;
        currentTalkingTo = null;
        dialogueCanvas.enabled = false;
    }

    public void StartFullDialogue(FullDialogue dialogue, Talkable talkable = null)
    {
        if (dialogue == null) return;
        if (dialogueInUse != null) return;
        if (Time.time - lastLeftDialogue < nextDialogueDelay) return;
        if (talkable != null) talkable.OnTalkedTo();

        currentTalkingTo = talkable;
        dialogueInUse = dialogue;
        currentDialogueIndex = 0;
        ShowCurrentDialogueLine();
    }   
    public void ProgressToNextDialogue()
    {
        if (dialogueInUse == null) return;
        if (Time.time - lastContinuedDialogueLine < nextLineDelay) return;
        lastContinuedDialogueLine = Time.time;
        Dialogue currentDialogue = dialogueInUse.dialogue[currentDialogueIndex];   

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            dialogueContentText.maxVisibleCharacters = 999999;
            typingCoroutine = null;
            return;
        }
        
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
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                dialogueContentText.maxVisibleCharacters = 999999;
                typingCoroutine = null;
                return;
            }
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
        EffectHandler(currentDialogue.effects);
    }

    public void HandleChoiceEffects(DialogueChoice choice)
    {
        // Implement stat changes and other effects here
        EffectHandler(choice.effects);

        foreach (Transform choiceButton in choicesContainer.transform)
        {
            Destroy(choiceButton.gameObject);
        }
    }
    
    private void EffectHandler(DialogueEffects effects)
    {
        
        // Implement stat changes and other effects here
        if (effects.givenItem) 
            inventory.GiveItem(effects.givenItem);

        player.AddRep((int)effects.reputation);
        player.AddCorruption((int)effects.corruption);
        player.AddSponsers((int)effects.sponsers);
        player.AddLoyalViewers((int)effects.loyalviewers); 
        
        foreach (ComplexDialogueEffect effect in effects.complexEffects)
        {
            ComplexEffectHandler(effect);
        }
        
        if (effects.endDialogue)
        {
            if (effects.nextDialogue != null)
            {
                currentTalkingTo.dialogue = effects.nextDialogue;
            }
            HideDialogue();
            return;
        } else
        {
            if (effects.nextDialogue != null)
            {
                dialogueInUse = effects.nextDialogue;
                currentDialogueIndex = 0;
                ShowCurrentDialogueLine();
                return;
            }

            currentDialogueIndex++;
            ShowCurrentDialogueLine();
        }
    }
    public void SelectChoice(DialogueChoice dialogueChoice)
    {
        if (dialogueInUse == null) return;
        HandleChoiceEffects(dialogueChoice);
    }
    public void EnableBG()
    {
        bgImage.enabled = true;
    }
    public void ComplexEffectHandler(ComplexDialogueEffect effect)
    {
        switch (effect.effectType)
        {
            case DialogueEffectTypes.Reputation:
                player.AddRep((int)effect.floatValue);
                break;
            case DialogueEffectTypes.LoyalViewers:
                player.AddLoyalViewers((int)effect.floatValue);
                break;
            case DialogueEffectTypes.Sponsor:
                player.AddSponsers((int)effect.floatValue);
                break;
            case DialogueEffectTypes.Money:
                // player.AddMoney((int)effect.floatValue);
                break;
            case DialogueEffectTypes.Corruption:
                player.AddCorruption((int)effect.floatValue);
                break;
            case DialogueEffectTypes.DoEnding:
                RoundManager.Instance.ending = effect.intValue;
                RoundManager.Instance.EndRoundSequence();
                break;
            case DialogueEffectTypes.GiveItem:
                Debug.Log("Gave Item via Dialogue Effect");
                inventory.GiveItem(effect.itemValue);
                break;
            case DialogueEffectTypes.MidGameChoice:
                RoundManager.Instance.MakeMidGameChoice(effect.stringValue);
                break;
            case DialogueEffectTypes.OpenVictoryScreen:
                RoundManager.Instance.StartVictorySequence();
                break;
            default:
                break;
        }
    }
}