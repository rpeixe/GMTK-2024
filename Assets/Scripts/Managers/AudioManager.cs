using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] public AudioSource _musicSource;
    [SerializeField] public AudioSource _sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != _musicSource.clip)
        {
            _musicSource.Stop();
            _musicSource.clip = clip;
            _musicSource.Play();
        }
    }

    public void PlayEffect(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }

    public void ToggleSfx()
    {
        _sfxSource.mute = !_sfxSource.mute;
    }
}