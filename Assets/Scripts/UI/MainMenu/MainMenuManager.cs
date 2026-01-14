using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("HELP!!");
        AudioManager.Instance.PlayMenuMusic(3.0f);
    }
}