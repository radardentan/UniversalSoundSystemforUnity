using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    void Awake()
    {
        masterBus = Resources.Load("AudioMasterBus") as AudioMixer;
        SetDuckVolume(duckVolumeRate);
        bgmList = Resources.Load("BGM/BGMList") as BGMList;
    }
    void Start()
    {

    }
}
