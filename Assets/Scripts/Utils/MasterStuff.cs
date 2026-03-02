using UnityEngine;

public class MasterStuff : MonoBehaviour
{
    // basically, checks for existing instances of managers that should be singletons
    [SerializeField] private AudioManager audioManagerPrefab;
    [SerializeField] private SettingsManager settingsManagerPrefab;
    void Awake()
    {
        if (AudioManager.Instance == null)
        {
            Instantiate(audioManagerPrefab);
        }
        
        if (SettingsManager.Instance == null)
        {
            Instantiate(settingsManagerPrefab);
        }
    }
}
