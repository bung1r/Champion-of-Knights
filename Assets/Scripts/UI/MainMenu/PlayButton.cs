using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public string sceneToLoad = "Game";
    public void OnPlayButtonPressed()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGenericMenuClickSFX(Camera.main.transform);
        SceneManager.LoadScene(sceneToLoad);
    }

}