using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public string sceneToLoad = "Game";
    public void OnPlayButtonPressed()
    {
        Debug.Log("Loading scene: " + sceneToLoad);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayGenericMenuClickSFX(Camera.main.transform);
        SceneManager.LoadScene(sceneToLoad);
    }

}