using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class InventoryManagerGUI : MonoBehaviour
{
    private PlayerStatManager statManager;
    private InventoryManager inventoryManager;
    public List<InventorySlot> inventorySlots = new List<InventorySlot>(); //asign in the inspector
    public Transform selectIndicator; //assign in the inspector
    private InventorySlot selectedSlot;
    public void Awake()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].slotIndex = i;
        }
    }
    public void AssignUIManager(PlayerStatManager manager)
    {
        statManager = manager;
        inventoryManager = statManager.GetComponent<InventoryManager>();
        inventoryManager.AssignInventoryGUI(this);
    }

    public void ChangeSelectedSlot(int index)
    {
        selectedSlot = inventorySlots[index];
        selectIndicator.position = selectedSlot.transform.position;
    }
    public void AssignItemInit(int index, Item item)
    {
        inventorySlots[index].slotIndex = index;
        inventorySlots[index].UpdateItem(item);
    }
    public void UpdateSelectSlot(Item item = null)
    {;
        selectedSlot.UpdateItem(item);
    }
}
