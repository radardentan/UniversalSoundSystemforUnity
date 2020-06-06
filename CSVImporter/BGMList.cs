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
        public List<float> SectionMarkers;
        public List<(float startTime, float endTime)> subTrackTimeMarkers;
        
    }
}

