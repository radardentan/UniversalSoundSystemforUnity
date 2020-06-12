using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectLoader
{
    public static T Initialize<T>(string exportedFile)
        where T : ScriptableObject
    {
        T type = AssetDatabase.LoadAssetAtPath<T>(exportedFile) ;
        if (type == null)
        {
            type = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(type, exportedFile);
        }
        return type;
    }
}
