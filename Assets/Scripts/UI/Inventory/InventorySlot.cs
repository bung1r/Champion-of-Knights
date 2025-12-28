using System;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class InventorySlot : MonoBehaviour
{
    private PlayerStatManager statManager;
    // public Item item; // Item assigned to this 
    [HideInInspector] public int slotIndex;
    private TextMeshProUGUI slotText;
    public Item item; // Item assigned to this slot
    void Awake()
    {
        slotText = GetComponent<TextMeshProUGUI>();
        UpdateItem(item);
    }
    public void AssignUIManager(PlayerStatManager manager)
    {
        statManager = manager;
    }
    public void UpdateItem(Item newItem = null)
    {
        if (newItem == null)
        {
            item = null;
            slotText.text = $"{slotIndex + 1}. Empty";
            return;
        } else {
            item = newItem;
            slotText.text = $"{slotIndex + 1}. {item.itemName}";
        }
    }
}
