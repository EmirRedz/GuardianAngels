using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannle { Master,Music,Sfx};

    //character_selection character_Selection;

    public float masterVolumePercent { get; private set;}
    public float musicVolumePercent { get; private set;}
    public float sfxVolumePercent { get; private set;}

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    Transform audioListenerTransform;
    Transform playerTransform;
    Sounds sounds;


    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            sounds = GetComponent<Sounds>();
            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music Source" + (i + 1));
                newMusicSource.tag = "MusicSource";
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.SetParent(transform);
            }

            GameObject newSfxSource = new GameObject("Sfx Source");
            sfx2DSource = newSfxSource.AddComponent<AudioSource>();
            newSfxSource.transform.SetParent(transform);

            audioListenerTransform = GetComponentInChildren<AudioListener>().transform;

            masterVolumePercent = PlayerPrefs.GetFloat("MasterVolume", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("MusicVolume", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("SfxVolume", 1);

        }
        
    }
    private void FixedUpdate()
    {
        if (FindObjectOfType<Player>() != null)
        {
            playerTransform = FindObjectOfType<Player>().transform;
        }
        else
        {
            playerTransform = transform;
        }

        if (playerTransform.transform != null)
        {
            audioListenerTransform.position = playerTransform.position;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        if (playerTransform == null)
        {
            if (FindObjectOfType<Player>() != null)
            {
                playerTransform = FindObjectOfType<Player>().transform;
            }
        }
    }

    public void SetVolume(float volumePercent, AudioChannle channle)
    {
        switch (channle)
        {
            case AudioChannle.Master:
                masterVolumePercent = volumePercent;
                break;

            case AudioChannle.Music:
                musicVolumePercent = volumePercent;
                break;

            case AudioChannle.Sfx:
                sfxVolumePercent = volumePercent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("MasterVolume", masterVolumePercent);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumePercent);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolumePercent);
        PlayerPrefs.Save();

    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(sounds.GetAudioFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.clip = sounds.GetAudioFromName(soundName);
        sfx2DSource.Play();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(MusicCrossfade(fadeDuration));
    }

    IEnumerator MusicCrossfade(float duration)
    {
        float percent = 0;
        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent,percent);
            musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent,0,percent);
            yield return null;
        }
    }
}
