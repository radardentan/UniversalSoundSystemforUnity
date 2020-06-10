using System.Collections;
using System.Collections.Generic;
using System;
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

        public (double loopStartTime, double loopEndTime) loopTimeMarkers;
        public List<double> sectionMarkers;
        public List<(double startTime, double endTime)> subTrackTimeMarkers;

        public bool CompareParam(Param other) 
        {
            if (!dictKey.Equals(other.dictKey)) return false;
            if (!songTitle.Equals(other.songTitle)) return false;
            if (!Tags.Count.Equals(other.Tags.Count)) return false;
            if (!sectionMarkers.Count.Equals(other.sectionMarkers.Count)) return false;
            if (!subTrackTimeMarkers.Count.Equals(other.subTrackTimeMarkers.Count)) return false;
            for (int i = 0; i < Tags.Count; i++)
            {
                if (!Tags[i].Equals(other.Tags[i])) return false;
            }
            for (int i = 0; i < sectionMarkers.Count; i++)
            {
                if (!sectionMarkers[i].Equals(other.sectionMarkers[i])) return false;
            }
            for (int i = 0; i < subTrackTimeMarkers.Count; i++)
            {
                if (!subTrackTimeMarkers[i].Equals(other.subTrackTimeMarkers[i])) return false;
            }
            if (!BPM.Equals(other.BPM)) return false;
            if (!numBeatsPerSegments.Equals(other.numBeatsPerSegments)) return false;
            return true;
        }

        public Param() 
        {
            dictKey = null;
            songTitle = null;
            Tags = null;
            BPM = 120;
            numBeatsPerSegments = (4, 4);
            loopTimeMarkers = (-1d, -1d);
            sectionMarkers = null;
            subTrackTimeMarkers = null;
        }
    }
}

