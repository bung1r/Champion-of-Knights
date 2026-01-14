using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
[CreateAssetMenu(menuName = "RoundManager/Viewer Item Database")]
public class ViewerItemDatabase : ScriptableObject
{
    public List<DatabaseItemData> items;
    public List<Item> GetAllItems()
    {
        List<Item> itemList = new List<Item>();
        foreach (DatabaseItemData data in items)
        {
            itemList.Add(data.item);
        }
        return itemList;
    }
    public Item GetAudienceItem(float viewers, float sponsors)
    {
        List<Item> itemList = new List<Item>();
        List<float> weightList = new List<float>();
        foreach (DatabaseItemData data in items)
        {
            itemList.Add(data.item);
            float calculatedWeight = data.baseWeight + (viewers * data.viewerScaling) + (sponsors * data.sponsorScaling);
            calculatedWeight = Mathf.Min(calculatedWeight, data.maxWeight);
            weightList.Add(calculatedWeight);
        }

        return WeightedRandom.Choose(itemList, weightList);
    }
}