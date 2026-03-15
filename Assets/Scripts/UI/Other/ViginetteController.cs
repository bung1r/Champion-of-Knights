using UnityEngine;
using UnityEngine.UI;

public class ViginetteController : MonoBehaviour
{
    public Image viginetteImage;
    public static ViginetteController Instance;
    private float targetAlpha = 0f;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
    }
    void Start()
    {
        viginetteImage = GetComponent<Image>();
    }
    
    public void SetViginette(float hp, float maxhp)
    {
        float ratio = hp / maxhp;
        targetAlpha = 1f - Mathf.Min(1f, ratio * 5f);
        float currentAlpha = Mathf.Lerp(viginetteImage.color.a, targetAlpha, 0.3f);
        viginetteImage.color = new Color(viginetteImage.color.r, viginetteImage.color.g, viginetteImage.color.b, currentAlpha);
    }
}