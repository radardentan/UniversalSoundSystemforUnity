using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using UnityEngine.Timeline;

public class Importer : AssetPostprocessor
{
    const string targetFormat = ".csv";


    public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedAssetPaths)
    {

        foreach (string asset in importedAssets)
        {
            if (Path.GetExtension(asset) != targetFormat) continue;    //指定した拡張子以外のファイルはスキップ

            string targetFile = Path.GetFileNameWithoutExtension(asset);
            string exportedFile = asset.Replace(targetFormat, ".asset");

            if(targetFile == "BGMList") BGMListFormatter.Format(asset, exportedFile);

            AssetDatabase.SaveAssets();
            Debug.Log(targetFile + "の読込/更新が完了しました。");
        }
    }   
}