using System;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start ()
    {
        PlayMusic("TrickyFox");


        foreach (Sound s in sfxSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }

    public void PlaySFX ( string _sfxName )
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == _sfxName);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }

    public void StopSFX ( string _sfxName )
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == _sfxName);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }

    public void PlayMusic ( string _musicName )
    {
        Sound s = Array.Find(musicSounds, sound => sound.name == _musicName);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        musicSource.clip = s.clip;

        musicSource.volume = s.volume;
        musicSource.pitch = s.pitch;

        musicSource.loop = s.loop;

        musicSource.Play();
    }

    public void StopMusic ()
    {
        musicSource.Stop();

    }

}
