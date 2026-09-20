
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class NewGameAsset : EditorWindow
{

    public string savePath = "/Resources/AssetConfig";
    public string assetName = "GameObjectAsset_test";
    public string className = "";
    // 构造函数设置窗口名称
    NewGameAsset()
    {
        this.titleContent = new GUIContent("生成asset配置");
    }

    // 添加菜单项用于打开窗口
    [MenuItem("Tools/生成asset配置")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(NewGameAsset));
    }

    protected void OnEnable()
    {
    }

    void OnGUI()
    {
        savePath = EditorGUILayout.TextField("保存路径:", savePath);
        assetName = EditorGUILayout.TextField("保存名字:", assetName);
        className = EditorGUILayout.TextField("生成配置的类名:", className);
        if (GUILayout.Button("生成物体配置"))
        {
            Type scriptableObjectType = GetTypes(className);
            Debug.Log("生成类型:" + scriptableObjectType);
            if (scriptableObjectType != null && typeof(ScriptableObject).IsAssignableFrom(scriptableObjectType))
            {
                ScriptableObject assets = ScriptableObject.CreateInstance(scriptableObjectType) as ScriptableObject;
                string ss = Application.dataPath + savePath;
                if (!Directory.Exists(ss))
                {
                    Directory.CreateDirectory(ss);
                }
                ss = string.Format("Assets" + savePath + "/{0}.asset", assetName);
                AssetDatabase.CreateAsset(assets, ss);
            }
            else
            {
                Debug.LogError("没有这个类：" + className);
            }


        }
    }

    Type GetTypes(string className)
    {
        Type res=null;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var assemblyTypes = assembly.GetTypes();
            for (int i = 0; i < assemblyTypes.Length; i++)
            {
                if (assemblyTypes[i].Name == className)
                {
                    res = assemblyTypes[i];
                }
            }
        }
        return res;
    }

}
