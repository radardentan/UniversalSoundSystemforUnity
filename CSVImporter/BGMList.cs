using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;

public class BGMList : ScriptableObject
{
    public List<Param> Params;

    public class Param : ScriptableObject
    {
        public string dictKey;
        public string songTitle;
        public List<string> Tags;

        public int BPM;
        public (int beats, int segments) numBeatsPerSegments;

        public (float loopStartTime, float loopEndTime) loopTimeMarkers;
        public List<float> sectionMarkers;
        public List<(float startTime, float endTime)> subTrackTimeMarkers;

        public bool CompareParam(Param other) 
        {
            if (!dictKey.Equals(other.dictKey)) return false;
            if (!songTitle.Equals(other.songTitle)) return false;
            for(int i = 0; i < Tags.Count; i++) 
            {
                if (!Tags[i].Equals(other.Tags[i])) return false;
            }
            if (!BPM.Equals(other.BPM)) return false;
            if (!numBeatsPerSegments.Equals(other.numBeatsPerSegments)) return false;
            for(int i = 0; i < sectionMarkers.Count; i++) 
            {
                if (!sectionMarkers[i].Equals(other.sectionMarkers[i])) return false;
            }
            for(int i=0; i < subTrackTimeMarkers.Count; i++) 
            {
                if (subTrackTimeMarkers[i].Equals(other.subTrackTimeMarkers)) return false;
            }
            return true;
        }
    }
}

