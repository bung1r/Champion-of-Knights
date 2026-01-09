using UnityEngine;

public class Talkable : MonoBehaviour, IInteractable
{
    public FullDialogue dialogue;

    public string customName = "NPC";

    public void Interact(GameObject interactor)
    {
        if (interactor.GetComponent<PlayerStatManager>() != null)
        {
            DialogueManager.Instance.StartFullDialogue(dialogue, this);
        }
    }
}