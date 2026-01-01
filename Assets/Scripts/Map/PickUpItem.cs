using UnityEditor;
using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable
{
    public Item item;

    public void Interact(GameObject interactor)
    {
        InventoryManager inventoryManager = interactor.GetComponent<InventoryManager>();
        
        inventoryManager.UpdateSelectedSlot(item);

        Destroy(gameObject);
    }
}