using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public float volume = 1f;
    public float pitch = 1f;
    public bool loop = false;

    public Sound(AudioClip clip, float volume = 1f, float pitch = 1f, bool loop = false)
    {
        this.clip = clip;
        this.volume = volume;
        this.pitch = pitch;
        this.loop = loop;
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioLowPassFilter lowPassFilter;

    [Header("Audio Clips")]
    public Sound[] musicClips;
    public Sound[] sfxClips;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        lowPassFilter.enabled = false; // Ensure low-pass filter is disabled at start
    }

    public void DimAudio(bool status)
    {
        lowPassFilter.enabled = status;
    }
}
