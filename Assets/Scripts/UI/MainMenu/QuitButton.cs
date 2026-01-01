using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGenericMenuClickSFX(Camera.main.transform);
        Application.Quit();
    }
}