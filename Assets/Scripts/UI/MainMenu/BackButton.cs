using UnityEngine;

public class BackButton : MonoBehaviour
{
    public Canvas canvasToClose;
    public Canvas canvasToOpen;

    public void OnBackButtonPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGenericMenuClickSFX(Camera.main.transform);
        canvasToClose.enabled = false;
        canvasToOpen.enabled = true;
    }
}