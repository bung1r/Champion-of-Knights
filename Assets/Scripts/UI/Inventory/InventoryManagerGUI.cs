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
    [HideInInspector] public StatsUIManager statsUIManager;
    public void Awake()
    {
        statsUIManager = GetComponentInParent<StatsUIManager>();
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
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.AssignStatManager(statManager);
            slot.AssignStatsUIManager(statsUIManager);
        }
    }

    public void ChangeSelectedSlot(int index)
    {
        selectedSlot = inventorySlots[index];
        selectIndicator.position = selectedSlot.transform.position;
        
        if (selectedSlot != null && selectedSlot.item != null)
        {
            statsUIManager.ShowInteractPrompt($"F to use {selectedSlot.item.itemName}");
        } else
        {
            // statsUIManager.HideInteractPrompt();
        }
    }
    public void AssignItemInit(int index, Item item)
    {
        inventorySlots[index].slotIndex = index;
        inventorySlots[index].InitItem(item);
    }
    public void UpdateSelectSlot(Item item = null)
    {;
        if (selectedSlot == null) ChangeSelectedSlot(inventoryManager.GetSlotIndex());
        selectedSlot.UpdateItem(item);
    }
}
