using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance; //assign in inspector
    private AudioManager audioManager; 
    [SerializeField] private TextMeshProUGUI masterVolumeText; //assign in inspector
    private Slider masterVolumeSlider; 
    private TextMeshProUGUI masterVolumeNumber;
    [SerializeField] private TextMeshProUGUI musicVolumeText; //assign in inspector
    private Slider musicVolumeSlider; 
    private TextMeshProUGUI musicVolumeNumber;
    [SerializeField] private TextMeshProUGUI sfxVolumeText; //assign in inspector
    private Slider sfxVolumeSlider; 
    private TextMeshProUGUI sfxVolumeNumber;
    public bool epilepsyMode = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject.transform.root);
        Instance = this;
    }
    void Start()
    {
        audioManager = AudioManager.Instance;
        masterVolumeSlider = masterVolumeText.GetComponentInChildren<Slider>();
        masterVolumeNumber = GetTextInChildren(masterVolumeText.transform);
        musicVolumeSlider = musicVolumeText.GetComponentInChildren<Slider>();
        musicVolumeNumber = GetTextInChildren(musicVolumeText.transform);
        sfxVolumeSlider = sfxVolumeText.GetComponentInChildren<Slider>();
        sfxVolumeNumber = GetTextInChildren(sfxVolumeText.transform);
    }

    void Update()
    {
        if (masterVolumeSlider != null)
        {
            audioManager.masterVolume = masterVolumeSlider.value;
            masterVolumeNumber.text = masterVolumeSlider.value.ToString()+"/100";
        }
        if (musicVolumeSlider != null)
        {
            audioManager.musicVolume = musicVolumeSlider.value;
            musicVolumeNumber.text = musicVolumeSlider.value.ToString()+"/100";
        }
        if (sfxVolumeSlider != null)
        {
            audioManager.sfxVolume = sfxVolumeSlider.value;
            sfxVolumeNumber.text = sfxVolumeSlider.value.ToString()+"/100";
        }
    }
    public void ChangeEpilepsy(bool enable)
    {
        epilepsyMode = enable;
    }
    TextMeshProUGUI GetTextInChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            TextMeshProUGUI comp = child.GetComponent<TextMeshProUGUI>();
            if (comp != null)
            {
                return comp;
            }
        }
        return null;
    }
}