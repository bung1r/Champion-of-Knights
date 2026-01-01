using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    public static Timer Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }

        Instance = this;
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateTimer(float timeInSeconds)
    {
        
    }
    public void EnableTimer()
    {
        textMesh.alpha = 1f;
    }
    public void DisableTimer()
    {
        textMesh.alpha = 0f;
    }
}