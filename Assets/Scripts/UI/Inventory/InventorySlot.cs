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
    private StatsUIManager statsUIManager;
    public Item item; // Item assigned to this slot
    void Awake()
    {
        slotText = GetComponent<TextMeshProUGUI>();
        InitItem(item);
    }
    public void AssignStatManager(PlayerStatManager manager)
    {
        statManager = manager;
    }
    public void AssignStatsUIManager(StatsUIManager statsUI)
    {
        statsUIManager = statsUI;
    }
    public void UpdateItem(Item newItem = null)
    {
        if (newItem == null)
        {
            item = null;
            slotText.text = $"{slotIndex + 1}. Empty";
            return;
        } else {
            if (item != null && item.prefab != null)
            {
                Vector3 spawnPos = statManager.transform.position + statManager.transform.forward + Vector3.up;
                Instantiate(item.prefab, spawnPos, Quaternion.identity);
            }
            item = newItem;
            slotText.text = $"{slotIndex + 1}. {item.itemName}";
        }
    }
    public void InitItem(Item newItem = null)
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
