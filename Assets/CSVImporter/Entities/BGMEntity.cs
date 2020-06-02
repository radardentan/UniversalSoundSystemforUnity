using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMEntity : ScriptableObject
{
    public Dictionary<string, Param> Params = new Dictionary<string, Param>();
    [System.Serializable]
    public class Param
    {
        public int id;
        public string songName; //曲名
        public string dictKey;  //曲を識別するファイル名
        public int BPM;
        public float loopPoint; //ループ地点(秒)
        public int[] numBeatsPerSegment;    //基準となる拍子
        public float[] sectionMarkers; //横の遷移の基準

    }    
}
