using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using UnityEngine;

public class RoundManagerUI : MonoBehaviour
{
    private RoundManager roundManager;
    [SerializeField] private TextMeshProUGUI roundTimer;
    [SerializeField] private ObjectiveUIManager objectiveUIManager;
    void Start()
    {
        roundManager = RoundManager.Instance;
        if (roundManager == null)
        {
            Debug.LogError("RoundManager instance not found!");
        }
        roundManager.AssignRoundManagerUIManager(this);
    }   

    public void UpdateTimer(float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        roundTimer.text = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);
    }

    public void AddEntry(ObjectiveScaling objScaling, Objective objective)
    {
        objectiveUIManager.AddEntry(objScaling, objective);
    }

    public void ClearEntries()
    {
        objectiveUIManager.ClearAllEntries();
    }
}