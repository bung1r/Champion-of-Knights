using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RoundSummaryManagerUI : MonoBehaviour
{
    private Canvas roundSummaryCanvas;
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private TextMeshProUGUI totalKillsText;
    [SerializeField] private TextMeshProUGUI totalParriesText;
    [SerializeField] private TextMeshProUGUI highestGradeText;
    [SerializeField] private TextMeshProUGUI highestViewerCountText;
    [SerializeField] private TextMeshProUGUI verdictText;
    [SerializeField] private TextMeshProUGUI flavorText;
    private List<string> positiveFlavorTexts = new List<string>()
    {
        "You feel one step closer to making it back home!",
        "The crowd cheering makes you stronger!",
        "A sense of pride fills your heart with this victory!",
        "The thrill you once sought for as a knight, you feel now with an audience cheering your name!",
        "You feel as though everything led you to this moment, yet a feeling of homesickness lingers..."
    };
    private List<string> negativeFlavorTexts = new List<string>()
    {
        "What happens now? You certainly don't know.",
        "It's... over. Maybe you'll have another chance. Or not.",
        "You're finished. What comes next is uncertain.",
        "Failure is a stepping stone to success... or so they say.",
        "Your dream of getting back home seems even more distant now."
    };
    void Start()
    {
        roundSummaryCanvas = GetComponentInParent<Canvas>();
    }
    
    public void UpdateObjectives(int completed, int total)
    {
        objectivesText.text = "Objectives: " + completed.ToString() + " / " + total.ToString();
        if (completed >= total)
        {
            objectivesText.color = Color.green;
            objectivesText.text += " - - - PASS!!!";
            verdictText.text = "Congratulations! You have completed all objectives, and have passed this round!";
            flavorText.text = positiveFlavorTexts[Random.Range(0, positiveFlavorTexts.Count)];
        }
        else
        {
            objectivesText.color = Color.red;
            objectivesText.text += " - - - FAIL!!!";
            verdictText.text = "Unfortunately, you have failed to complete all objectives for this round. There will be consequences.";
            flavorText.text = negativeFlavorTexts[Random.Range(0, negativeFlavorTexts.Count)];
        }
    }
    public void UpdateKills(int kills)
    {
        totalKillsText.text = "Total Kills: " + kills.ToString();
    }
    public void UpdateParries(int parries)
    {
        totalParriesText.text = "Total Parries: " + parries.ToString();
    }
    public void UpdateGrade(string grade)
    {
        highestGradeText.text = "Highest Grade: " + grade;
    }
    public void UpdateViewerCount(int viewerCount)
    {
        highestViewerCountText.text = "Highest Viewer Count: " + viewerCount.ToString();
    }

    async public void EnableAfterDelay(float delaySeconds)
    {
        await Task.Delay((int)(delaySeconds * 1000));
        if (roundSummaryCanvas == null) return;
        roundSummaryCanvas.enabled = true;
    }
    async public void DisableAfterDelay(float delaySeconds)
    {
        await Task.Delay((int)(delaySeconds * 1000));
        if (roundSummaryCanvas == null) return;
        roundSummaryCanvas.enabled = false;
    }
    
}