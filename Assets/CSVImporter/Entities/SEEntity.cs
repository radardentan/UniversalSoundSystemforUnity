using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEEntity : ScriptableObject
{
    public Dictionary<string, Param> Params = new Dictionary<string, Param>();
    [System.Serializable]
    public class Param
    {
        public int id;
        public string categoryName; //曲名
        public string dictKey;  //曲を識別するファイル名
        public bool isRoundRobin;   //ラウンドロビンするかどうか

    }
}
