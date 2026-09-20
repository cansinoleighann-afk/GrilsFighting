using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResLoad
{
    private static ResLoad _self;
    public static ResLoad self
    {
        get
        {
            if (_self == null)
            {
                _self = new ResLoad();

            }
            return _self;
        }
    }

    public Dictionary<string, GameObject> resList = new Dictionary<string, GameObject>();

    public Dictionary<string, AssetBundle> ABlist = new Dictionary<string, AssetBundle>();

    private GameObject _pool;
    public GameObject pool
    {
        get
        {
            if (_pool == null)
            {
                _pool = new GameObject("pool");
                _pool.SetActive(false);
            }
            return _pool;
        }
        private set { }

    }

    /// <summary>
    /// 云路径包含的文件夹
    /// </summary>
    string couldPath = "/AssetBundles/WebGL/";

    #region resource加载
    /// <summary>
    /// 获取物体
    /// </summary>
    /// <param name="pathN"></param>
    /// <returns></returns>
    public GameObject loadAll(string pathN,GameObject pre=null,bool isOther=false)
    {
        GameObject g = null;
        if (g == null)
        {
            if (pathN == "" && pre!=null)
            {
                pathN=pre.name;
            }
            string[] sss = pathN.Split('/');
            string nnn = sss[sss.Length - 1];
            for (int i = 0; i < pool.transform.childCount; i++)
            {
                GameObject pg = pool.transform.GetChild(i).gameObject;
                pg.name = pg.name.Replace("(Clone)", "");
                
                if (pg.name == nnn)
                {
                    if (pg.transform.childCount > 0)
                    {
                        g = pg.transform.GetChild(0).gameObject;
                        break;
                    }
       
                }
            }
        }
        if(g== null && pre!=null)
        {
            g = GameObject.Instantiate(pre);
        }
        if (g == null)
        {
            if (resList.ContainsKey(pathN))
            {
                GameObject pg = resList[pathN];
                if (pg != null)
                {
                    g = GameObject.Instantiate(pg);
                }
            }
        }
        if (g == null)
        {
            GameObject pg = resLoad<GameObject>(pathN, isOther);
            if (pg != null)
            {
#if  !UNITY_EDITOR
                pg.SetActive(false);
#else

#endif
                g = GameObject.Instantiate(pg);
                if (resList.ContainsKey(pathN) == false)
                {
                    resList[pathN] = pg;
                }
                else
                {
                    if (resList[pathN] == null)
                    {
                        resList[pathN] = pg;
                    }
                }
            }
        }
        if (g != null)
        {
            g.SetActive(true);
            g.name = g.name.Replace("(Clone)", "");
        }
        return g;
    }

    static string res_test_path = "Assets/test_Resources/";
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T resLoad<T>(string path,bool istest=false) where T : UnityEngine.Object
    {
#if  !UNITY_EDITOR
        istest = false;
#else

#endif
        if (istest == false)
        {
            T obj = Resources.Load<T>(path);
            return obj;
        }
        else
        {
#if !UNITY_EDITOR
            return null;
#else
            
            Debug.Log("测试加载:::" + res_test_path + path);
            string hou= obj_hou[typeof(T).ToString()];
            T obj =  UnityEditor.AssetDatabase.LoadAssetAtPath<T>(res_test_path+path+ hou);
            obj = GameObject.Instantiate<T>(obj);
            return obj;
#endif
        }

    }

    static Dictionary<string,string> obj_hou=new Dictionary<string, string>()
    {
        {"UnityEngine.GameObject",".prefab" },
        {"UnityEngine.Material",".mat" },
        {"UnityEngine.AnimatorController",".controller" },
        {"UnityEngine.RuntimeAnimatorController",".controller" },
    };

    /// <summary>
    /// 移除到缓存池里
    /// </summary>
    /// <param name="g"></param>
    public GameObject removePool(GameObject g,GameObject par=null)
    {
        if (g == null)
        {
            return null;
        }
        g.name = g.name.Replace("(Clone)", "");
        if(par == null)
        {
            for (int i = 0; i < pool.transform.childCount; i++)
            {
                Transform pg = pool.transform.GetChild(i);
                if (pg.name == g.name)
                {
                    par = pg.gameObject;
                }
            }
        }
        if (par == null)
        {
            par = new GameObject(g.name);
            par.transform.SetParent(pool.transform);
        }
        g.transform.SetParent(par.transform);
        return par.gameObject;
    }

    /// <summary>
    /// 异步加载预制体
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fun"></param>
    public void asyncLoad(string path, Action<float, bool, GameObject, EventArg> fun, EventArg arg = null)
    {
        GameObject g = null;
        if (resList.ContainsKey(path) == true)
        {
            GameObject pg = resList[path];
            if (pg != null)
            {
                g = GameObject.Instantiate(pg);
            }
        }
        if (g != null)
        {
            fun(1, true, g, arg);
        }
        else
        {
            Debug.Log("路径:::::" + path);
            Tools.Instance.mono.StartCoroutine(AsyncLoadResources(path, fun));
        }
    }

    IEnumerator AsyncLoadResources(string path, Action<float, bool, GameObject, EventArg> fun, EventArg arg = null)
    {
        ResourceRequest resourcesRequest = Resources.LoadAsync<GameObject>(path);

        while (!resourcesRequest.isDone)
        {
            fun(resourcesRequest.progress, false, null, arg);
            yield return null;
        }
        //Debug.Log("加载进度:::" + resourcesRequest.progress);
        GameObject prefab = resourcesRequest.asset as GameObject;
        GameObject g = GameObject.Instantiate(prefab);
        g.name = g.name.Replace("(Clone)", "");
        if (resList.ContainsKey(path) == false)
        {
            resList[path] = prefab;
        }
        else
        {
            if (resList[path] == null)
            {
                resList[path] = prefab;
            }
        }
        fun(1, true, g, arg);
    }

    #endregion

    #region ab加载
    AssetBundle webGl = null;
    bool isLoadABConfig = false;

    public void initConfig(Action okfun)
    {
        Tools.Instance.mono.StartCoroutine(_initConfig(okfun));
    }

    IEnumerator _initConfig(Action okfun)
    {


#if !UNITY_EDITOR
        string url = AssetBundleConfig.self.pathAB + couldPath + AssetBundleConfig.self.webglV;
        UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
        yield return uwr.SendWebRequest();
        if (string.IsNullOrEmpty(uwr.error) == false)
        {
            Debug.LogError(uwr.error);
            yield break;
        }
        else
        {
            webGl = DownloadHandlerAssetBundle.GetContent(uwr);
        }
#endif

#if UNITY_EDITOR
        string path = Application.dataPath + "/StreamingAssets/AssetBundles/WebGL/" + AssetBundleConfig.self.webglV;
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        yield return request;
        webGl = request.assetBundle;
#endif
        if (okfun != null)
        {
            okfun();
        }
        yield break;
    }

    #region gameoject加载

    /// <summary>
    /// 加载AssetBundle
    /// </summary>
    /// <param name="url"></param>
    /// <param name="loadOkFun"></param>
    /// <returns></returns>
    public void AsyncLoadAB(string nn, Action<GameObject, EventArg> loadOkFun, EventArg arg = null)
    {
        string[] str= nn.Split(';');
        string abName = str[0];
        string packName = str[1];
        //Debug.Log("加载::::" + nn);
        if (ABlist.ContainsKey(packName) == false)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("这里安卓设备");
#endif

#if UNITY_IPHONE && !UNITY_EDITOR
        Debug.Log("这里苹果设备");
#endif
            Tools.Instance.mono.StartCoroutine(LoadABAsync(packName, abName, loadOkFun, arg));
            return;
#if !UNITY_EDITOR
            Tools.Instance.mono.StartCoroutine(LoadABAsync(packName, abName, loadOkFun,arg));
#endif

#if UNITY_EDITOR
            if (isLoadTest == true)
            {
                string kk = "";
                foreach (string key in AssetBundleConfig.self.abList.Keys)
                {
                    string v = AssetBundleConfig.self.abList[key];
                    string[] vv = v.Split(';');
                    if (packName == vv[1] && abName == vv[0])
                    {
                        kk = v;
                    }
                }
                foreach (string key in AssetBundleConfig.self.abAtlasList.Keys)
                {
                    string v = AssetBundleConfig.self.abAtlasList[key];
                    string[] vv = v.Split(';');
                    if (packName == vv[1] && abName == vv[0])
                    {
                        kk = v;
                    }
                }
                Tools.Instance.LaterFun(() =>
                {
                    testLoad(AssetBundleConfig.self.abPcList[kk], (g) => {
                        loadOkFun(g, arg);
                    });
                }, 0.1f);
            }
            else
            {
                Tools.Instance.mono.StartCoroutine(LoaclLoadABAsync(packName, abName, loadOkFun, arg));
            }
            
#endif
        }
        else
        {
            AssetBundle bundle = ABlist[packName];
            GameObject pre = bundle.LoadAsset<GameObject>(abName);
            GameObject g = GameObject.Instantiate(pre);
            g.SetActive(true);
            g.name = g.name.Replace("(Clone)", "");
            if (loadOkFun != null)
            {
                loadOkFun(g, arg);
            }
        }

    }

    IEnumerator AsyncLoadAssetBundle(string packName, string yesPackName, string abName, Action<GameObject, EventArg> loadOkFun, EventArg arg)
    {
        string url = AssetBundleConfig.self.pathAB + couldPath + yesPackName;

        Debug.Log("加载地址:22222::" + url);
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);

        yield return request.SendWebRequest();
        if (string.IsNullOrEmpty(request.error) == false)
        {
            Debug.LogError(request.error);
            yield break;
        }
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        if (ABlist.ContainsKey(packName) == false)
        {
            ABlist[packName] = bundle;
        }
        GameObject pre = bundle.LoadAsset<GameObject>(abName);
        pre.SetActive(false);
        GameObject g = GameObject.Instantiate(pre);
        g.SetActive(true);
        g.name = g.name.Replace("(Clone)", "");
        if (loadOkFun != null)
        {
            loadOkFun(g, arg);
        }

    }

    IEnumerator LoadABAsync(string packName, string abName, Action<GameObject, EventArg> loadOkFun, EventArg arg)
    {
        AssetBundle manifestbundle = webGl;
        if (webGl == null)
        {
            
            string url = AssetBundleConfig.self.pathAB + couldPath + AssetBundleConfig.self.webglV;
            Debug.Log("加载>>>>>>本地>>>>" + url + "    >>>:" + abName);
            UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
            yield return uwr.SendWebRequest();
            if (string.IsNullOrEmpty(uwr.error) == false)
            {
                Debug.LogError(uwr.error);
                yield break;
            }
            manifestbundle = DownloadHandlerAssetBundle.GetContent(uwr);
            webGl = manifestbundle;
        }

        AssetBundleManifest abm = manifestbundle?.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        var hashNames = GetABNamesWithHash(abm);
        var yesPackName = hashNames[packName];
        Tools.Instance.mono.StartCoroutine(AsyncLoadAssetBundle(packName, yesPackName, abName, loadOkFun, arg));
    }
    string loaclABstr = "WebGL";
    /// <summary>
    /// 本地加载ab
    /// </summary>
    /// <param name="packName"></param>
    /// <param name="abName"></param>
    /// <param name="loadOkFun"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    IEnumerator LoaclLoadABAsync(string packName, string abName, Action<GameObject, EventArg> loadOkFun, EventArg arg) 
    {
        //1.第二种加载的方式LoadFromFileAsync异步加载(本地加载)
        AssetBundle manifestbundle = webGl;
        
        if (webGl == null)
        {
            Debug.Log("加载>>>>>>本地>>>>" + abName + "    >>>:" + webGl);
            string path = Application.dataPath + "/StreamingAssets/AssetBundles/"+ loaclABstr + "/"+ loaclABstr + "" + AssetBundleConfig.self.webglV.Replace("WebGL","");
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            manifestbundle = request.assetBundle;
            webGl = manifestbundle;
        }
        AssetBundleManifest abm = manifestbundle?.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        var hashNames = GetABNamesWithHash(abm);
        var yesPackName = hashNames[packName];
        Tools.Instance.mono.StartCoroutine(LoaclAsyncLoadAssetBundle(packName, yesPackName, abName, loadOkFun, arg));

    }

    IEnumerator LoaclAsyncLoadAssetBundle(string packName, string yesPackName, string abName, Action<GameObject, EventArg> loadOkFun, EventArg arg)
    {
        string path = Application.dataPath + "/StreamingAssets/AssetBundles/"+ loaclABstr + "/" + yesPackName;
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        yield return request;
        
        AssetBundle bundle = request.assetBundle;
        if (ABlist.ContainsKey(packName) == false)
        {
            //Debug.Log("保存:::加载ab包:::::" + packName);
            ABlist[packName] = bundle;
        }
        GameObject pre = bundle.LoadAsset<GameObject>(abName);
        //Debug.Log("加载ab包:::::" + pre.name);
        GameObject g = GameObject.Instantiate(pre);
        g.name= g.name.Replace("(Clone)", "");
        if (loadOkFun != null)
        {
            loadOkFun(g, arg);
        }
    }

    #endregion

    #region 范型加载

    /// <summary>
    /// 加载AssetBundle
    /// </summary>
    /// <param name="url"></param>
    /// <param name="loadOkFun"></param>
    /// <returns></returns>
    public void AsyncLoadAB<T>(string nn, Action<T, EventArg> loadOkFun, EventArg arg = null) where T : UnityEngine.Object
    {
        string[] str = nn.Split(';');
        string abName = str[0];
        string packName = str[1];
        if (ABlist.ContainsKey(packName) == false)
        {
            //Tools.Instance.mono.StartCoroutine(LoadABAsync<T>(packName, abName, loadOkFun, arg));
#if  !UNITY_EDITOR
            Tools.Instance.mono.StartCoroutine(LoadABAsync<T>(packName, abName, loadOkFun, arg));
#else
            if (isLoadTest == true)
            {

                string kk = "";
                foreach (string key in AssetBundleConfig.self.abList.Keys)
                {
                    string v = AssetBundleConfig.self.abList[key];
                    string[] vv = v.Split(';');
                    if (packName==vv[1] && abName ==vv[0] )
                    {
                        kk = v;
                    }
                }
                foreach (string key in AssetBundleConfig.self.abAtlasList.Keys)
                {
                    string v = AssetBundleConfig.self.abAtlasList[key];
                    string[] vv = v.Split(';');
                    if (packName == vv[1] && abName == vv[0])
                    {
                        kk = v;
                    }
                }
                bool istestHave = AssetBundleConfig.self.abPcList.ContainsKey(kk);
                if (istestHave == true)
                {
                    Tools.Instance.LaterFun(() =>
                    {
                        testLoad<T>(AssetBundleConfig.self.abPcList[kk], (g) => {
                            loadOkFun(g, arg);
                        });
                    }, 0.1f);

                }
            }
            else
            {
                Tools.Instance.mono.StartCoroutine(LoaclLoadABAsync<T>(packName, abName, loadOkFun, arg));
            }
#endif

        }
        else
        {
            AssetBundle bundle = ABlist[packName];
            T pre = bundle.LoadAsset<T>(abName);
            T g = UnityEngine.Object.Instantiate<T>(pre);
            g.name = g.name.Replace("(Clone)", "");
            if (loadOkFun != null)
            {
                loadOkFun(g, arg);
            }
        }

    }

    IEnumerator AsyncLoadAssetBundle<T>(string packName, string yesPackName, string abName, Action<T, EventArg> loadOkFun, EventArg arg) where T : UnityEngine.Object
    {
        string url = AssetBundleConfig.self.pathAB + couldPath + yesPackName;
        UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);

        yield return request.SendWebRequest();
        if (string.IsNullOrEmpty(request.error) == false)
        {
            Debug.LogError(request.error);
            yield break;
        }
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);
        if (ABlist.ContainsKey(packName) == false)
        {
            ABlist[packName] = bundle;
        }
        T pre = bundle.LoadAsset<T>(abName);
        T g = UnityEngine.Object.Instantiate<T>(pre);
        g.name = g.name.Replace("(Clone)", "");
        if (loadOkFun != null)
        {
            loadOkFun(g, arg);
        }

    }

    /// <summary>
    /// ab包版本更新时清理缓存
    /// </summary>
    /// <param name="abm"></param>
    /// <returns></returns>
    private Dictionary<string, string> GetABNamesWithHash(AssetBundleManifest abm)
    {
        var hashNames = abm.GetAllAssetBundles();
        Dictionary<string, string> ABNamesDict = new Dictionary<string, string>();
        foreach (var hashName in hashNames)
        {
            var abName = Regex.Match(hashName, "_[0-9a-f]{32}$").Success ? hashName.Substring(0, hashName.Length - 33) : hashName;
            ABNamesDict.Add(abName, hashName);
            //Debug.Log("报名:::::::::" + abName + "    :" + hashName);
        }
        return ABNamesDict;
    }

    IEnumerator LoadABAsync<T>(string packName, string abName, Action<T, EventArg> loadOkFun, EventArg arg) where T : UnityEngine.Object
    {

        AssetBundle manifestbundle = webGl;
        if (webGl == null)
        {
            string url = AssetBundleConfig.self.pathAB + couldPath + AssetBundleConfig.self.webglV;
            UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
            yield return uwr.SendWebRequest();
            if (string.IsNullOrEmpty(uwr.error) == false)
            {
                Debug.LogError(uwr.error);
                yield break;
            }
            manifestbundle = DownloadHandlerAssetBundle.GetContent(uwr);
            webGl = manifestbundle;
        }

        AssetBundleManifest abm = manifestbundle?.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        var hashNames = GetABNamesWithHash(abm);
        var yesPackName = hashNames[packName];
        Tools.Instance.mono.StartCoroutine(AsyncLoadAssetBundle(packName, yesPackName, abName, loadOkFun, arg));
    }
    /// <summary>
    /// 本地加载ab
    /// </summary>
    /// <param name="packName"></param>
    /// <param name="abName"></param>
    /// <param name="loadOkFun"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    IEnumerator LoaclLoadABAsync<T>(string packName, string abName, Action<T, EventArg> loadOkFun, EventArg arg) where T : UnityEngine.Object
    {
        
        AssetBundle manifestbundle = webGl;
       // Debug.Log("是否配置了ab包:" + webGl);
        if (webGl == null)
        {
            //1.第二种加载的方式LoadFromFileAsync异步加载(本地加载)
            string path = Application.dataPath + "/StreamingAssets/AssetBundles/Windows/Windows" + AssetBundleConfig.self.webglV.Replace("WebGL", "");
            //string path = Application.dataPath + "/StreamingAssets/AssetBundles/WebGL/" + AssetBundleConfig.self.webglV;
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
            yield return request;
            manifestbundle = request.assetBundle;
            webGl = manifestbundle;
            //Debug.Log("极大;;;;;;;;;;;;;;;;;");
        }

        AssetBundleManifest abm = manifestbundle?.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        var hashNames = GetABNamesWithHash(abm);
        var yesPackName = hashNames[packName];
        Tools.Instance.mono.StartCoroutine(LoaclAsyncLoadAssetBundle(packName, yesPackName, abName, loadOkFun, arg));

    }

    IEnumerator LoaclAsyncLoadAssetBundle<T>(string packName, string yesPackName, string abName, Action<T, EventArg> loadOkFun, EventArg arg) where T : UnityEngine.Object
    {
        //string path = Application.dataPath + "/StreamingAssets/AssetBundles/Windows/Windows" + AssetBundleConfig.self.webglV.Replace("WebGL", "");
        string path = Application.dataPath + "/StreamingAssets/AssetBundles/Windows/" + yesPackName;
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
        yield return request;
        AssetBundle bundle = request.assetBundle;
        if (ABlist.ContainsKey(packName) == false)
        {
            ABlist[packName] = bundle;
        }
        T pre = bundle.LoadAsset<T>(abName);
        T g =UnityEngine.Object.Instantiate<T>(pre);
        g.name = g.name.Replace("(Clone)", "");
        if (loadOkFun != null)
        {
            loadOkFun(g, arg);
        }

    }
    #endregion

    #endregion

    #region 图集加载

    Dictionary<string,List<Action<SpriteAtlas, EventArg>>> atlasOkFun = new Dictionary<string, List<Action<SpriteAtlas, EventArg>>>();

    Dictionary<string, SpriteAtlas> atlasList = new Dictionary<string, SpriteAtlas>();
    /// <summary>
    /// 图集加载
    /// </summary>
    /// <param name="nn">图集路径或ab包路径</param>
    /// <param name="loadOkFun">回调方法</param>
    /// <param name="isResouces">是否是Resouces加载或Ab</param>
    /// <param name="arg"></param>
    public void AsyncLoadAtlas(string path, Action<SpriteAtlas, EventArg> loadOkFun, EventArg arg = null)
    {
        //Debug.Log("ab包加载图集图片:::::::" + path);

        string[] pathlist = path.Split('/');
        string nn = pathlist[pathlist.Length-1];
        if (AssetBundleConfig.self.abAtlasList.ContainsKey(nn)==false)
        {
            if (atlasList.ContainsKey(path) == false)
            {
                SpriteAtlas at = Resources.Load<SpriteAtlas>(path);
                atlasList[path] = at;
            }
            else
            {
                if (atlasList[path] == null)
                {
                    SpriteAtlas at = Resources.Load<SpriteAtlas>(path);
                    atlasList[path] = at;
                }
            }
            
            if (loadOkFun != null)
            {
                loadOkFun(atlasList[path], arg);
            }
        }
        else
        {
            if (atlasOkFun.ContainsKey(nn) == true)
            {
                atlasOkFun[nn].Add(loadOkFun);
               // Debug.Log("添加图集>>>>>>>>>>:图集:::" + nn);
            }
            else
            {
                atlasOkFun.Add(nn, new List<Action<SpriteAtlas, EventArg>>());
                atlasOkFun[nn].Add(loadOkFun);
                //Debug.Log("新增加:图集:::" + nn);
                AsyncLoadAB<SpriteAtlas>(AssetBundleConfig.self.abAtlasList[nn], (at, arg1) =>
                {
                    for (int i = 0; i < atlasOkFun[nn].Count; i++)
                    {
                        atlasOkFun[nn][i](at, arg);
                    }
                    atlasOkFun.Remove(nn);
                });
            }

        }
    }

    /// <summary>
    /// 异步赋值image
    /// </summary>
    /// <param name="ima"></param>
    /// <param name="atlasPath"></param>
    /// <param name="spriteStr"></param>
    public void SetAtlasImage(Image ima,string atlasPath,string spriteStr)
    {
        //Debug.Log("加载图集图片:::::::" + atlasPath);
        AsyncLoadAtlas(atlasPath, (at, evt) =>
        {
            //Debug.Log("加载图集图片:::::::" + ima+" :"+at);
            if (ima!=null && ima.ToString() != "null")
            {
                //Debug.Log("加载图集图片::2222:::::" + ima + " :" + at);
                ima.sprite = at.GetSprite(spriteStr);
            }
        });
    }

    #endregion

    #region 加载场景
    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="fun"></param>
    public void LoadSceneAsync(string sceneName, Action<Scene> fun)
    {
        //Debug.Log("加载地图:::::" + sceneName);
        Tools.Instance.mono.StartCoroutine(_LoadSceneAsync(sceneName, fun));
    }

    IEnumerator _LoadSceneAsync(string sceneName, Action<Scene> fun)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.isLoaded)
        {
            Debug.Log("场景已经存在 : " + sceneName);
            yield break;
        }
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);


        //如果不需要等待，可直接加载后跳转场景
        operation.allowSceneActivation = true;
        while (!operation.isDone)
        {
            yield return null;
        }
        scene = SceneManager.GetSceneByName(sceneName);
        fun?.Invoke(scene);
    }
    /// <summary>
    /// 删除场景
    /// </summary>
    /// <param name="sceneName"></param>
    public void UnloadSceneAsync(string sceneName, Action okFun = null)
    {
        Tools.Instance.mono.StartCoroutine(_UnloadSceneAsync(sceneName, okFun));
    }

    IEnumerator _UnloadSceneAsync(string sceneName, Action okFun)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (!scene.isLoaded)
        {
            Debug.Log("没有这个场景: " + sceneName);
            yield break;
        }

        AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
        yield return async;
        okFun?.Invoke();
    }

    #endregion

    #region 加载图片
    Dictionary<string, Texture2D> huanTex = new Dictionary<string, Texture2D>();
    Dictionary<string, List<Action<Texture2D>>> huanTex_fun = new Dictionary<string, List<Action<Texture2D>>>();
    /// <summary>
    /// 加载网络图片
    /// </summary>
    /// <param name="url"></param>
    /// <param name="ima"></param>
    public void net_tex_image(string url, Image ima, Dictionary<string, string> head=null)
    {
        if (url != "")
        {
            if (huanTex_fun.ContainsKey(url) == false)
            {
                huanTex_fun.Add(url, new List<Action<Texture2D>>());
            }
            huanTex_fun[url].Add((res) =>
            {
                if (ima != null)
                {
                    ima.sprite = Tools.Instance.texToSprite(res);
                }
            });
            if (huanTex.ContainsKey(url) == true)
            {
                if (huanTex[url] != null)
                {
                    for (int i = 0; i < huanTex_fun[url].Count; i++)
                    {
                        huanTex_fun[url][i](huanTex[url]);
                    }
                    huanTex_fun.Remove(url);
                }
            }
            else
            {
                if (huanTex_fun[url].Count == 1)
                {
                    load_net_tex(url, (res) =>
                    {
                        huanTex.Add(url, res);
                        Debug.Log("加载图片》》头像：》》》》》" + res);
                        for (int i = 0; i < huanTex_fun[url].Count; i++)
                        {
                            huanTex_fun[url][i](res);
                        }
                        huanTex_fun.Remove(url);
                    }, head);
                }
            }
        }
    }

    public void load_net_tex(string url, Action<Texture2D> fun, Dictionary<string, string> head)
    {
        Tools.Instance.mono.StartCoroutine(_GetTexture(url, fun, head));
    }

    IEnumerator _GetTexture(string url, Action<Texture2D> actionResult, Dictionary<string, string> head)
    {
        //UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, 0);
        UnityWebRequest uwr = new UnityWebRequest(new Uri(url));
        if (head != null)
        {
            foreach (var key in head.Keys)
            {
                Debug.Log("设置请求头::::::::" + url + "  :" + key + "  :" + head[key]);
                uwr.SetRequestHeader(key, head[key]);
            }
        }
        DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
        uwr.downloadHandler = downloadTexture;
        Texture2D t = null;
        yield return uwr.SendWebRequest();
        if (string.IsNullOrEmpty(uwr.error) == false)
        {
            Debug.Log("下载失败" + uwr.error);
            yield break;
        }
        else
        {
            t = downloadTexture.texture;
        }
        if (actionResult != null)
        {
            actionResult(t);
        }
    }

    #endregion

    #region 微信字体加载
    public Font wxFont=null;
    public void load_wx_zi(Action<Font> okFun)
    {
        //var fallbackFont = "https://lingmeng-1317113396.cos.ap-guangzhou.myqcloud.com/yiyouGame/StreamingAssets/wxzi1.ttf";
#if PT_wx
        if (wxFont == null)
        {
            WeChatWASM.WX.GetWXFont("", (_font) =>
            {
                wxFont = _font;
                okFun?.Invoke(wxFont);
            });
        }
        else
        {
            okFun?.Invoke(wxFont);
        }
#endif
    }
    /// <summary>
    /// 从新设置界面的字体
    /// </summary>
    public bool is_view_reset_font = false;
    public void set_view_font(GameObject view)
    {
        if(is_view_reset_font==true)
        {
            set_text_font(view, wxFont);
        }
    }
    /// <summary>
    /// 设置字体
    /// </summary>
    /// <param name="g"></param>
    /// <param name="font"></param>
    public void set_text_font(GameObject g, Font font)
    {
        if (font != null)
        {
            Text gt = g.GetComponent<Text>();
            if (gt != null)
            {
                gt.font = font;
            }
            Tools.Instance.GetAllObj_fun(g, (gg) =>
            {
                Text text = gg.GetComponent<Text>();
                if (text != null && text.font!= font)
                {
                    text.font = font;
                }
                return false;
            });
        }

    }
