using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class CSVImporter : AssetPostprocessor
{

    static void OnPostProcessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        string targetFile_1 = "Assets/BGMList.csv";
        string targetFile_2 = "Assets/SEList.csv";

        string exportedFile_1 = "Assets/ImportedItems/BGMList.asset";
        string exportedFile_2 = "Assets/ImportedItems/SEList.asset";


        foreach(string asset in importedAssets)
        {
            if (targetFile_1.Equals(asset))
            {
                BGMEntity bgmEntity = AssetDatabase.LoadAssetAtPath<BGMEntity>(exportedFile_1);

                if (bgmEntity = null)
                {
                    bgmEntity = ScriptableObject.CreateInstance<BGMEntity>();
                    AssetDatabase.CreateAsset(bgmEntity, exportedFile_1);
                }

                bgmEntity.Params.Clear();

                using (StreamReader streamReader = new StreamReader(targetFile_1))
                {
                    streamReader.ReadLine();    //ヘッダを読み飛ばす
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        string[] dataStrs = line.Split(',');

                        BGMEntity.Param param = new BGMEntity.Param
                        {
                            id = int.Parse(dataStrs[0]),
                            songName = dataStrs[1],
                            dictKey = dataStrs[2],
                            BPM = int.Parse(dataStrs[3]),
                            loopPoint = float.Parse(dataStrs[4])
                        };

                        bgmEntity.Params.Add(param.dictKey, param);

                        
                    }
                }

            }
            else if (targetFile_2.Equals(asset))
            {
                SEEntity seEntity = AssetDatabase.LoadAssetAtPath<SEEntity>(exportedFile_2);
                if (seEntity = null)
                {
                    seEntity = ScriptableObject.CreateInstance<SEEntity>();
                    AssetDatabase.CreateAsset((ScriptableObject)seEntity, exportedFile_1);
                }
                seEntity.Params.Clear();

                using (StreamReader streamReader = new StreamReader(targetFile_2))
                {
                    streamReader.ReadLine();    //ヘッダを読み飛ばす
                    while (!streamReader.EndOfStream)
                    {
                        string line = streamReader.ReadLine();
                        string[] dataStrs = line.Split(',');

                        SEEntity.Param param = new SEEntity.Param
                        {
                            id = int.Parse(dataStrs[0]),
                            categoryName = dataStrs[1],
                            dictKey = dataStrs[2],
                            isRoundRobin = bool.Parse(dataStrs[3])                       
                        };
                
                        seEntity.Params.Add(param.dictKey,param);
                    
                        
                    }
                }
            }
            else continue;

            AssetDatabase.SaveAssets();

            Debug.Log("BGMリスト/SEリストの読込・更新が完了しました。");
        }

    }
}
