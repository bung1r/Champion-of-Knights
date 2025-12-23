using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

[Serializable]
public class StyleHUDManager : MonoBehaviour
{

    public List<StyleEntry> allStyles = new List<StyleEntry>();
    public GameObject listContainer; // assign ts
    public GameObject textPrefab; // assign ts
    public StyleBonusDatabase bonusDatabase; // assign ts
    public GameObject styleText;
    public GameObject viewersText;
    public void AddEntry(StyleBonusTypes bonusType)
    {
        GameObject styleObj = Instantiate(textPrefab, listContainer.transform);
        StyleEntry newEntry = new StyleEntry(bonusDatabase, styleObj, bonusType);
        allStyles.Add(newEntry);
    }
    public void UpdateText(float currStyle, float viewers)
    {
        styleText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Style: {Mathf.FloorToInt(currStyle)}";
        viewersText.GetComponent<TMPro.TextMeshProUGUI>().text = $"Viewers: {viewers}";
    }
}


