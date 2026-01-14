
using UnityEngine;
public class BlackScreen : MonoBehaviour
{
    public static BlackScreen Instance;
    private UnityEngine.UI.Image image;
    private bool isFadingToBlack = false;
    private float startFadeToBlack = -1f;
    private bool isFadingFromBlack = false;
    private float startFadeFromBlack = -1f;
    private float fadeDuration = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        image = GetComponent<UnityEngine.UI.Image>();
    }
    
    void Update()
    {
        if (isFadingToBlack)
        {
            float t = (Time.time - startFadeToBlack) / fadeDuration;
            if (t >= 1f)
            {
                t = 1f;
                isFadingToBlack = false;
            }
            image.color = new Color(0, 0, 0, t);
        }

        if (isFadingFromBlack)
        {
            float t = (Time.time - startFadeFromBlack) / fadeDuration;
            if (t >= 1f)
            {
                t = 1f;
                isFadingFromBlack = false;
            }
            image.color = new Color(0, 0, 0, 1f - t);
        }
    }
    public void FadeToBlack(float timeInSeconds)
    {
        // set up
        isFadingToBlack = true;
        isFadingFromBlack = false;

        startFadeToBlack = Time.time;
        fadeDuration = timeInSeconds;
    }

    public void FadeFromBlack(float timeInSeconds)
    {
        if (image.color.a <= 0f) return;
        isFadingFromBlack = true;
        isFadingToBlack = false;

        startFadeFromBlack = Time.time;
        fadeDuration = timeInSeconds;
    }
    public async void FadeToBlackWithDelay(float delaySeconds, float timeInSeconds)
    {
        await System.Threading.Tasks.Task.Delay((int)(delaySeconds * 1000));
        FadeToBlack(timeInSeconds);
    }
    public async void FadeFromBlackWithDelay(float delaySeconds, float timeInSeconds)
    {
        await System.Threading.Tasks.Task.Delay((int)(delaySeconds * 1000));
        FadeFromBlack(timeInSeconds);
    }

}