
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ResetRead : EditorWindow
{
    [SerializeField]
    protected List<GameObject> objlist = new List<GameObject>();
    protected SerializedObject _serializedObject;
    protected SerializedProperty _assetLstProperty;
    public bool isRead = false;
    public string path = "/res/mesh1/";

    // 构造函数设置窗口名称
    ResetRead()
    {
        this.titleContent = new GUIContent("修改文件的读写");
    }

    // 添加菜单项用于打开窗口
    [MenuItem("Tools/修改文件的读写")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(ResetRead));
    }

    protected void OnEnable()
    {
        // 使用当前类初始化
        _serializedObject = new SerializedObject(this);
        //获取当前类中可序列话的属性
        _assetLstProperty = _serializedObject.FindProperty("objlist");
    }

    void OnGUI()
    {
        //_serializedObject.Update();
        //EditorGUI.BeginChangeCheck();
        //EditorGUILayout.PropertyField(_assetLstProperty, true);
        //if (EditorGUI.EndChangeCheck())
        //{
        //    _serializedObject.ApplyModifiedProperties();
        //}
        Rect fileRect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isRead = EditorGUI.Toggle(fileRect, "是否可读写", isRead);
        //if (GUILayout.Button("修改objlist模型"))
        //{
        //    Debug.Log("修改读写：：：" + objlist.Count);
        //    for (int i = 0; i < objlist.Count; i++)
        //    {
        //        List<GameObject> aaa = LayaTools.getAllObj(objlist[i],false);
        //        aaa.Add(objlist[i]);
        //        for (int j = 0; j < aaa.Count; j++)
        //        {
        //            ModifySceneMeshesObj(aaa[i]);
        //        }
        //    }
            
        //}
        //path = EditorGUILayout.TextField("路径:", path);
        //if (GUILayout.Button("修改文件"))
        //{
        //    start();
        //}
        if (GUILayout.Button("修改场景中的所有模型读写"))
        {
            ModifySceneMeshes();
        }
    }

    void start()
    {
        string p = "Assets" + path;

        if (Directory.Exists(p))
        {
            DirectoryInfo direction = new DirectoryInfo(p);

            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".asset"))
                {
                    string n = files[i].Name.Replace(".asset", "");
                    FileDetails(n);
                }
                else if (files[i].Name.EndsWith(".mesh"))
                {
                    string n = files[i].Name.Replace(".mesh", "");
                    FileDetails(n);
                }
            }
        }
    }

    public void FileDetails(string path_n)
    {
        string p = Application.dataPath+"/" + path_n;// path + nn + ".asset";
        Debug.Log("修改文件:" + p);

        if (!File.Exists(p))
        {
            Debug.LogError("文件不存在: " + p);
            return;
        }

        try
        {
            string str = File.ReadAllText(p);
            if (isRead == true)
            {
                str = str.Replace("m_IsReadable: 0", "m_IsReadable: 1");
            }
            else
            {
                str = str.Replace("m_IsReadable: 1", "m_IsReadable: 0");
            }

            File.WriteAllText(p, str);
            Debug.Log("文件内容修改成功"+ p);
        }
        catch (Exception e)
        {
            Debug.LogError("修改文件失败: " + p + "\n错误: " + e.Message);
        }
    }

    public string ReadJson(string n)
    {
        string scrss = "Assets" + path + n;
        Debug.Log("路径:" + scrss);
        TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(scrss);
        Debug.Log("路径内容:" + text);
        return text.text;
    }

    void ModifySceneMeshes()
    {
        MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();
        foreach (MeshFilter meshFilter in meshFilters)
        {
            set_read(meshFilter);
        }
    }
    void ModifySceneMeshesObj(GameObject g)
    {
        MeshFilter meshFilter= g.GetComponent<MeshFilter>();
        Debug.Log("修改读写：：222：：" + meshFilter);
        if(meshFilter!= null)
        {
            set_read(meshFilter);
        }
    }
    void set_read(MeshFilter meshFilter)
    {
        if (meshFilter.sharedMesh != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
            if (!string.IsNullOrEmpty(assetPath))
            {
                ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
                if (importer != null && importer.isReadable!=isRead)
                {
                    importer.isReadable = isRead;
                    AssetDatabase.ImportAsset(assetPath);
                    Debug.Log("已修改模型为可读写: " + assetPath);
                }
                else
                {
                    if(assetPath.Contains(".asset") ==true)
                    {
                        assetPath = assetPath.Replace("Assets/", "");
                        FileDetails(assetPath);
                    }
                    if (assetPath.Contains(".mesh") == true)
                    {
                        assetPath = assetPath.Replace("Assets/", "");
                        FileDetails(assetPath);
                    }
                }
            }
        }
    }
}
