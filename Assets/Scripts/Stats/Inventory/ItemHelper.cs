using System;
using UnityEngine;
using System.Threading.Tasks;

// might be the worst code i've ever written
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon;
    public int uses = 1;
    public float cooldown = 0f; // how long before it can be used again
    public float duration = 1f; // how look it takes for action to complete
    public float performDelay = 0.3f; // delay before perform is called after use
    public virtual bool Use(StatManager statManager = null) {
        Debug.Log("RAHH");
        FullPerform(statManager); return true;
        }
    public virtual bool CanUse() {return false;}
    public virtual float GetCooldownRemaining() {return 10f;}
    public virtual void BeginUse(StatManager statManager = null) {} // this is for a charge (hold down)
    public virtual void WhileUse(StatManager statManager = null) {} // this is for anything while a charged move is happening
    public virtual void EndUse(StatManager statManager = null) {}  
    
    async private void FullPerform(StatManager statManager = null)
    {
        await Task.Delay((int)(performDelay * 1000));
        Perform(statManager);
    } // DO NOT OVERRIDE THIS UNLESS NECESSARY, BRO!
    public virtual void Perform(StatManager statManager = null) {} // called after perform delay
}

public class ItemRuntime
{
    private Item item; 
    public int usesRemaining = 1;
    public float lastUsedTime = -999999f;
    public ItemRuntime(Item otherItem)
    {
        item = otherItem;
        usesRemaining = otherItem.uses;
    }
    public virtual bool CanUse(StatManager statManager = null)
    {
        if (usesRemaining <= 0) return false;
        if (Time.time - lastUsedTime < item.cooldown) return false;
        return true;
    }
    public virtual bool Use(StatManager statManager = null)
    {
        if (!CanUse()) return false;
        usesRemaining -= 1;
        lastUsedTime = Time.time;
        item.Use(statManager);
        return true;
    }
    public virtual float GetCooldownRemaining(StatManager statManager = null) {return 0f;}
    public virtual void BeginUse(StatManager statManager = null)
    {
        if (!CanUse()) return;
        item.BeginUse(statManager);
    } 
    public virtual void WhileUse(StatManager statManager = null)
    {
        item.WhileUse(statManager);
    } 
    public virtual void EndUse(StatManager statManager = null)
    {
        item.EndUse(statManager);
        
    } 
   
}

[Serializable]
public class ItemPair
{
    public Item item;
    public ItemRuntime runtime;
    public ItemPair(Item newItem)
    {
        item = newItem;
        runtime = new ItemRuntime(item);
        runtime.usesRemaining = newItem.uses;
    }
    public void UpdateItem(Item newItem)
    {
        item = newItem;
        if (newItem == null)
        {
            runtime = null;
            return;
        }
        runtime = new ItemRuntime(item);
    }
}