#endregion

    public void clearAll()
    {
        ClearAb();
        ClearResource();
    }

    public void ClearAb(bool isClearView=true)
    {
        IEnumerable<AssetBundle> abList = AssetBundle.GetAllLoadedAssetBundles();
        List<string> rm = new List<string>();
        foreach (AssetBundle item in abList)
        {
            //Debug.Log("清理>>>>>>>>>" + item.name);
            if (AssetBundleConfig.self.noClearPack.Contains(item.name) == false && item!=webGl && item.name!="" && item.name!= AssetBundleConfig.self.webglV)
            {
                rm.Add(item.name);
                item.Unload(true);
            }
        }

        
        List<string> viewList = new List<string>();
        foreach (string key in ViewManage.Instance.ViewPreList.Keys)
        {
            viewList.Add(key);
        }

        for (int i = 0; i < rm.Count; i++)
        {
            ABlist.Remove(rm[i]);
            for (int k = 0; k < viewList.Count; k++)
            {
                if(viewList[k].ToLower()== rm[i])
                {
                    GameObject vg = ViewManage.Instance.ViewPreList[viewList[k]];
                    ViewManage.Instance.ViewPreList.Remove(viewList[k]);
                    GameObject.Destroy(vg);
                    break;
                }
            }
        }
        atlasList = new Dictionary<string, SpriteAtlas>();
        Debug.Log("清理ab包::::" + webGl);
    }

    public void ClearResource()
    {
        Tools.Instance.ClearObj(pool);
        resList = null;
        resList = new Dictionary<string, GameObject>();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }


    #region 测试的加载

    public bool isLoadTest = true;

    public string testPath = "";
    public void testLoad(string pathN, Action<GameObject> laterFun = null)
    {
        string cmat = pathN;
        if (cmat.Contains(".prefab") == false)
        {
            cmat += ".prefab";
        }
        
        GameObject g = null;
#if !UNITY_EDITOR

#else
        Debug.Log("路径>>>>>>>>" + cmat);
        g = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(cmat);
#endif
        if (g != null)
        {
            g.SetActive(false);
            g = GameObject.Instantiate(g);
            g.SetActive(true);
        }
        laterFun(g);
    }
    public void testLoad<T>(string pathN, Action<T> laterFun = null) where T : UnityEngine.Object
    {
        //Debug.Log("路径>>>>>>>>" + pathN);
        T g = null;
#if !UNITY_EDITOR

#else
        g = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(pathN);
#endif
        g = GameObject.Instantiate<T>(g);
        laterFun(g);
    }

    string audioPath = "Assets/Assetbundle/Audio/";
    string res_audioPath = "Audio/";
    public AudioClip LoadAudio(string pathN)
    {
        AudioClip g = null;
        pathN = res_audioPath + pathN;
        string[] str = pathN.Split('.');
        pathN = str[0];
        //Debug.Log("路径>>>>>>>" + pathN);
        g = Resources.Load<AudioClip>(pathN);
        //Debug.Log("音乐::::::" + g);
        return g;
    }
    #endregion
}