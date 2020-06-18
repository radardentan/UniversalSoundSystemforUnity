using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
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
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]public static void LoadInstance() 
    {
        _instance = Instance;
    }

    AudioMixer masterBus;
    

    [SerializeField] const float duckVolumeRate = 0.5f;

    public void SetVolume(float _volume, string _busName)
    {
        float volumeDB = (float)(20.0d * Mathf.Log10(_volume));
        masterBus.SetFloat(_busName, Mathf.Clamp(volumeDB, -80.0f, 0.0f));
    }

    public void SetDuckVolume(float _volumeRate)
    {
        float duckVolume = Mathf.Clamp01(_volumeRate * duckVolumeRate);
        float duckVolumeDB = (float)(20.0d * Mathf.Clamp(duckVolume, -80.0f, 0.0f));
        masterBus.SetFloat("DuckVolume", duckVolumeDB);
    }

    PlayableDirector[] playableDirectors;
    int flip = 0;

    public void BGMPlay(string dictKey) 
    {   
        //再生中のBGMを止める
        
        //BGMを読み込んで再生
        TimelineAsset timeline = SetBGM(dictKey);
        playableDirectors[flip].playableAsset = timeline;
        playableDirectors[flip].Play();
        flip = 1 - flip;
        
    }
    public void BGMStop() 
    {
        //クロスフェードさせて止める
    }

    TimelineAsset SetBGM(string dictKey) 
    {
        //TimelineAssetを何らかの方法で読み込む
        TimelineAsset timeline = null;
        return timeline;

    } 


    void Awake()
    {
        masterBus = Resources.Load("AudioMasterBus") as AudioMixer;
        SetDuckVolume(duckVolumeRate);
        playableDirectors = new PlayableDirector[2];

    }

}
