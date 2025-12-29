using System;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class ObjectiveUIElement : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;
    public Objective objective;
    public string foundationString;
    // public float updateTime = 0.5f;
    // private float timer = 0f;
    public void Start()
    {
        if (objectiveText == null)
        {
            objectiveText = GetComponentInChildren<TextMeshProUGUI>();  
        }
    }
    public void Update()
    {
        if (objective == null) return;
        objectiveText.text = objective.UIString(foundationString);
        
    }

}