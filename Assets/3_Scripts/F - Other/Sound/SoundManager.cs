using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public static GameObject soundObjectPref;

    private void Awake()
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

        soundObjectPref = Resources.Load<GameObject>("SoundObject");
    }

    public static void PlayMusic(GameAssets.MusicSound music)
    {
        //GameObject soundGameObject = Instantiate(soundObjectPref);
        //AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();
        //MusicAudioClip musicAudioClip = GetMusicAudioClip(music);
        //audioSource.volume = musicAudioClip.baseVolume;
        //audioSource.PlayOneShot(musicAudioClip.audioClip);
        //soundGameObject.GetComponent<SoundObject>().DelayedDestroy(musicAudioClip.audioClip.length + 0.5f);
    }

    public static void PlayEffect(AudioClipData data)
    {
        if (data == null) return;
        if (data.audioClipName == "" || data.audioClipName == string.Empty) return;

        GameObject soundGameObject = Instantiate(soundObjectPref);
        AudioSource audioSource = soundGameObject.GetComponent<AudioSource>();
        AudioClip audioClip = Resources.Load<AudioClip>(data.audioClipName);
        audioSource.volume = data.volume;
        audioSource.PlayOneShot(audioClip);
        soundGameObject.GetComponent<SoundObject>().DelayedDestroy(audioClip.length + 0.5f);
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void ToggleEffects()
    {
        // needs to be implemented sometime :)
        ///effectSource.mute = !effectSource.mute;
    }

    public void ToggleMusic()
    {
        // musicSource.mute = !musicSource.mute;
    }
}

[Serializable]
public class AudioClipData
{
    public float volume = 1f;
    public string audioClipName;
}
