using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BGMListFormatter
{
    public static void Format(string asset, string exportedFile) 
    {
        BGMList bgmList = ScriptableObjectLoader.Initialize<BGMList>(exportedFile);
        bgmList.Params.Clear();
        foreach (List<string> data in CSVParser.ProcessingData<BGMList>(asset))
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
            BGMList.Param checkedParam = ScriptableObjectLoader.Initialize<BGMList.Param>(param.dictKey);  //ファイル名がdictkeyのScriptableObjectの読込/生成
            bool isDataChanged = param.CompareParam(checkedParam);
            if (!isDataChanged)
            {
                checkedParam = param;
                BGMTimelineCreator.Create(param);
            }
        }
    }
}
