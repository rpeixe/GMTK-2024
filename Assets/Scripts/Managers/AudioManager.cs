using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] public AudioSource _musicSource;
    [SerializeField] public AudioSource _sfxSource;
    [SerializeField] public List<AudioClip> musicList;
    public string sfxPath = "file://"+Application.dataPath+"/Audio/Sfx/";
    public int currentMusic = 0;

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

    private void Start()
    {
        StartCoroutine(MusicLoop());
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

    public void PlayEffect(AudioClip clip, float volume)
    {
        float temp = _sfxSource.volume;
        _sfxSource.volume = volume;
        _sfxSource.PlayOneShot(clip);
        _musicSource.volume = temp;
    }


    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }

    public void ToggleSfx()
    {
        _sfxSource.mute = !_sfxSource.mute;
    }

    private IEnumerator MusicLoop()
    {
        while (true)
        {
            PlayMusic(musicList[currentMusic]);
            while (_musicSource.isPlaying)
            {
                yield return new WaitForSeconds(1f);
            }
            currentMusic = (currentMusic + 1) % musicList.Count;
        }
    }

    public async Task<AudioClip> GetSfx(string fileName)
    {
        return await GetAudioClip(sfxPath+fileName, AudioType.WAV);
    }

    public async Task<AudioClip> GetAudioClip(string filePath, AudioType fileType)
    {
        Debug.Log(filePath);

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filePath, fileType))
        {
            var result = www.SendWebRequest();

            while (!result.isDone) { await Task.Delay(100); }

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
                return null;
            }
            else
            {
                return DownloadHandlerAudioClip.GetContent(www);
            }
        }
    }


}