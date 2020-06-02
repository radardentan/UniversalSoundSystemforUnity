using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{

    //シングルトン化
    static SoundManager _instance;
    public static SoundManager Instance {
        get {
            if(_instance == null) {
                GameObject gameObject = new GameObject("SoundManager");
                _instance = gameObject.AddComponent<SoundManager>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }

    AudioMixer masterBus;

    AudioConfiguration config = AudioSettings.GetConfiguration();
    double Latency => config.dspBufferSize / config.sampleRate; 

    //BGM・SEリストのマスタデータ
    BGMEntity BGMList { get; set; }
    SEEntity SEList { get; set; }


    //ボリュームコンフィグ
    [SerializeField] const float duckVolumeRate = 0.5f;

    /// <summary>
    /// 第1引数に最大音量に対する音量の割合、第2引数に音量を設定する対象のオーディオミキサーグループ(のExposedParameterの名前)を指定し、
    /// 音量を指定の状態に設定するメソッドです。
    /// 例えば、第1引数に0.5f,第2引数にMasterVolumeを指定すると、ゲーム全体の音量を半分にすることができます。
    /// </summary>
    /// <param name="_volume">最大音量を1とした音量の割合(0~1)</param>
    /// <param name="_busName">音量を設定したいオーディオミキサーグループ（グループ名ではなく、設定済みのExposedParameterの名前）</param>
    public void SetVolume(float _volume, string _busName)
    {
        float volumeDB = (float)(20.0d * Mathf.Log10(_volume));
        masterBus.SetFloat(_busName , Mathf.Clamp(volumeDB, -80.0f, 0.0f));
    }

    /// <summary>
    /// 大量に効果音を鳴らした場合の音割れを防ぐ手段として、効果音に合わせてBGMの音量を一時的に下げることが一般的です。
    /// この手法は「ダッキング」と呼ばれます。
    /// SetDuckVolumeメソッドでは、ダッキングの際にBGMの音量が減少する割合を設定します。
    /// ゲーム全体の初期設定として、SoundManagerクラスのシングルトンインスタンス生成時にこのメソッドが呼び出されます。
    /// （そのとき第1引数にはSoundManagerクラスの変数duckVolumeRateの値が使用されます。）
    /// </summary>
    /// <param name="_volumeRate">ダッキング処理の際にBGMの音量が減少する割合(0~1)</param>
    public void SetDuckVolume(float _volumeRate)
    {
        float duckVolume = Mathf.Clamp01(_volumeRate * duckVolumeRate);
        float duckVolumeDB = (float)(20.0d * Mathf.Clamp(duckVolume, -80.0f, 0.0f));
        masterBus.SetFloat("DuckVolume", duckVolumeDB);
    }

    //BGMグループの読み込み・破棄
    Dictionary<string, List<AudioSource>> loadedBGMList = new Dictionary<string, List<AudioSource>>();


    public string[] LoadedBGMSongTitle => BGMList.Params.Keys.Where(x => loadedBGMList.Keys.Contains(x)).ToArray();
    public string[] LoadedBGMdictKeys => loadedBGMList.Keys.ToArray();

    /// <summary>
    /// 第1引数で指定したフォルダ内にある全てのオーディオファイルを読み込み、
    /// 自動的にオーディオソースと紐付けるメソッドです。
    /// 読み込んだデータを破棄する場合にはBGMUnLoadメソッドを使用してください。
    /// </summary>
    /// <param name="dictKey">読み込みたいファイル群を含むフォルダの名前</param>
    public void BGMLoad(string dictKey)
    {
        AudioClip[] bgmAudioClip = Resources.LoadAll("BGM/" + dictKey) as AudioClip[];
        List<AudioSource> bgmAudioSources = new List<AudioSource>();
        foreach(AudioClip audioClip in bgmAudioClip)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = audioClip;
            bgmAudioSources.Add(audioSource);
            List<AudioClip> sectionMakers = new List<AudioClip>();

        }
        loadedBGMList.Add(dictKey, bgmAudioSources);
       

        loadedBGMList[dictKey][0].time = BGMList.Params[dictKey].loopPoint;
    }

    /// <summary>
    /// 第1引数で指定された一連のオーディオファイル群と、それらに紐付いているオーディオソースをシーンから破棄します。
    /// シーン上のBGMを一括で全て破棄したい場合は、BGMUnLoadAllメソッドを使用してください。
    /// </summary>
    /// <param name="dictKey">メモリから破棄したいファイル群を管理する名前(オーディオファイルのフォルダ名)</param>
    public void BGMUnLoad(string dictKey)
    {
        if (loadedBGMList.ContainsKey(dictKey))
        {
            List<AudioSource> targetGroup = loadedBGMList[dictKey];
            foreach (AudioSource audioSource in targetGroup)
            {
                Resources.UnloadAsset(audioSource.clip);
                Destroy(audioSource);
            }
            loadedBGMList.Remove(dictKey);
        }
    }



    /// <summary>
    /// 現在読み込まれているオーディオフォルダとそれらに紐付いたオーディオソースを一括で破棄するメソッドです。
    /// </summary>
    public void BGMUnLoadAll()
    {
        foreach(string dictKey in loadedBGMList.Keys)
        {
            BGMUnLoad(dictKey);
        }
    }

    //再生・一時停止・再生再開
    //再生
    public void PlayBGM(string dictkey)
    {
        if (loadedBGMList.Count == 0)
        {
            BGMLoad(dictkey);
            loadedBGMList[dictkey][0].PlayScheduled(AudioSettings.dspTime + Latency);
        }
        else
        {
            //既に再生されているBGMの情報
            string target = loadedBGMList.Keys.ToArray()[0];
            int bpm = BGMList.Params[target].BPM;
            int beats = BGMList.Params[target].numBeatsPerSegment[0];
            double nextEventTime = beats * 60.0d / bpm;

            BGMLoad(dictkey);
            loadedBGMList[dictkey][0].PlayScheduled(AudioSettings.dspTime + Latency + nextEventTime);
            BGMUnLoad(target);
        }       
    }

    public void BGMPause()
    {
        for (int i = 0; i < loadedBGMList.Values.Count; i++)
        {
            foreach (AudioSource audioSource in loadedBGMList.Values.ToArray()[i])
            {
                audioSource.Pause();
            }
        }
    }

    public void BGMUnPause()
    {
        for (int i = 0; i < loadedBGMList.Values.Count; i++)
        {
            foreach (AudioSource audioSource in loadedBGMList.Values.ToArray()[i])
            {
                audioSource.UnPause();
            }
        }
    }

    void Awake()
    {
        masterBus = Resources.Load("AudioMasterBus") as AudioMixer;
        SetDuckVolume(duckVolumeRate);
    }
}
