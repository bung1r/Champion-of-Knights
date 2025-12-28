using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;

[Serializable]
public class Resistances
{
    
    public List<ResistanceEntry> entries = new List<ResistanceEntry>();
    public float Get(DamageType damageType) 
    {
        float total = 0;
        foreach (ResistanceEntry entry in entries)
        {
            if (entry.type == damageType)
            {
                total+=entry.resistance;
            }          
        }
        if (total > 0)
        {
            return total;
        } else
        {
            return 1f;
        }
    }

    public void AddEntry(ResistanceEntry entry)
    {
        entries.Add(entry);
    }

// removes entry using when it was created
    public void RemoveEntry(float timeCreated)
    {
        int i = 0;
        foreach(ResistanceEntry entry in entries)
        {
            if (entry.timeCreated == timeCreated) break;
            i++;
        }

        entries.RemoveAt(i);
    }
// removes entry using the actual object reference
    public void RemoveEntry(ResistanceEntry target)
    {
        entries.Remove(target);
    }
    public Resistances() {}

    public Resistances(Resistances other) 
    {
        foreach(ResistanceEntry entry in other.entries)
        {
            entries.Add(new ResistanceEntry(entry));
        }
    }
}

[Serializable]
public class ResistanceEntry
{
    public DamageType type;
    public float resistance;
    [NonSerialized] public float timeCreated;
    public ResistanceEntry()
    {
    }
    public ResistanceEntry(ResistanceEntry entry)
    {
        type = entry.type;
        resistance = entry.resistance;
        timeCreated = Time.time;
    }
    public ResistanceEntry(GuardStats guardStats)
    {
        type = guardStats.resistance;
        resistance = guardStats.resistAmount;
        timeCreated = Time.time;
    }
}