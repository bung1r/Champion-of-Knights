using System;
using System.Collections.Generic;
using TMPro;
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
    void Update()
    {
        for (int i = allStyles.Count - 1; i >= 0; i--)
        {
            if (Time.time - allStyles[i].timeCreated >= 2f && allStyles[i].styleObj.TryGetComponent<TextMeshProUGUI>(out var text))
            {
                float alpha = Mathf.Lerp(1f, 0f, (Time.time - allStyles[i].timeCreated - 2f) / 2f);
                var color = text.color;
                color.a = alpha;
                text.color = color;
            }

            if (Time.time - allStyles[i].timeCreated >= 4f)
            {
                Destroy(allStyles[i].styleObj);
                allStyles.RemoveAt(i);
            }
        }
    } 
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


