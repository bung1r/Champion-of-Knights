using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System.Threading.Tasks;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource deathSFX;
    [SerializeField] private AudioSource hitSFX;
    [SerializeField] private AudioSource parrySFX;
    [SerializeField] private AudioSource swingSFX;
    [SerializeField] private AudioSource thump1SFX;
    [SerializeField] private AudioSource thump2SFX;
    [SerializeField] private AudioSource thump3SFX;
    [SerializeField] private AudioSource hitEnemySFX;
    [SerializeField] private AudioSource guardSFX;
    [SerializeField] private AudioSource footstepGrassSFX;
    [SerializeField] private AudioSource footstepConcreteSFX;
    [SerializeField] private AudioSource footstepEchoSFX;
    [SerializeField] private AudioSource windupSwingSFX;
    [SerializeField] private AudioSource roboticFootstepSFX;
    [SerializeField] private AudioSource windupChargeSFX;
    [SerializeField] private AudioSource styleMeterUpSFX;
    [SerializeField] private AudioSource wrongBuzzerSFX;
    [SerializeField] private AudioSource buyNodeSFX;
    [SerializeField] private AudioSource levelUp;
    [SerializeField] private AudioSource genericMenuClick;

    private List<AudioSource> activeSources = new List<AudioSource>();
    private Dictionary<AudioSource, float> sourceAndPitchDict = new Dictionary<AudioSource, float>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        // add all active sources to the list for pitch variation tracking 
        activeSources.Add(deathSFX);
        activeSources.Add(hitSFX);
        activeSources.Add(parrySFX);
        activeSources.Add(swingSFX);
        activeSources.Add(thump1SFX);
        activeSources.Add(thump2SFX);
        activeSources.Add(thump3SFX);
        activeSources.Add(hitEnemySFX);
        activeSources.Add(guardSFX);
        activeSources.Add(footstepGrassSFX);
        activeSources.Add(footstepConcreteSFX);
        activeSources.Add(footstepEchoSFX);
        activeSources.Add(windupSwingSFX);
        activeSources.Add(roboticFootstepSFX);
        activeSources.Add(windupChargeSFX);
        activeSources.Add(styleMeterUpSFX);
        activeSources.Add(wrongBuzzerSFX);
        activeSources.Add(buyNodeSFX);
        activeSources.Add(levelUp);
        activeSources.Add(genericMenuClick);

        foreach (AudioSource source in activeSources)
        {
            if (source == null) continue;
            sourceAndPitchDict[source] = source.pitch;
        }
    }
    public void PlaySourceAtPointWithPitch(AudioSource source, Vector3 position, float variation = 0.05f)
    {
        if (source != null)
        {
            float basePitch = sourceAndPitchDict[source];
            source.pitch = UnityEngine.Random.Range(basePitch - variation, basePitch + variation);
            source.transform.position = position;
            source.PlayOneShot(source.clip);
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
}


