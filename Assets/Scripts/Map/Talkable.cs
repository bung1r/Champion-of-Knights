using System;
using UnityEngine;

public class Talkable : MonoBehaviour, IInteractable
{
    public FullDialogue dialogue;

    public string customName = "NPC";
    public event Action<Talkable> onTalkedTo;

    public void Interact(GameObject interactor)
    {
        if (interactor.GetComponent<PlayerStatManager>() != null)
        {
            DialogueManager.Instance.StartFullDialogue(dialogue, this);
        }
    }

    public void OnTalkedTo()
    {
        onTalkedTo?.Invoke(this);
    }

    public void SwitchDialouge(FullDialogue newDialogue)
    {
        dialogue = newDialogue;
    }
}