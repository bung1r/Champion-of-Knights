using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    public Canvas settingsCanvas;

    void Start()
    {
        settingsCanvas = SettingsManager.Instance.transform.root.GetComponent<Canvas>();
    }
    public void OnSettingsButtonPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGenericMenuClickSFX(Camera.main.transform);
        if (settingsCanvas != null) settingsCanvas.enabled = !settingsCanvas.enabled;
    }
}