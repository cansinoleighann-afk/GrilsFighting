using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NewJsonClass : EditorWindow
{
    // Start is called before the first frame update
    [SerializeField]//必须要加
    protected List<GameObject> objlist = new List<GameObject>();
    //public GameObject objlist;
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;

    public string path = "/Scripts/Json/";
    //利用构造函数来设置窗口名称
    NewJsonClass()
    {
        this.titleContent = new GUIContent("生成Excel配置");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/生成Excel配置")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(NewJsonClass));
    }

    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
    }

    void OnGUI()
    {
        if (GUILayout.Button("生成Excel配置"))
        {
            start();
            AssetDatabase.Refresh();
        }
    }

    void start()
    {
        string p = "Assets" + path;
        Debug.Log("路径:::::::"+ Directory.Exists(p));
        if (Directory.Exists(p))
        {
            DirectoryInfo direction = new DirectoryInfo(p);

            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);


            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".json"))
                {
                    string n = files[i].Name.Replace(".json", "");
                    string content = ReadJson(files[i].Name);
                    string scr = setJson(content,n);
                    xie(scr, n);
                }
            }
        }
    }

    void xie(string str, string viewName)
    {
        byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);
        using (FileStream fs = File.Create(Application.dataPath + path + viewName + ".cs"))
        {
            fs.Write(byteArray, 0, byteArray.Length);
        }
    }
    string setJson(string c,string classN)
    {
        string str = "";

        string part1 = "";
        string part2 = "";
        string part3 = "public string _argData;" + @"
    ";
        string part4 = "";

        string[] key= c.Split('{');
        string infoClass = key[2].Split('}')[0];
        string[] keylist= infoClass.Split(',');
        for (int i = 0; i < keylist.Length; i++)
        {
            string[] v= keylist[i].Split(':');
            string keyC = v[0].Replace("\"","").Replace("\n", "").Trim();
            
            string value1 = "";
            string value2 = "";
            string value3 = "";
            //Debug.Log("字符::::" + keyC + "   v:" + v[0]+ v[1]);
            if (v[1].IndexOf('"') <0)
            {
                value1 = "float " + "_" + keyC;
                value3 = "public float "+ keyC+ @";
    ";
            }
            else
            {
                value1 = "string " + "_" + keyC;
                value3 = "public string " + keyC + @";
    ";
            }
            value2 = keyC + "=_" + keyC;
            if (i != keylist.Length - 1)
            {
                part1 += value1 + ",";
            }
            else
            {
                part1 += value1;
            }
            part2 += value2 + ";";
            part3 += value3;
        }
        string[] vv = c.Split(':');

        int off = keylist.Length;

        for (int i = 0; i <= vv.Length-off-1; i=i+ off+1)
        {
            string[] key4Index = vv[i].Split('"');
            string key4 = key4Index[key4Index.Length-2];
            string vv4 = "";
            //Debug.Log("key:::::" + key4+" :"+i+"  off:"+off+ "  :"+ vv[i]);
            for (int j = i+2; j <= i+off+1; j++)
            {
                string v4 = "";
                if (j != i + off + 1)
                {

                    v4 = vv[j].Split(',')[0].Replace("\n", "").Trim();
                    if (v4.IndexOf('"') < 0)
                    {
                        v4 += "f";
                    }
                    vv4 += v4 + ",";
                }else
                {
                    v4 = vv[j].Split('}')[0].Replace("\n", "").Trim();
                    if (v4.IndexOf('"') < 0)
                    {
                        v4 += "f";
                    }
                    vv4 += v4 ;
                }
            }

            part4+= "info.Add(\""+key4+"\""+ ", new "+ classN + "Info("+ vv4 + @"));
        ";
        }

        str = scrStr.Replace("###", classN);
        str = str.Replace("&&&1", part1);
        str = str.Replace("&&&2", part2);
        str = str.Replace("&&&3", part3);
        str = str.Replace("&&&4", part4);
        return str;
    }

    string scrStr = @"
using System.Collections.Generic;
using System;
[Serializable]
public class ###
{
    #region
    private static ### _self;
    public static ### self { get { if (_self == null) { _self = new ###(); } return _self; } }
    public static void setValue(Dictionary<string, ###Info> v) { _self.info = v; }
    public Dictionary<string, ###Info> info = new Dictionary<string, ###Info>();
    #endregion
    public ###()
    {
        &&&4
    }

}
[Serializable]
public class ###Info
{
    public ###Info(&&&1)
    {
        &&&2
    }
    &&&3
}
";

    public string ReadJson(string n)
    {
        string scrss = "Assets" + path + n;
        TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(scrss);
        return text.text;

    }



    public List<string> LoadAll(string path)
    {
        List<string> json = new List<string>();
        Debug.Log("加载游戏json：文件夹："+path);

       TextAsset[] text =Resources.LoadAll<TextAsset>(path);
        for (int i = 0; i < text.Length; i++)
        {
            Debug.Log("加载游戏json：" + text[i].name);
            json.Add(text[i].text);
        }
        
        return json;
    }
}