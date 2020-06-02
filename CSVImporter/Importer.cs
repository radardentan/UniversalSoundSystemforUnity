using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;

public class Importer : AssetPostprocessor
{
    string targetFormat = ".csv";



    public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedAssetPaths)
    {

        foreach (string asset in importedAssets)
        {
            if (Path.GetExtension(asset) != targetFormat) continue;    //指定した拡張子以外のファイルはスキップ

            string targetFile = Path.GetFileNameWithoutExtension(asset);
            string exportedFile = asset.Replace(targetFormat, ".asset");

            //アセットがなければ作成
            T Initialize<T>()
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

            void ProcessingData<T>(Action action)
                where T : ScriptableObject
            {

                using (StreamReader stream = new StreamReader(asset))
                {
                    stream.ReadLine();  //ヘッダを読み飛ばす

                    while (!stream.EndOfStream)
                    {
                        string line = stream.ReadLine();
                        string[] data = line.Split(',');
                        action();
                    }
                }
            }

            if(targetFile == "BGMList") 
            {
                BGMList bgmList = Initialize<BGMList>();
                bgmList.Params.Clear();
                ProcessingData<BGMList>(() =>
                {
                    BGMList.Param param = new BGMList.Param()
                    {

                    };
                    bgmList.Params.Add(param);
                });
            }


            /*将来的にc#8.0で以下の様に書けるようになる
             
            Action action = targetFile switch
            {
                "BGMList" => (() =>
                {
                    BGMList bgmList = Initialize<BGMList>();
                    bgmList.Params.Clear();
                    ProcessingData<BGMList>(() =>
                    {
                        BGMList.Param param = new BGMList.Param()
                        {

                        };
                        bgmList.Params.Add(param);
                    });
                }),
                "SEList" => (() =>
                {

                }),
                "BGSList" => (() =>
                {

                }),
                _ => (() => { }) //破棄パターンでは何もしない
            };
            */

            AssetDatabase.SaveAssets();
            Debug.Log(targetFile + "の読込/更新が完了しました。");
        }
    }
}