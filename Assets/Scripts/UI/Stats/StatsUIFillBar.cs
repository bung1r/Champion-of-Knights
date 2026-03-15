using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class StatsUIBarFill : MonoBehaviour
{
    public Image bar;
    private float targetValue = 0f;
    void Start()
    {
        bar = GetComponent<Image>();
    }

    void Update()
    {
       bar.fillAmount = Mathf.Lerp(bar.fillAmount, targetValue, Time.deltaTime * 20f);
    }

    public void UpdateBar(float min, float max)
    {
        float ratio = min / max;
        targetValue = ratio / 2f; // from 0 to 1 -> 0 to 0.5
    }

}
