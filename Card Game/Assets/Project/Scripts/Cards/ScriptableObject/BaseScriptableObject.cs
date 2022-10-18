#if UNITY_EDITOR
using Andrius.Core.Debuging;
using UnityEditor;
#endif
using UnityEngine;

public class BaseScriptableObject : ScriptableObject
{
    [ReadOnly] public string guiId;


    protected virtual void OnEnable()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        guiId = AssetDatabase.AssetPathToGUID(path);
#endif
    }
}
