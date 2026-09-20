using AssetBundles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class SetAB : EditorWindow
{
    // Start is called before the first frame update
    [SerializeField]//必须要加
    protected List<GameObject> objlist = new List<GameObject>();
    //public GameObject objlist;
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;

    public bool isAll = false;
    public bool isAllWeb = true;
    public bool isUseTxt = false;
    public string abpath = "Assets/Assetbundle/Auto";
    public string abQian = "";

    public string path = "/assetbundle/";
    //利用构造函数来设置窗口名称
    SetAB()
    {
        this.titleContent = new GUIContent("生成AB");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/生成AB")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(SetAB));
    }

    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
    }

    void OnGUI()
    {

        if (mouseOverWindow == this)
        {//鼠标位于当前窗口
            if (Event.current.type == EventType.DragUpdated)
            {//拖入窗口未松开鼠标
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;//改变鼠标外观
            }
            else if (Event.current.type == EventType.DragExited)
            {//拖入窗口并松开鼠标
                Focus();//获取焦点，使unity置顶(在其他窗口的前面)
                        //Rect rect=EditorGUILayout.GetControlRect();
                        //rect.Contains(Event.current.mousePosition);//可以使用鼠标位置判断进入指定区域
                if (DragAndDrop.paths != null)
                {
                    int len = DragAndDrop.paths.Length;
                    for (int i = 0; i < len; i++)
                    {
                        //Debug.Log("路径:" + DragAndDrop.paths[i]);//输出拖入的文件或文件夹路径
                        abpath = DragAndDrop.paths[i];
                    }
                }
            }
        }

        abpath = EditorGUILayout.TextField(abpath);
        if (GUILayout.Button("修改路径中ab包名字"))
        {
            setAbPath();
            AssetDatabase.Refresh();
        }
        Rect fileRect4 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isUseTxt = EditorGUI.Toggle(fileRect4, "是否使用abPack里的包名", isUseTxt);
        abQian = EditorGUILayout.TextField(abQian);
        if (GUILayout.Button("记录当前ab包txt"))
        {
            setAbTxt();
            AssetDatabase.Refresh();
        }

        //path = EditorGUILayout.TextField(path);
        if (GUILayout.Button("生成AB"))
        {
            start();
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("结构版本"))
        {
            buildV();
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("生产ab的代码"))
        {
            webglV = "WebGL" + UnityEngine.PlayerPrefs.GetString("abVVV");
            Dictionary<string, string> aaa = setAbName(webglV);
            TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets" + scrPath);
            xie(text.text, aaa);
            AssetDatabase.Refresh();
            xieAbList();
            clearAb();
        }
        Rect fileRect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isAll = EditorGUI.Toggle(fileRect, "是否打包pc", isAll);
        Rect fileRect1 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isAllWeb = EditorGUI.Toggle(fileRect1, "是否打包web", isAllWeb);
    }

    void start()
    {
        BuildAssetBundles();
        Debug.Log("打包完成>>>>>>>>>>");
    }
    public void BuildAssetBundles()
    {
        //pcBuild();
        //webBuild();
        if (isAll == true)
        {
            pcBuild();
            //iosBuild();
            //androidBuild();

        }
        if (isAllWeb == true)
        {
            webBuild();
        }
    }
    public void webBuild()
    {
        string outputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformForAssetBundles(BuildTarget.WebGL));
        outputPath = Path.Combine("Assets/StreamingAssets", outputPath);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.AppendHashToAssetBundleName, BuildTarget.WebGL);
    }

    public void iosBuild()
    {
        string outputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformForAssetBundles(BuildTarget.iOS));
        outputPath = Path.Combine("Assets/StreamingAssets", outputPath);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.AppendHashToAssetBundleName, BuildTarget.iOS);
    }

    public void androidBuild()
    {
        string outputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformForAssetBundles(BuildTarget.Android));
        outputPath = Path.Combine("Assets/StreamingAssets", outputPath);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.AppendHashToAssetBundleName, BuildTarget.Android);
    }

    public void pcBuild()
    {
        string outputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformForAssetBundles(BuildTarget.StandaloneWindows64));
        outputPath = Path.Combine("Assets/StreamingAssets", outputPath);
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.AppendHashToAssetBundleName, BuildTarget.StandaloneWindows64);
    }

    void buildV()
    {
        string aa1 = UnityEngine.Random.Range(0, 9) + "";
        string aa2 = UnityEngine.Random.Range(0, 9) + "";
        string aa3 = UnityEngine.Random.Range(0, 9) + "";
        string aa4 = UnityEngine.Random.Range(0, 9) + "";
        string aa5 = UnityEngine.Random.Range(0, 9) + "";
        string aa6 = UnityEngine.Random.Range(0, 9) + "";
        string v = aa1 + aa2 + aa3 + aa4 + aa5 + aa6;
        UnityEngine.PlayerPrefs.SetString("abVVV", v);

        string woutputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformForAssetBundles(BuildTarget.StandaloneWindows64));
        woutputPath = Path.Combine("Assets/StreamingAssets", woutputPath);
        string w = "Windows" + v;
        //AssetDatabase.RenameAsset(woutputPath + "/Windows", w);


        webglV = "WebGL" + v;

        string pcoutputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformForAssetBundles(BuildTarget.WebGL));
        pcoutputPath = Path.Combine("Assets/StreamingAssets", pcoutputPath);
        AssetDatabase.RenameAsset(pcoutputPath + "/WebGL", webglV);
        Debug.Log("版本::::" + webglV);
        if (isAll == true)
        {
            //AssetDatabase.RenameAsset(outputPath + "/Android", webglV);
            //AssetDatabase.RenameAsset(outputPath + "/iOS", webglV);
        }

    }

    void xieAbList()
    {
        string str = allAbList.Count + "";
        foreach (string key in allAbList.Keys)
        {
            str += @"
" + allAbList[key];
        }
        //        for (int i = 0; i < allAbList.Count; i++)
        //        {
        //            str += @"
        //"+ allAbList[i];
        //        }
        byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);
        using (FileStream fs = File.Create(Application.dataPath + "/Assetbundle/CustomSearch.txt"))
        {
            fs.Write(byteArray, 0, byteArray.Length);
        }
    }

    Dictionary<string, string> setAbName(string webN)
    {
        Dictionary<string, string> allAb = new Dictionary<string, string>();
        clearAb();
        string path = Application.dataPath + "/StreamingAssets/AssetBundles/WebGL/" + webN;
        Debug.Log("路径:::" + path);

        AssetBundle ab = AssetBundle.LoadFromFile(path);
        AssetBundleManifest abm = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        var hashNames = abm.GetAllAssetBundles();
        Dictionary<string, string> ABNamesDict = new Dictionary<string, string>();
        foreach (var hashName in hashNames)
        {
            var abName = Regex.Match(hashName, "_[0-9a-f]{32}$").Success ? hashName.Substring(0, hashName.Length - 33) : hashName;
            //Debug.Log("名字:::" + abName + "  :" + hashName);
            ABNamesDict.Add(abName, hashName);
            Dictionary<string, string> all = getAllAb(hashName, abName);
            foreach (string key in all.Keys)
            {
                allAb[key] = all[key];
            }
        }
        return allAb;
        //clearAb();
    }

    Dictionary<string, string> allAbList = new Dictionary<string, string>();
    Dictionary<string, string> getAllAb(string n, string pack)
    {
        Dictionary<string, string> allAb = new Dictionary<string, string>();
        string r = Application.dataPath + "/StreamingAssets/AssetBundles/WebGL/";
        using (FileStream fsRead = new FileStream(r + pack + ".manifest", FileMode.Open))
        {
            int fsLen = (int)fsRead.Length;
            byte[] heByte = new byte[fsLen];
            int rr = fsRead.Read(heByte, 0, heByte.Length);
            string myStr = System.Text.Encoding.UTF8.GetString(heByte);

            myStr = myStr.Replace("Assets:", "ē");
            myStr = myStr.Replace("Dependencies:", "ē");
            myStr = myStr.Split('ē')[1];
            string[] abList = myStr.Split('-');
            for (int i = 0; i < abList.Length; i++)
            {
                string abstr = abList[i].Trim();
                if (abstr != "")
                {

                    string[] nn = abstr.Split('/');
                    string str = nn[nn.Length - 1].Split('.')[0];
                    allAb[str + "_" + pack] = str + ";" + pack;
                    allAbList.Add(str + ";" + pack, abstr);
                    Debug.Log("字符:::::" + allAb[str + "_" + pack]);
                }

            }
        }
        return allAb;
    }

    void clearAb()
    {
        IEnumerable<AssetBundle> abList = AssetBundle.GetAllLoadedAssetBundles();
        foreach (AssetBundle item in abList)
        {
            item.Unload(true);
        }
    }

    string scrPath = "/Scripts/Tools/AssetBundleConfig.cs";
    void xie(string str, Dictionary<string, string> abList)
    {
        str = setStr(str, abList);
        byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);
        using (FileStream fs = File.Create(Application.dataPath + scrPath))
        {
            fs.Write(byteArray, 0, byteArray.Length);
        }
    }

    string webglV = "";
    string setStr(string con, Dictionary<string, string> abList)
    {
        string abStr = "";
        string abDic = "";
        string abAtlasDic = "";
        string abPcDic = "";
        bool dicOne = false;
        bool dicAAOne = false;
        foreach (string key in abList.Keys)
        {
            string ss = @"
    public string " + key + @" = """ + abList[key] + @""";";
            abStr += ss;
            string ssDic = "";
            string ssAtlasDic = "";
            if (abList[key].Contains("atlas") == false)
            {
                if (dicOne == false)
                {
                    ssDic = @"{ """ + abList[key].Split(';')[0] + @""",""" + abList[key] + @"""}";
                    dicOne = true;
                }
                else
                {
                    ssDic = @",{ """ + abList[key].Split(';')[0] + @""",""" + abList[key] + @"""}";
                }
            }
            else
            {
                if (dicAAOne == false)
                {
                    ssAtlasDic = @"{ """ + abList[key].Split(';')[0] + @""",""" + abList[key] + @"""}";
                    dicAAOne = true;
                }
                else
                {
                    ssAtlasDic = @",{ """ + abList[key].Split(';')[0] + @""",""" + abList[key] + @"""}";
                }
            }

            abDic += ssDic;
            abAtlasDic += ssAtlasDic;
        }

        foreach (string key in allAbList.Keys)
        {
            string sss = @"{ """ + key + @""",""" + allAbList[key] + @"""}";
            if (abPcDic == "")
            {
                abPcDic = sss;
            }
            else
            {
                abPcDic += "," + sss;
            }
        }
        string[] strlist = con.Split('é');
        string str = strlist[1];
        str = @"é
    public string webglV = """ + webglV + @""";
    ###AB
    public Dictionary<string,string> abList = new Dictionary<string, string> { 
        ###List
    };
    //图集
    public Dictionary<string,string> abAtlasList = new Dictionary<string, string> { 
        ###Atlas
    };
    public Dictionary<string,string> abPcList = new Dictionary<string, string> { 
        ###PcList
    };
    #endregion é";
        str = str.Replace("###AB", abStr);
        str = str.Replace("###List", abDic);
        str = str.Replace("###Atlas", abAtlasDic);
        str = str.Replace("###PcList", abPcDic);
        string yes = strlist[0] + str + strlist[2];
        return yes;
    }


    /// <summary>
    /// 具体设置单个资源的包名
    /// </summary>
    /// <param name="file">文件对象</param>
    /// <param name="bagname">包名</param>
    /// <param name="assetsfilepath">用于检索文件的路径变量</param>
    private static void DealSingleABName(FileInfo file, string newbagname, StringBuilder assetsfilepath)
    {
        assetsfilepath.Clear();
        assetsfilepath.Append(file.FullName.Substring(file.FullName.IndexOf("Assets")));
        //找到文件
        AssetImporter res = AssetImporter.GetAtPath(assetsfilepath.ToString());
        //设置包名
        if (res != null)
        {
            res.assetBundleName = newbagname;
        }
        else
        {
            Debug.LogError("设置包名异常：" + assetsfilepath.ToString());
        }
    }

    void setAbPath()
    {
        Dictionary<string, string> setHave = getAbTxt();
        DirectoryInfo direction = new DirectoryInfo(abpath);
        FileInfo[] files = direction.GetFiles();
        //Debug.Log("修改ab名字:::" + files.Length);
        if (abpath.Contains("AtlasAb") == true)
        {
            abQian = "atlas";
        }
        for (int i = 0; i < files.Length; ++i)
        {
            if (!files[i].Name.Contains(".meta"))
            {
                string assetpath = abpath + "/" + files[i].Name;
                string nn = files[i].Name.ToLower().Substring(0, files[i].Name.IndexOf('.'));
                AssetImporter importer = AssetImporter.GetAtPath(assetpath);

                bool isHaveS = setHave.ContainsKey(nn);
                string vv = "";
                if (isHaveS == true)
                {
                    vv = setHave[nn];
                }


                nn = nn.Replace("_", "");
                string abn = abQian + nn;
                bool isHaveGai = false;
                if (isUseTxt == true)
                {
                    if (isHaveS == true && vv != "")
                    {
                        abn = vv;
                        isHaveGai = true;
                    }
                }
                //Debug.Log("修改ab名字:222::" + name+"  :"+ importer);
                if (importer != null)
                {
                    if (abQian != "no")
                    {
                        importer.assetBundleName = abn;
                    }
                    else
                    {
                        if (isHaveGai == true)
                        {
                            importer.assetBundleName = abn;
                        }
                        else
                        {
                            importer.assetBundleName = null;
                        }

                    }

                }
            }
        }

        //AssetImporter assetImporter = AssetImporter.GetAtPath("Assets/Resources/HairPrefab");
    }

    void setAbTxt()
    {
        DirectoryInfo direction = new DirectoryInfo(abpath);
        FileInfo[] files = direction.GetFiles();
        Dictionary<string, string> dictxt = new Dictionary<string, string>();
        for (int i = 0; i < files.Length; ++i)
        {
            if (!files[i].Name.Contains(".meta"))
            {
                string assetpath = abpath + "/" + files[i].Name;
                string n = files[i].Name.ToLower().Substring(0, files[i].Name.IndexOf('.'));
                AssetImporter importer = AssetImporter.GetAtPath(assetpath);
                if (importer != null && importer.assetBundleName != null && importer.assetBundleName != "")
                {

                    if (dictxt.ContainsKey(n) == false)
                    {
                        dictxt.Add(n, importer.assetBundleName);
                    }
                }
            }
        }

        string str = "";
        foreach (string key in dictxt.Keys)
        {
            if (str == "")
            {
                str = key + "," + dictxt[key];
            }
            else
            {
                str += ";" + @"
" + key + "," + dictxt[key];
            }
        }
        if (str != "")
        {
            byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);
            string pp = "/" + abpath.Replace("Assets/", "");
            using (FileStream fs = File.Create(Application.dataPath + pp + "/abPack.txt"))
            {
                fs.Write(byteArray, 0, byteArray.Length);
            }
        }

    }

    Dictionary<string, string> getAbTxt()
    {
        string scrHave = abpath + "/abPack.txt";
        bool ishave = File.Exists(scrHave);

        Dictionary<string, string> dic = new Dictionary<string, string>();
        if (ishave == true)
        {
            TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(scrHave);
            string[] sss = text.text.Split(';');
            //Debug.Log("是否有>>>>>>>>>" + text.text);
            for (int i = 0; i < sss.Length; i++)
            {
                string[] vvv = sss[i].Split(',');

                if (vvv.Length == 2)
                {
                    vvv[0] = vvv[0].Replace(" ", "");
                    vvv[0] = Regex.Replace(vvv[0], @"[/n/r]", "");
                    vvv[0] = vvv[0].Replace(@"
", "");
                    vvv[1] = vvv[1].Replace(" ", "");
                    vvv[1] = Regex.Replace(vvv[1], @"[/n/r]", "");
                    vvv[1] = vvv[1].Replace(@"
", "");
                    //Debug.Log("是否有>>2222>>>>>>>" + vvv[0] + ":" + vvv[1]);
                    if (vvv[0] != "" && vvv[1] != "")
                    {
                        if (dic.ContainsKey(vvv[0]) == false)
                        {
                            dic.Add(vvv[0], vvv[1]);

                        }
                    }
                }
            }
        }
        return dic;
    }
    #region 测试的加载
    public static void testLoad(string pathN, Action<GameObject> laterFun, string testPath)
    {
        string cmat = testPath + pathN + ".prefab";
        GameObject g = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(cmat);
        g = GameObject.Instantiate(g);
        laterFun(g);
    }

    #endregion
}