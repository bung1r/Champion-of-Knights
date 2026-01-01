using UnityEngine;

public class MasterStuff : MonoBehaviour
{
    // basically, checks for existing instances of managers that should be singletons
    [SerializeField] private AudioManager audioManagerPrefab;
    void Awake()
    {
        if (AudioManager.Instance == null)
        {
            Instantiate(audioManagerPrefab);
        }
    }
}
