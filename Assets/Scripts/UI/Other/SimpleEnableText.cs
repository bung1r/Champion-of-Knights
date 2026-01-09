using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleEnableText : MonoBehaviour
{
    public TextMeshProUGUI textObject;
    public float startAlpha = 0f;
    private bool isFading = false;
    private bool isEnabling = false;
    private float actionStart = -999f;
    private float actionDuration = -999f;

    void Start()
    {
        if (textObject == null) textObject = GetComponent<TextMeshProUGUI>();
        if (textObject == null) textObject = GetComponentInChildren<TextMeshProUGUI>();
        if (textObject == null) Debug.LogError("SimpleEnableText: No TextMeshProUGUI found!");
        textObject.alpha = startAlpha;
    }

    void Update()
    {
        if (isFading)
        {
            float elapsed = Time.time - actionStart;
            float t = Mathf.Clamp01(elapsed / actionDuration);
            textObject.alpha = 1f - t;
            if (t >= 1f)
            {
                isFading = false;
            }
        } else if (isEnabling)
        {
            float elapsed = Time.time - actionStart;
            float t = Mathf.Clamp01(elapsed / actionDuration);
            textObject.alpha = t;
            if (t >= 1f)
            {
                isEnabling = false;
            }
        }
    }    

    async public void EnableTextForSeconds(float seconds, float fadeOutDuration = 1f)
    {
        EnableText();
        await System.Threading.Tasks.Task.Delay((int)(seconds * 1000));
        FadeOutText(1f);
    }

    public void EnableText()
    {
        textObject.alpha = 1f;
    }

    public void FadeInText(float fadeDuration)
    {
        isEnabling = true;
        actionStart = Time.time;
        actionDuration = fadeDuration;
    }

    public void DisableText()
    {
        textObject.alpha = 0f;
    }

    public void FadeOutText(float fadeDuration)
    {
        isFading = true;
        actionStart = Time.time;
        actionDuration = fadeDuration;
    }

}