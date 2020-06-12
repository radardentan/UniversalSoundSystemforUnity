using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVParser 
{
    public static IEnumerable<List<string>> ProcessingData<T>(string asset)
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
}
