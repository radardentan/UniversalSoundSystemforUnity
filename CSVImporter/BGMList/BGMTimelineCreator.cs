using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class BGMTimelineCreator : MonoBehaviour
{
    static T Initialize<T>(string exportedFile)
    where T : TimelineAsset
    {
        T type = AssetDatabase.LoadAssetAtPath<T>(exportedFile);
        if (type == null)
        {
            type = TimelineAsset.CreateInstance<T>();
            AssetDatabase.CreateAsset(type, exportedFile);
        }
        return type;
    }
    public static void Create(BGMList.Param param)
    {
        //TimelineAssetの生成
        TimelineAsset timeline = Initialize<TimelineAsset>(param.dictKey);
        AudioTrack[] audioTracks = timeline.GetRootTracks() as AudioTrack[];
        List<AudioClip> audioClips = AssetLoader.LoadAll<AudioClip>("BGM/" + param.dictKey);    //ここでファイル名でソートさせる必要がありそう
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
        foreach (TimelineClip clip in timelineClips)
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
        if (param.sectionMarkers.Count > 0)
        {
            foreach (double sectionmarker in param.sectionMarkers)
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


            foreach (TimelineClip clip in timelineClips)
            {
                clip.clipIn = 0;
                clip.duration = param.subTrackTimeMarkers[i - 1].endTime - param.subTrackTimeMarkers[i - 1].startTime;
                clip.start = param.subTrackTimeMarkers[i - 1].startTime;
            }
        }
    }
}
