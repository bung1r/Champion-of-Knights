using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsButton : MonoBehaviour
{
    public Canvas creditsCanvas;
    public Canvas mainMenuCanvas;
    public void OnCreditButtonPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGenericMenuClickSFX(Camera.main.transform);
        creditsCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
    }

}