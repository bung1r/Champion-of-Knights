using UnityEditor;
using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable
{
    public Item item;
    void Start()
    {
        RoundManager.Instance.AddOnGroundItem(gameObject);
    }
    public void Interact(GameObject interactor)
    {
        InventoryManager inventoryManager = interactor.GetComponent<InventoryManager>();
        
        inventoryManager.PickUpItem(item);

        Destroy(gameObject);
    }
}