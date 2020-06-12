using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetLoader
{
    public static List<T> LoadAll<T>(string directoryPath)
        where T : UnityEngine.Object
    {
        List<T> assetList = new List<T>();
        IEnumerable<string> files = Directory.EnumerateFiles("Assets/" + directoryPath, "*", SearchOption.AllDirectories);
        foreach (string filePath in files)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
            if (asset != null)
            {
                assetList.Add(asset);
            }
        }
        return assetList;
    }
}
