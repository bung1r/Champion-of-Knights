using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapSceneHandler : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);
    }

    void Start()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}