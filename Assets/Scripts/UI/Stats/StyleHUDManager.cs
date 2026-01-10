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
    public StyleBonusDatabase bonusDatabase; // assign ts\
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI styleText;
    public TextMeshProUGUI viewersText;
    public TextMeshProUGUI reputationText;
    public TextMeshProUGUI corruptionText;
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
    public void AddEntry(StyleBonusTypes bonusType, int mult = 1)
    {
        GameObject styleObj = Instantiate(textPrefab, listContainer.transform);
        StyleEntry newEntry = new StyleEntry(bonusDatabase, styleObj, bonusType, mult);
        allStyles.Add(newEntry);
    }
    public void UpdateText(float currStyle, float viewers, float styleLevel, float reputation, float corruption)
    {
        if (styleLevel == 0) gradeText.text = "F";
        else if (styleLevel == 1) gradeText.text = "D";
        else if (styleLevel == 2) gradeText.text = "C";
        else if (styleLevel == 3) gradeText.text = "B";
        else if (styleLevel == 4) gradeText.text = "A";
        else if (styleLevel == 5) gradeText.text = "S";
        else if (styleLevel == 6) gradeText.text = "X";
        
        // styleText.text = $"Style: {Mathf.FloorToInt(currStyle)}";
        viewersText.text = $"Viewers: {Mathf.FloorToInt(viewers)}";
        
        reputationText.text = $"Rep: {Mathf.FloorToInt(reputation)}";
        if (reputation >= 0)
        {
            reputationText.color = new Color(1f - reputation/100f, 1f, 1f - reputation/100f);
        } else
        {
            reputationText.color = new Color(1f, 1f - Math.Abs(reputation)/100f, 1f - Math.Abs(reputation)/100f);
        }

        corruptionText.text = $"Corruption: {Mathf.FloorToInt(corruption)}";
        corruptionText.color = new Color(1f, 1f - corruption/100f, 1f - corruption/100f);
    }
}


