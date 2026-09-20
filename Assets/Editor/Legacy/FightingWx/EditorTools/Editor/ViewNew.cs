using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class ViewNew : EditorWindow
{
    string AbPath = "/Assetbundle/View/";
    string prePath = "/Resources/View/";
    string scrPath = "/Scripts/View/";

    //利用构造函数来设置窗口名称
    ViewNew()
    {
        this.titleContent = new GUIContent("生成view代码");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/生成view代码")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(ViewNew));
    }

    protected void OnEnable()
    {

    }

    void OnGUI()
    {

        if (GUILayout.Button("生成view代码"))
        {
            start();
            AssetDatabase.Refresh();
        }
    }


    void start()
    {

        //获取指定路径下面的所有资源文件
        findView(prePath);
        findView(AbPath);
    }

    void findView(string vPath)
    {
        string path = "Assets" + vPath;
        Debug.Log("路径：：" + path);
        List<string> viewStrList = new List<string>();
        List<string> viewStrVideoList = new List<string>();
        if (Directory.Exists(path))
        {
            DirectoryInfo direction = new DirectoryInfo(path);

            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

            Debug.Log(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Name.EndsWith(".prefab"))
                {
                    continue;
                }
                string str = files[i].Name.Replace(".prefab", "");
                string scrHave = Application.dataPath + scrPath + str + ".cs";
                GameObject gg = AssetDatabase.LoadAssetAtPath<GameObject>(path + files[i].Name);
                bool ishave = File.Exists(scrHave);

                if (ishave == true)
                {
                    string scrss = "Assets" + scrPath + str + ".cs";
                    Debug.Log("路径：：：" + scrss);
                    TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(scrss);
                    xie(text.text, str, gg);
                }
                else
                {
                    this.newsrc(str, gg);
                }
                //寻找视频icon的view
                List<string> videoIconList= new List<string>();
                bool ishhh = false;
                Tools.Instance.GetAllObj_fun(gg, (bbb) =>
                {
                    Image ima = bbb.GetComponent<Image>();
                    if (ima != null && ima.sprite != null)
                    {
                        // Rewarded-video icon configuration was removed with the ad layer.
                        bool isHave = false;
                        if (isHave)
                        {
                            ishhh = true;
                            string vn=gg.name+"-"+bbb.name+"-1";
                            if (videoIconList.Contains(vn)==false)
                            {
                                videoIconList.Add(vn);
                            }
                        }
                    }
                    return false;
                });
                if (ishhh)
                {
                    viewStrList.Add(gg.name+"-1");
                    viewStrVideoList.AddRange(videoIconList);
                }
            }
        }
        Debug.LogError("包含视频标识的view："  + JsonConvert.SerializeObject(viewStrList) + "   >>>>>>>>>>>>>单个视频：" + JsonConvert.SerializeObject(viewStrVideoList));
    }

    void newsrc(string viewName, GameObject gg)
    {
        Debug.Log("修改的view：：" + viewName);
        string ss = scrStr.Replace("###", viewName);
        string showss = showStr.Replace("###", viewName);
        ss = ss.Replace("//&&&&1", showStr);
        xie(ss, viewName, gg);
    }

    void xie(string str, string viewName, GameObject gg)
    {
        str = getUiM(str, gg);
        str = getUiShow(str, viewName);
        byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);
        using (FileStream fs = File.Create(Application.dataPath + scrPath + viewName + ".cs"))
        {
            fs.Write(byteArray, 0, byteArray.Length);
        }
    }

    /// <summary>
    /// 自动生成要获取的obj
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    string getUiM(string str, GameObject g)
    {
        string scrs = str;
        string[] strlist = str.Split('é');
        if (strlist.Length == 3)
        {
            List<GameObject> glist = this.GetObjAll(g);
            string strg = "";
            string strget = "";

            for (int i = 0; i < glist.Count; i++)
            {
                if (glist[i].name.StartsWith("m_") == true)
                {
                    strg += @"
    public GameObject " + glist[i].name + " = null;";

                    strget += @"
        " + glist[i].name + @" = GetGameObjectByName(""" + glist[i].name + @""");";
                }
                else if (glist[i].name.StartsWith("t_") == true)
                {
                    strg += @"
    public Text " + glist[i].name + " = null;";

                    strget += @"
        " + glist[i].name + @" = GetComponentByName<Text>(""" + glist[i].name + @""");";
                }
                else if (glist[i].name.StartsWith("i_") == true)
                {
                    strg += @"
    public Image " + glist[i].name + " = null;";

                    strget += @"
        " + glist[i].name + @" = GetComponentByName<Image>(""" + glist[i].name + @""");";
                }
                else if (glist[i].name.StartsWith("s_") == true)
                {
                    strg += @"
    public InputField " + glist[i].name + " = null;";

                    strget += @"
        " + glist[i].name + @" = GetComponentByName<InputField>(""" + glist[i].name + @""");";
                }
                else if (glist[i].name.StartsWith("r_") == true)
                {
                    strg += @"
    public RectTransform " + glist[i].name + " = null;";

                    strget += @"
        " + glist[i].name + @" = GetComponentByName<RectTransform>(""" + glist[i].name + @""");";
                }
            }
            string strYes = "é获取m_前缀的物体" + strg + @"
    private void initGetGameObj()
    {" + @"" + strget + @"
    }
    #endregion é";
            scrs = strlist[0] + strYes + strlist[2];
        }
        return scrs;
    }

    /// <summary>
    /// 自动生成要获取的obj
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    string getUiShow(string str, string viewName)
    {

        string scrs = "";

        string[] strlist = str.Split('ě');
        string showss = showStr.Replace("###", viewName);
        scrs = strlist[0] + showss + strlist[2];
        return scrs;
    }

    /// <summary>
    /// 获取所有的obj
    /// </summary>
    /// <param name="g"></param>
    /// <returns></returns>
    List<GameObject> GetObjAll(GameObject g)
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            alllist.Add(g.transform.GetChild(i).gameObject);
            if (g.transform.GetChild(i).childCount > 0)
            {
                alllist.AddRange(GetObjAll(g.transform.GetChild(i).gameObject));
            }
        }
        return alllist;
    }

    #region 示例代码
    string scrStr = @"
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class ### : UiBase
{
    #region $自动生成的代码
    #region //&&&&1显示显示隐藏view
    #region é获取m_前缀的物体
    private void initGetGameObj()
    {
    }
    #endregion é获取m_前缀的物体
    #endregion $自动生成的代码
    ///加载调用一次
    private void load()
    {

    }
}
";
    string showStr = @"ě显示隐藏view
    public static ### self = null;
    public static string ViewName = ""###"";
    public static EventArg arg = null;
    public static void ShowView(bool isTop = true, int order = 0,EventArg arg=null)
    {
        ###.arg = arg;
        GameObject g = null;
        if (###.self != null  && ###.self.gameObject!=null && ###.self.gameObject.name!=""none"")
        {
            g = ###.self.gameObject;
            ViewManage.Instance.ShowView(g, isTop, order);
        }
        if (g == null)
        {
            ViewManage.Instance.LoadView(###.ViewName, isTop, order,(g1)=>
            {
                ### scr = g1.GetComponent<###>();
                if (scr == null)
                {
                    scr = g1.AddComponent<###>();
                }
                ###.self = scr;
            });
        }
    }
    private void Awake()
    {
        ###.self = this;
        this.initGetGameObj();
        this.load();
    }
    public static void HideView()
    {
        if (###.self != null)
        {
            ###.self.gameObject.SetActive(false);
        }
    }
    public static void DestroyView()
    {
        if (###.self != null)
        {
            ###.self.gameObject.gameObject.name=""none"";
            GameObject.Destroy(###.self.gameObject);
            ###.self = null;
        }
    }
    #endregion ě";
    #endregion
}
