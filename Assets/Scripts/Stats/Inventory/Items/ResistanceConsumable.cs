using System;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Items/ResistancePotion")]
[Serializable]
public class ResistanceConsumable : Item
{
    public ResistanceEntry resistanceEntry;
    public float resistanceLength;
    
    public override async void Perform(StatManager statManager)
    {
        float timeCreated = Time.time;
        statManager.GetStats().resistances.AddEntry(new ResistanceEntry
        {
            type = resistanceEntry.type,
            resistance = resistanceEntry.resistance,
            timeCreated = timeCreated
        });
        await Task.Delay((int)(resistanceLength * 1000));
        if (statManager != null && statManager.GetStats() != null)
        {
            statManager.GetStats().resistances.RemoveEntry(timeCreated);
        }
    }
}