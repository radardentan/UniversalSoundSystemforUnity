using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEngine.Timeline;

public class Importer : AssetPostprocessor
{
    readonly string targetFormat = ".csv";

    //アセットがなければ作成
    static T Initialize<T>(string exportedFile)
        where T : ScriptableObject
    {
        T type = AssetDatabase.LoadAssetAtPath<T>(exportedFile);
        if (type == null)
        {
            type = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(type, exportedFile);
        }
        return type;
    }

    //データのScriptableObjectへの格納
    static IEnumerable<List<string>> ProcessingData<T>(string asset)
        where T : ScriptableObject
    {

        using (StreamReader stream = new StreamReader(asset))
        {
            stream.ReadLine();  //ヘッダを読み飛ばす

            while (!stream.EndOfStream)
            {
                string line = stream.ReadLine();
                string[] data = line.Split(',');

                //","への対応
                List<string> dataList = new List<string>(data);
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (dataList[i].Length > 0 && dataList[i].TrimStart()[0] == '"')
                    {
                        dataList[i].TrimStart();

                        if (dataList[i].TrimEnd()[dataList[i].Length - 1] == '"')
                        {
                            dataList[i].TrimEnd();
                            dataList[i].Remove(0, 1);
                            dataList[i].Remove(dataList[i].Length - 1, 1);
                            continue;
                        }

                        while (true)
                        {
                            dataList[i] += "," + dataList[i + 1];
                            dataList.RemoveAt(i + 1);

                            if (dataList[i].TrimEnd()[dataList[i].Length - 1] == '"')
                            {
                                dataList[i].TrimEnd();
                                dataList[i].Remove(0, 1);
                                dataList[i].Remove(dataList[i].Length - 1, 1);
                                break;
                            }
                        }
                    }
                }
                yield return dataList;
            }
        }
    }

    static List<T> LoadAll<T>(string directoryPath) 
        where T : UnityEngine.Object
    {
        List<T> assetList = new List<T>();
        IEnumerable<string> files = Directory.EnumerateFiles("Assets/" + directoryPath, "*", SearchOption.AllDirectories);
        foreach (string filePath in files)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (asset != null)
            {
                assetList.Add(asset);
            }
        }
        return assetList;
    }
    

    public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedAssetPaths)
    {

        foreach (string asset in importedAssets)
        {
            if (Path.GetExtension(asset) != targetFormat) continue;    //指定した拡張子以外のファイルはスキップ

            string targetFile = Path.GetFileNameWithoutExtension(asset);
            string exportedFile = asset.Replace(targetFormat, ".asset");

            if(targetFile == "BGMList") 
            {
                BGMList bgmList = Initialize<BGMList>(exportedFile);
                bgmList.Params.Clear();
                foreach(List<string> data in ProcessingData<BGMList>(asset)) 
                {
                    BGMList.Param param = new BGMList.Param()
                    {
                        dictKey = data[0],
                        songTitle = data[1],
                        Tags = data[2].Split(',').ToList(),
                        BPM = int.Parse(data[3]),              
                    };
                    if (data.Count < 4) continue;
                    param.numBeatsPerSegments = (int.Parse(data[4].Split('/')[0]), int.Parse(data[4].Split('/')[1]));
                    if (data.Count < 5) continue;
                    param.loopTimeMarkers = (int.Parse(data[5].Split(',')[0]), int.Parse(data[5].Split(',')[1]));
                    if (data.Count < 6) continue;
                    param.sectionMarkers = data[6].Split(',').ToList().ConvertAll(a => double.Parse(a));
                    if (data.Count < 7) continue;
                    param.subTrackTimeMarkers = data.GetRange(7, data.Count - 6).Select(x => (double.Parse(x.Split(',')[0]), double.Parse(x.Split(',')[1]))).ToList();

                    bgmList.Params.Add(param);

                    //ここに比較処理を挟みたい
                    BGMList.Param checkedParam = Initialize<BGMList.Param>(param.dictKey);  //ファイル名がdictkeyのScriptableObjectの読込/生成
                    bool isDataChanged = param.CompareParam(checkedParam);
                    if (!isDataChanged) 
                    {
                        checkedParam = param;
                        BGMTimelineCreater(param);                        
                    }
                }
            }

            AssetDatabase.SaveAssets();
            Debug.Log(targetFile + "の読込/更新が完了しました。");
        }
    }

    static void BGMTimelineCreater(BGMList.Param param) 
    {
        //TimelineAssetの生成
        TimelineAsset timeline = Initialize<TimelineAsset>(param.dictKey);
        AudioTrack[] audioTracks = timeline.GetRootTracks() as AudioTrack[];
        List<AudioClip> audioClips = LoadAll<AudioClip>("BGM/" + param.dictKey);    //ここでファイル名でソートさせる必要がありそう
        foreach (AudioTrack audioTrack in audioTracks)
        {
            timeline.DeleteTrack(audioTrack);
        }
        //AudioTrackの作成
        for (int i = 0; i <= param.subTrackTimeMarkers.Count; i++)
        {
            timeline.CreateTrack<AudioTrack>();
        }
        audioTracks = timeline.GetRootTracks() as AudioTrack[];
        //AudioClipの割り当て
        //メイントラック
        audioTracks[0].CreateClip(audioClips[0]);
        IEnumerable<TimelineClip> timelineClips = audioTracks[0].GetClips();
        foreach(TimelineClip clip in timelineClips) 
        {
            clip.clipIn = 0;
            clip.duration = audioClips[1].length;
            clip.start = 0;
        }
        timeline.CreateMarkerTrack();
        //ループマーカーの追加
        timeline.markerTrack.CreateMarker(typeof(LoopStartMarker), param.loopTimeMarkers.loopStartTime);
        timeline.markerTrack.CreateMarker(typeof(LoopEndMarker), param.loopTimeMarkers.loopEndTime);
        //セクションマーカーの追加
        if(param.sectionMarkers.Count > 0) 
        {
            foreach(double sectionmarker in param.sectionMarkers) 
            {
                timeline.markerTrack.CreateMarker(typeof(SectionMarker), sectionmarker);
            }
        }
        if (param.subTrackTimeMarkers.Count <= 0) return;
        //サブトラックの割り当て
        for (int i = 1; i <= param.subTrackTimeMarkers.Count; i++)
        {
            audioTracks[i].CreateClip(audioClips[i]);
            timelineClips = audioTracks[i].GetClips();


            foreach(TimelineClip clip in timelineClips) 
            {
                clip.clipIn = 0;
                clip.duration = param.subTrackTimeMarkers[i - 1].endTime - param.subTrackTimeMarkers[i - 1].startTime;
                clip.start = param.subTrackTimeMarkers[i - 1].startTime;
            }
        }
    }
}