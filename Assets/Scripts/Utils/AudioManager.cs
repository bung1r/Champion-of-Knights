using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using UnityEditor;
using UnityEngine.Rendering;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] public AudioSource deathSFX;
    [SerializeField] public AudioSource hitSFX;
    [SerializeField] public AudioSource parrySFX;
    [SerializeField] public AudioSource swingSFX;
    [SerializeField] public AudioSource thump1SFX;
    [SerializeField] public AudioSource thump2SFX;
    [SerializeField] public AudioSource thump3SFX;
    [SerializeField] public AudioSource hitEnemySFX;
    [SerializeField] public AudioSource guardSFX;
    [SerializeField] public AudioSource footstepGrassSFX;
    [SerializeField] public AudioSource footstepConcreteSFX;
    [SerializeField] public AudioSource footstepEchoSFX;
    [SerializeField] public AudioSource windupSwingSFX;
    [SerializeField] public AudioSource roboticFootstepSFX;
    [SerializeField] public AudioSource windupChargeSFX;
    [SerializeField] public AudioSource styleMeterUpSFX;
    [SerializeField] public AudioSource wrongBuzzerSFX;
    [SerializeField] public AudioSource nearDeathHeartbeatSFX;
    [SerializeField] public AudioSource healSFX;
    [SerializeField] public AudioSource droneShootSFX;
    [SerializeField] public AudioSource swordWhooshSFX;
    [SerializeField] public AudioSource drinkPotionRegSFX;
    [SerializeField] public AudioSource drinkPotionBigSFX;
    [SerializeField] private AudioSource buyNodeSFX;
    [SerializeField] private AudioSource levelUp;
    [SerializeField] private AudioSource genericMenuClick;
    [SerializeField] private AudioSource battleMusic;
    private float battleMusicMaxVolume;
    [SerializeField] private AudioSource menuMusic;
    private float menuMusicMaxVolume;
    [SerializeField] private AudioSource intermissionMusic;
    private float intermissionMusicMaxVolume;
    private List<AudioSource> activeSources = new List<AudioSource>();
    public Dictionary<string, AudioSource> NameToAudio = new Dictionary<string, AudioSource>();
    private Dictionary<AudioSource, float> sourceAndPitchDict = new Dictionary<AudioSource, float>();
    private Dictionary<AudioSource, string> sourceAndTypeDict = new Dictionary<AudioSource, string>();
    private Dictionary<AudioSource, float> sourceAndVolumeDict = new Dictionary<AudioSource, float>();
    private Dictionary<AudioSource, float> sourceAndCurrentVolumeDict = new Dictionary<AudioSource, float>();

    private List<string> claps = new List<string>() { "Clap2", "Clap3", "Clap4", "Clap5", "Clap6"};


    public float masterVolume = 100f;
    public float musicVolume = 50f;
    public float sfxVolume = 50f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        battleMusicMaxVolume = battleMusic.volume;
        menuMusicMaxVolume = menuMusic.volume;
        intermissionMusicMaxVolume = intermissionMusic.volume;
    }
    public void Start()
    {
        // add all active sources to the list for pitch variation tracking 
        foreach (Transform child in transform)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            if (source != null)
            {
                activeSources.Add(source);
            }
        }

        foreach (AudioSource source in activeSources)
        {
            if (source == null) continue;
            sourceAndPitchDict[source] = source.pitch;
            sourceAndVolumeDict[source] = source.volume;
            NameToAudio[source.gameObject.name] = source;

            if (source.clip.length > 20f)
            {
                sourceAndTypeDict[source] = "music";
                sourceAndCurrentVolumeDict[source] = source.volume;
            }
            else
            {
                sourceAndTypeDict[source] = "sfx";
                sourceAndCurrentVolumeDict[source] = source.volume;
            }
        }

        nearDeathHeartbeatSFX.volume = 0f;
        nearDeathHeartbeatSFX.Play();

    }
    public void PlaySourceAtPointWithPitch(AudioSource source, Vector3 position, float variation = 0.05f)
    {
        if (source != null)
        {
            float basePitch = sourceAndPitchDict[source];
            source.pitch = UnityEngine.Random.Range(basePitch - variation, basePitch + variation);
            float baseVolume = sourceAndVolumeDict[source];
            // if (sourceAndTypeDict[source] == "music")
            // {
            //     source.volume = baseVolume * (musicVolume / 50f) * (masterVolume / 100f);
            // }
            // else
            // {
            //     source.volume = baseVolume * (sfxVolume / 50f) * (masterVolume / 100f);
            // }
            source.transform.position = position;
            source.PlayOneShot(source.clip);
        }
    }
    public async void PlaySourceAtPointWithPitchAsync(AudioSource source, Vector3 position, float delay, float variation = 0.05f)
    {
        await Task.Delay((int)(delay * 1000)); // delay to ensure the source is ready to play
        if (source != null)
        {
            PlaySourceAtPointWithPitch(source, position, variation);
        }
    }
    public AudioSource PlaySourceWithDedicatedAudioSource(AudioSource source, Vector3 position, float variation = 0.05f)
    {
        if (source != null)
        {
            GameObject tempGO = new GameObject("TempAudio"); // create the temp object
            tempGO.transform.position = position; // set its position
            AudioSource tempSource = tempGO.AddComponent<AudioSource>(); // add an audio source
            tempSource.clip = source.clip; // set the clip to play
            tempSource.volume = source.volume;
            tempSource.spatialBlend = source.spatialBlend; // make it 2D sound
            tempSource.dopplerLevel = source.dopplerLevel;
            float basePitch = sourceAndPitchDict[source];
            tempSource.pitch = UnityEngine.Random.Range(basePitch - variation, basePitch + variation);
            tempSource.Play(); // play the clip
            Destroy(tempGO, source.clip.length / tempSource.pitch); // destroy after it finishes playing
            return tempSource;
        }
        return null;
    }

    public void HandleAudioHelpers(List<ItemAudioHelper> audioHelpers, Transform origin)
    {
        foreach (ItemAudioHelper audioHelper in audioHelpers)
        {
            AudioSource source = NameToAudio[audioHelper.audioName];
            if (source != null)
            {
                PlaySourceAtPointWithPitchAsync(source, origin.position, audioHelper.delay, audioHelper.variation);
            }
        }
    }
    public void PlayDeathSFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(deathSFX, origin.position, variation);
    }
    public void PlayGuardSFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(guardSFX, origin.position, variation);
    }
    public void PlayFootstepGrassSFX(Transform origin, float variation = 0.1f)
    {
        PlaySourceAtPointWithPitch(footstepGrassSFX, origin.position, variation);
    }
    public void PlayFootstepConcreteSFX(Transform origin, float variation = 0.1f)
    {
        PlaySourceAtPointWithPitch(footstepConcreteSFX, origin.position, variation);
    }
    public void PlayFootstepEchoSFX(Transform origin, float variation = 0.1f)
    {
        PlaySourceAtPointWithPitch(footstepEchoSFX, origin.position, variation);
    }
    public void PlayHitSFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(hitEnemySFX, origin.position, variation);
    }
    public void PlayParrySFX(Transform origin, float variation = 0.02f)
    {
        PlaySourceAtPointWithPitch(parrySFX, origin.position, variation);
    }
    public void PlayStyleMeterUpSFX(float styleLevel, Transform origin, float variation = 0f)
    {
        sourceAndPitchDict[styleMeterUpSFX] = 1.0f + (styleLevel * 0.1f);
        PlaySourceAtPointWithPitch(styleMeterUpSFX, origin.position, variation);
    }
    public void PlaySwingSFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(swingSFX, origin.position, variation);
    }
    public void PlayWrongBuzzerSFX(Transform origin, float variation = 0f)
    {
        PlaySourceAtPointWithPitch(wrongBuzzerSFX, origin.position, variation);
    }
    public void PlayBuyNodeSFX(Transform origin, float variation = 0f)
    {
        PlaySourceAtPointWithPitch(buyNodeSFX, origin.position, variation);
    }
    public void PlayLevelUpSFX(Transform origin, float variation = 0f)
    {
        PlaySourceAtPointWithPitch(levelUp, origin.position, variation);
    }
    public void PlayGenericMenuClickSFX(Transform origin, float variation = 0f)
    {
        PlaySourceAtPointWithPitch(genericMenuClick, origin.position, variation);
    }
    public void PlayHitWallSFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(thump2SFX, origin.position, variation);
    }
    public void PlayHealSFX(Transform origin, float variation = 0f)
    {
        PlaySourceAtPointWithPitch(healSFX, origin.position, variation);
    }
    public void SetHeartbeatVolume(float hp, float maxhp)
    {
        float ratio = hp / maxhp;
        float targetVolume = (1f - Mathf.Min(1f, ratio * 5f)) * 1f;
        sourceAndCurrentVolumeDict[nearDeathHeartbeatSFX] = targetVolume;
    }
    public void PlayMenuMusic(float fadeInTime)
    {
        StartCoroutine(FadeInMusic(menuMusic, menuMusicMaxVolume, fadeInTime));
    }
    public void DisableMenuMusic(float fadeOutTime)
    {
        StartCoroutine(FadeOutMusic(menuMusic, fadeOutTime));
    }
    public void PlayBattleMusic(float fadeInTime)
    {
        StartCoroutine(FadeInMusic(battleMusic, battleMusicMaxVolume, fadeInTime));
    }
    public void DisableBattleMusic(float fadeOutTime)
    {
        StartCoroutine(FadeOutMusic(battleMusic, fadeOutTime));
    }
    public void PlayIntermissionMusic(float fadeInTime)
    {
        StartCoroutine(FadeInMusic(intermissionMusic, intermissionMusicMaxVolume, fadeInTime));
    }
    public void DisableIntermissionMusic(float fadeOutTime)
    {
        StartCoroutine(FadeOutMusic(intermissionMusic, fadeOutTime));
    }

    IEnumerator FadeInMusic(AudioSource musicSource, float targetVolume, float duration)
    {
        // musicSource.volume = 0;
        
        // float time = 0;
        // while (time < duration)
        // {
        //     time += Time.deltaTime;
        //     sourceAndCurrentVolumeDict[musicSource] = Mathf.Lerp(0, targetVolume, time / duration);
        //     yield return null;
        // }

        // sourceAndCurrentVolumeDict[musicSource] = targetVolume;

        musicSource.Play();
        yield return null;
    }

    IEnumerator FadeOutMusic(AudioSource musicSource, float duration)
    {
        float startVolume = musicSource.volume;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, time / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
    }
    private float lastMasterVolume = 100f;
    private float lastMusicVolume = 50f;
    private float lastSFXVolume = 50f;
    void Update()
    {
        if (lastMasterVolume == masterVolume && lastMusicVolume == musicVolume && lastSFXVolume == sfxVolume) return;
        lastMasterVolume = masterVolume;
        lastMusicVolume = musicVolume;
        lastSFXVolume = sfxVolume;
        foreach (var kvp in sourceAndCurrentVolumeDict)
        {
            if (sourceAndTypeDict[kvp.Key] == "music")
            {
                kvp.Key.volume = kvp.Value * (masterVolume / 100f) * (musicVolume / 50f);
            } else
            {
                kvp.Key.volume = kvp.Value * (masterVolume / 100f) * (sfxVolume / 50f);
            }
        }
    }

    private List<string> lowShouts = new List<string>() { "GoodJob", "FunnyWoohoo", "YouGotThis", "YeahBaby", "DontDieNow" };
    private List<string> midShouts = new List<string>() {"YeahLetsGo", "ThatsMyKnight" };
    private List<string> highshouts = new List<string>() { "GetALoadOfThisGuy", "IBetAllMyMoney" };

    private float clapAudioScaling = 0.21f;
    private float shoutAudioScaling = 0.17f;
    public void PlayAudience(int viewers)
    {
        int clapAudios = 0;
        float cheerDuration = UnityEngine.Random.Range(4f, 5.5f);
        int shoutAudios = 0;
        if (viewers < 1000)
        { // 0 - 999 viewers (Claps: Min 3, Max 5)
            clapAudios = (int)Mathf.Max(6, Mathf.Pow(viewers, clapAudioScaling)+4); // cool formula, min 6 claps. 
            shoutAudios = (int)Mathf.Max(1, Mathf.Pow(viewers, shoutAudioScaling)); // cool formula, min 1 shout
        }
        else if (viewers < 15000) // 1000 - 14999 viewers  (Claps: Min 3, Max 9)
        {
            clapAudios = (int)Mathf.Max(6, Mathf.Pow(viewers, clapAudioScaling) + 4 + UnityEngine.Random.Range(-1f, 1f)); // introduce some variation
            shoutAudios = (int)Mathf.Max(1, Mathf.Pow(viewers, shoutAudioScaling) + UnityEngine.Random.Range(-0.5f, 1f)); // introduce some variation
        }
        else  // 15k viewers+ (Claps: Min 8, Max 12)
        {
            clapAudios = (int)Mathf.Clamp(Mathf.Pow(viewers, clapAudioScaling) + 4 + UnityEngine.Random.Range(-1f, 1.5f), 6, 16); // introduce some variation
            shoutAudios = (int)Mathf.Clamp(Mathf.Pow(viewers, shoutAudioScaling) + UnityEngine.Random.Range(-0.5f, 1f), 1, 10); // introduce some variation
        }

        
        List<int> potentialIndexes = new List<int>{0, 1, 2, 3, 4};
        for (int i = 0; i < clapAudios; i++)
        {
            if (potentialIndexes.Count == 0) potentialIndexes = new List<int>{0,1,2,3,4};
            int potentialIndexIndex = UnityEngine.Random.Range(0, potentialIndexes.Count);
            int index = potentialIndexes[potentialIndexIndex];
            potentialIndexes.RemoveAt(potentialIndexIndex);
            string clapName = claps[index];
            AudioSource clapSource = NameToAudio[clapName];
            float delay = UnityEngine.Random.Range(0f,0.5f);
            StartCoroutine(HandleAudioSource(clapSource, delay, Vector3.zero, 0.15f, Time.fixedDeltaTime + cheerDuration)); // position can be adjusted as needed
        }

        for (int i = 0; i < shoutAudios; i++)
        {
            string shoutName = lowShouts[UnityEngine.Random.Range(0, lowShouts.Count)];
            AudioSource shoutSource = NameToAudio[shoutName];
            float delay = Mathf.Max(0f, UnityEngine.Random.Range(0f, cheerDuration - shoutSource.clip.length + 0.1f));
            StartCoroutine(HandleAudioSource(shoutSource, delay, Vector3.zero, 0.3f, Time.fixedDeltaTime + cheerDuration)); // position can be adjusted as needed
        }
    }

    private IEnumerator HandleAudioSource(AudioSource originalSource, float delay, Vector3 position, float variation, float endTime = -1f, float fadeOutTime = 1.5f)
    {
        // Debug.Log("Consider this audio source: " + originalSource.gameObject.name+ "handled!");
        yield return new WaitForSeconds(delay);
        AudioSource source = PlaySourceWithDedicatedAudioSource(originalSource, position, variation); // position can be adjusted as needed
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (source == null) yield break; // source might have been destroyed if the clip finished playing
            if (endTime - Time.fixedDeltaTime < fadeOutTime)
            {
                source.volume = Mathf.Lerp(originalSource.volume, 0, 1 - (fadeOutTime / (fadeOutTime - Time.fixedDeltaTime)));
            } else
            {
                source.volume = originalSource.volume;
            }

            if (endTime - Time.fixedDeltaTime < 0f)
            {
                source.Stop();
                Destroy(source.gameObject);
                yield break;
            }
        }
       
    }

}


