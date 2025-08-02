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

    [Range(0f, 1f)]
    public float volume = 1f;
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
            musicSource.volume = sound.volume;
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
            sfxSource.volume = sound.volume;
            sfxSource.Play();
        }
        else
        {
            Debug.LogWarning("Looping SFX clip not found: " + audioName);
        }
    }

    public void StopSFX()
    {
        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void DimAudio(bool status)
    {
        lowPassFilter.enabled = status;
    }
}
