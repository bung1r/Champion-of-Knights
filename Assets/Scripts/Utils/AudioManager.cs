using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource deathSFX;
    [SerializeField] private AudioSource hitSFX;
    [SerializeField] private AudioSource parrySFX;
    [SerializeField] private AudioSource swingSFX;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySourceAtPointWithPitch(AudioSource source, Vector3 position, float variation = 0.05f)
    {
        float basePitch = source.pitch;
        source.pitch = UnityEngine.Random.Range(basePitch - variation, basePitch + variation);
        AudioSource.PlayClipAtPoint(source.clip, position);
        source.pitch = basePitch;
    }
    public void PlayDeathSFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(deathSFX, origin.position, variation);
    }
    public void PlayHitSFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(hitSFX, origin.position, variation);
    }

    public void PlayParrySFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(parrySFX, origin.position, variation);
    }

    public void PlaySwingSFX(Transform origin, float variation = 0.05f)
    {
        PlaySourceAtPointWithPitch(swingSFX, origin.position, variation);
    }
    
}