using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;

[Serializable]
public class Resistances
{
    
    public List<ResistanceEntry> entries = new List<ResistanceEntry>();
    public float Get(DamageType damageType) 
    {
        foreach (ResistanceEntry entry in entries)
        {
            if (entry.type == damageType)
            {
                return entry.resistance;
            }          
        }
        return 1f;
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
    public ResistanceEntry() {}
    public ResistanceEntry(ResistanceEntry entry)
    {
        type = entry.type;
        resistance = entry.resistance;
    }
}