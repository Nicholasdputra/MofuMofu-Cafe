using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound
{
    public string audioName;
    public AudioClip clip;
    public bool loop = false;
    public float startTime = 0f;
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

        PlayMusic(SceneManager.GetActiveScene().name); // Play music for the current scene
    }

    public void PlayMusic(string name)
    {
        Sound sound = System.Array.Find(musicClips, s => s.audioName == name);
        if (sound != null)
        {
            musicSource.clip = sound.clip;
            musicSource.loop = sound.loop;
            musicSource.time = sound.startTime;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + name);
        }
    }

    public void PlayOneShotSFX(string audioName)
    {
        Sound sound = System.Array.Find(sfxClips, s => s.audioName == audioName);
        if (sound != null)
        {
            sfxSource.PlayOneShot(sound.clip);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + audioName);
        }
    }

    public void PlaySFX(string audioName)
    {
        Sound sound = System.Array.Find(sfxClips, s => s.audioName == audioName);
        if (sound != null)
        {
            sfxSource.clip = sound.clip;
            sfxSource.loop = sound.loop; ;
            sfxSource.time = sound.startTime;
            sfxSource.Play();
        }
        else
        {
            Debug.LogWarning("Looping SFX clip not found: " + audioName);
        }
    }

    public void DimAudio(bool status)
    {
        lowPassFilter.enabled = status;
    }
}
