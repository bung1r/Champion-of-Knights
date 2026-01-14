using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class InventoryManager : MonoBehaviour
{
    private PlayerStatManager statManager;
    private InputHandler inputHandler;
    private InventoryManagerGUI inventoryManagerGUI;
    public List<ItemPair> items = new List<ItemPair>(); //asign 9 of these in the inspector
    public Transform selectIndicator; //assign in the inspector
    private int selectedSlotIndex = 0;
    private ItemPair selectedItem;
    public void Awake()
    {
        statManager = GetComponent<PlayerStatManager>();
        inputHandler = GetComponent<InputHandler>();
    }
    public void Start()
    {
        selectedItem = items[selectedSlotIndex];
    }
    public void AssignInventoryGUI(InventoryManagerGUI gui)
    {
        inventoryManagerGUI = gui;

        for (int i = 0; i < items.Count; i++)
        {
            inventoryManagerGUI.AssignItemInit(i, items[i].item);
            items[i].UpdateItem(items[i].item);
        }
    }
    public void ChangeSelectedSlot(int index)
    {
        selectedSlotIndex = index;
        selectedItem = items[selectedSlotIndex];
        inventoryManagerGUI.ChangeSelectedSlot(index);
    }
    public void UpdateSelectedSlot(Item item)
    {
        selectedItem.UpdateItem(item);
        inventoryManagerGUI.UpdateSelectSlot(item);
    }
    async void WaitToUpdateSelectedSlot(Item item, float delay)
    {
        await System.Threading.Tasks.Task.Delay((int)(delay * 1000));
        UpdateSelectedSlot(item);
    }
    public void Update()
    {
        // wow so efficiency!
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeSelectedSlot(0);
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSelectedSlot(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSelectedSlot(2);
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeSelectedSlot(3);
        } else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeSelectedSlot(4);
        } else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ChangeSelectedSlot(5);
        } else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ChangeSelectedSlot(6);
        } else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ChangeSelectedSlot(7);
        } else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ChangeSelectedSlot(8);
        }

    }
    public void PickUpItem(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == null)
            {
                items[i].UpdateItem(item);
                inventoryManagerGUI.AssignItemInit(i, item);
                return;
            }
        }
    
        inventoryManagerGUI.UpdateSelectSlot(item);
    } 
    public void BeginInteract()
    {
        if (selectedItem == null || selectedItem.item == null || selectedItem.runtime == null) return;
        selectedItem.runtime.Use(statManager);
        selectedItem.runtime.BeginUse(statManager);
        if (selectedItem.runtime.usesRemaining <= 0)
        {
            WaitToUpdateSelectedSlot(null, selectedItem.item.performDelay);
        }
        
    }
    public int GetSlotIndex() =>  selectedSlotIndex;
    public ItemPair GetSelectedItem() => selectedItem;

    public void GiveItem(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == null)
            {
                items[i].UpdateItem(item);
                inventoryManagerGUI.AssignItemInit(i, item);
                return;
            }
        }

        Instantiate(item.prefab, statManager.transform.position + statManager.transform.forward * 0.3f + statManager.transform.up * 0.5f, Quaternion.identity);
    }
}   
