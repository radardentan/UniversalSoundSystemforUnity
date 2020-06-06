using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;

public class SoundManager : MonoBehaviour
{
    //シングルトン化
    static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject gameObject = new GameObject("SoundManager");
                _instance = gameObject.AddComponent<SoundManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

    AudioMixer masterBus;
    BGMList bgmList;
    TimelineAsset[] bgmLanes;
    

    [SerializeField] const float duckVolumeRate = 0.5f;

    void SetVolume(float _volume, string _busName)
    {
        float volumeDB = (float)(20.0d * Mathf.Log10(_volume));
        masterBus.SetFloat(_busName, Mathf.Clamp(volumeDB, -80.0f, 0.0f));
    }

    void SetDuckVolume(float _volumeRate)
    {
        float duckVolume = Mathf.Clamp01(_volumeRate * duckVolumeRate);
        float duckVolumeDB = (float)(20.0d * Mathf.Clamp(duckVolume, -80.0f, 0.0f));
        masterBus.SetFloat("DuckVolume", duckVolumeDB);
    }

    SortedList<string, List<AudioSource>> loadedBGMList;
    void BGMLoad(string dictKey) 
    {
        AudioClip[] audioClips = Resources.LoadAll("BGM" + dictKey) as AudioClip[];
        List<AudioSource> audioSources = new List<AudioSource>();
        foreach(AudioClip audioClip in audioClips) 
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSources.Add(audioSource);
        }
        loadedBGMList.Add(dictKey, audioSources);
    }

    int flip = 0;
    void PlayBGM(string dictKey) 
    {
        if(loadedBGMList.Count == 0) //現在BGMが鳴っていないとき
        {

        }
        else 
        {

        }
    }

    void Awake()
    {
        masterBus = Resources.Load("AudioMasterBus") as AudioMixer;
        SetDuckVolume(duckVolumeRate);
        bgmList = Resources.Load("BGM/BGMList") as BGMList;
        bgmLanes = Resources.LoadAll("BGMTimeLline") as TimelineAsset[];
    }

}
