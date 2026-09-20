using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections;
using Unity.Jobs;

public class Tools
{
    private static Tools _instance;
    public static Tools Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Tools();

            }
            return _instance;
        }
    }
    public Tools()
    {

    }

    class monoScr : MonoBehaviour { };
    MonoBehaviour _mono = null;
    public MonoBehaviour mono
    {
        get
        {
            if (_mono == null)
            {
                GameObject gg = new GameObject();
                _mono = gg.AddComponent<monoScr>();
            }
            return _mono;
        }
        set
        {
            _mono = value;
        }
    }

    public void initMono()
    {

    }

    /// <summary>
    /// 延迟隐藏物体
    /// </summary>
    public void LaterActive(GameObject g, float t, bool isActive = false)
    {
        mono.StartCoroutine(IELaterActive(g, t, isActive));
    }
    IEnumerator IELaterActive(GameObject g, float t, bool isActive = false)
    {
        yield return new WaitForSeconds(t);
        g.SetActive(isActive);
    }


    /// <summary>
    /// 延迟执行方法
    /// </summary>
    /// <param name="fun"></param>
    /// <param name="t"></param>
    /// <param name="isTimeScale">是否受Time.timeScale影响</param>
    /// <returns></returns>
    public IEnumerator LaterFun(Action fun, float t,bool isTimeScale=true)
    {
        IEnumerator ie =null;
        if (isTimeScale == false)
        {
            ie = IELaterFunNoTime(fun, t);
        }
        else
        {
            ie = IELaterFun(fun, t);
        }
        mono.StartCoroutine(ie);
        return ie;
    }
    IEnumerator IELaterFun(Action fun, float t)
    {
        yield return new WaitForSeconds(t);
        fun();
    }
    IEnumerator IELaterFunNoTime(Action fun, float t)
    {
        yield return new WaitForSecondsRealtime(t);
        fun();
    }


    /// <summary>
    /// 循环执行 返回true就是结束循环
    /// </summary>
    /// <param name="g"></param>
    /// <param name="t"></param>
    /// <param name="isActive"></param>
    public void lookFun(Func<bool> action, float offT = 0)
    {
        mono.StartCoroutine(_lookFun(action, offT));
    }
    IEnumerator _lookFun(Func<bool> action, float t = 0)
    {
        bool isOk = false;
        while (isOk == false)
        {
            isOk = action();
            if (t == 0)
            {
                yield return null;
            }
            else
            {
                yield return new WaitForSeconds(t);
            }
        }
    }

    Dictionary<GameObject, IEnumerator> LaterGlist = new Dictionary<GameObject, IEnumerator>();
    /// <summary>
    /// 延迟执行,并且在延迟事件内重复会刷新时间
    /// </summary>
    /// <param name="g"></param>
    /// <param name="fun"></param>
    /// <param name="t"></param>
    public void LaterOneFun(GameObject g, Action<GameObject> fun, float t)
    {
        if (g == null)
        {
            return;
        }
        if (LaterGlist.ContainsKey(g) == true)
        {
            mono.StopCoroutine(LaterGlist[g]);
            LaterGlist.Remove(g);
        }
        IEnumerator ie = IELaterFun(() =>
        {
            fun(g);
            if (LaterGlist.ContainsKey(g) == true)
            {
                LaterGlist.Remove(g);
            }
        }, t);
        LaterGlist.Add(g, ie);
    }

    //加载图片
    public void LoadTex(string url, Action<Texture2D> fun, bool isHttp = false, int x = 500, int y = 500)
    {
        if (AllTex.ContainsKey(url) == true)
        {
            Texture2D tex = Texture2D.Instantiate(AllTex[url]);
            fun(tex);
        }
        else
        {
            if (isHttp)
            {
                mono.StartCoroutine(loadtex(url, fun, x, y));
            }
            else
            {
                Texture2D tex = Resources.Load<Texture2D>(url);
                fun(tex);
            }
        }
    }
    public Dictionary<string, Texture2D> AllTex = new Dictionary<string, Texture2D>();
    IEnumerator loadtex(string url, Action<Texture2D> fun, int x, int y) // 协程
    {
        UnityWebRequest wr = new UnityWebRequest(url);

        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();
        int width = x;
        int high = y;
        if (string.IsNullOrEmpty(wr.error) == false)
        {
            Debug.Log("加载图片出错：" + wr.error);
            yield break;
        }
        else
        {
            Texture2D tex = new Texture2D(width, high);
            tex = texDl.texture;
            if (AllTex.ContainsKey(url) == false)
            {
                AllTex.Add(url, tex);
            }
            Texture2D tex1 = Texture2D.Instantiate(AllTex[url]);
            fun(tex1);
        }
    }

    /// <summary>
    /// Texture2D转成sprite
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    public Sprite texToSprite(Texture2D tex)
    {
        Sprite prite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        return prite;
    }

    /**清空子物体 */
    public void ClearObj(GameObject CarParent, GameObject noG = null)
    {
        List<GameObject> ClearCarList = new List<GameObject>();
        for (int index = 0; index < CarParent.transform.childCount; index++)
        {
            GameObject g = CarParent.transform.GetChild(index).gameObject;
            if (g != noG)
            {
                g.SetActive(false);
                ClearCarList.Add(g);
            }
        }
        GameObject[] rootS = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        Transform root = rootS[0].transform.parent;
        for (int index = 0; index < ClearCarList.Count; index++)
        {
            ClearCarList[index].transform.SetParent(root);
            GameObject.Destroy(ClearCarList[index]);
        }
        //Debug.Log("清空车:" + CarParent.transform.childCount);
    }

    /// <summary>
    /// 克隆ui
    /// </summary>
    /// <param name="pre"></param>
    /// <param name="par"></param>
    /// <returns></returns>
    public GameObject newUiObj(GameObject pre, GameObject par)
    {
        GameObject g = GameObject.Instantiate(pre);
        g.SetActive(true);
        g.transform.SetParent(par.transform, false);
        return g;
    }

    /// <summary>
    /// 设置toggle组件,点击显示隐藏底下的select
    /// </summary>
    /// <param name="par">父级</param>
    /// <param name="oriN">toggle名字</param>
    /// <param name="btnFun">点击回调</param>
    /// <param name="oriIndex">开始显示的索引</param>
    public ToggleUse initToggle(GameObject par, string oriN, Action<GameObject> btnFun, int oriIndex = 0)
    {
        GameObject oriG = null;
        Dictionary<string, GameObject> dic = new Dictionary<string, GameObject>();
        for (int i = 0; i < par.transform.childCount; i++)
        {
            GameObject g = GetOneByName(oriN + i, par);
            if (g != null)
            {
                dic.Add(oriN + i, g);
                if (oriIndex == i)
                {
                    oriG = g;
                }
            }
        }
        ToggleUse toggle = new ToggleUse(dic, btnFun, par, oriN);
        toggle.setToggle(oriG);
        return toggle;
    }


    #region 处理方法


    public static bool isDebug = false;
    /// <summary>
    /// 打印
    /// </summary>
    /// <param name="list"></param>
    public static void debug(params object[] list)
    {
        if (isDebug == true)
        {
            string str = "";
            for (int i = 0; i < list.Length; i++)
            {
                string ss = "";
                if (list[i] != null)
                {
                    ss = list[i].ToString();
                }
                else
                {
                    ss = "空";
                }
                str += ">>>>>" + ss;
            }
            Debug.Log(str);
        }
    }

    /// <summary>
    /// 合并字典 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public Dictionary<T, T> mergeDic<T>(Dictionary<T, T> a, Dictionary<T, T> b)
    {
        Dictionary<T, T> newDic = new Dictionary<T, T>();
        if (a != null)
        {
            foreach (var key in a.Keys)
            {
                newDic.Add(key, a[key]);
            }
        }
        if (b != null)
        {
            foreach (var key1 in b.Keys)
            {
                if (newDic.ContainsKey(key1) == false)
                {
                    newDic.Add(key1, b[key1]);
                }
            }
        }
        return newDic;
    }

    /// <summary>
    /// 将c#时间转成jave的时间戳 毫秒
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public long timeToSeconds(DateTime date)
    {
        return ToUnixMillis(date);
    }

    private long ToUnixMillis(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)diff.TotalMilliseconds;

        //DateTime dd = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        //DateTime timeUTC = DateTime.SpecifyKind(date, DateTimeKind.Utc);//本地时间转成UTC时间
        //TimeSpan ts = (timeUTC - dd);
        //return (long)ts.TotalMilliseconds;

    }

    /// <summary>
    /// java的毫秒转成c#的时间戳
    /// </summary>
    /// <param name="unixMillis"></param>
    /// <returns></returns>
    public DateTime secondsToTime(long unixMillis)
    {
        DateTime origin = new DateTime(1970, 1, 1, 8, 0, 0, 0, DateTimeKind.Local);
        //Debug.Log("   时间:::::::::::"+ unixMillis);
        return origin.AddMilliseconds(unixMillis);
    }

    /// <summary>
    /// 获取格式str="key1:value1;key1:value1"
    /// </summary>
    /// <param name="sss"></param>
    /// <returns></returns>
    public Dictionary<string, string> getDataDic(string sss)
    {
        Dictionary<string, string> v = new Dictionary<string, string>();
        string[] str = sss.Split(';');
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != "")
            {
                string[] ss = str[i].Split(':');
                v.Add(ss[0], ss[1]);
            }
        }
        return v;
    }

    /// <summary>
    /// 设置格式str="key1:value1;key1:value1"
    /// </summary>
    /// <param name="v"></param>
    /// <param name="savekey">要保存的data key</param>
    /// <returns></returns>
    public string setDataDic(Dictionary<string, string> v, string savekey = "")
    {
        string str = "";
        bool one = false;
        foreach (string key in v.Keys)
        {
            if (one == false)
            {
                str = key + ":" + v[key];
            }
            else
            {
                str += ";" + key + ":" + v[key];
            }
            one = true;
        }
        if (savekey != "")
        {
            DataTools.save(savekey, str);
        }
        return str;
    }

    /// <summary>
    /// 增加key值
    /// </summary>
    /// <param name="v"></param>
    /// <param name="key"></param>
    /// <param name="fun"></param>
    /// <returns></returns>
    public Dictionary<string, string> setDataKey(Dictionary<string, string> v, string key, Func<string, string, string> fun)
    {
        if (v.ContainsKey(key) == true)
        {
            v[key] = fun(key, v[key]);
        }
        else
        {
            v.Add(key, fun(key, ""));
        }
        return v;
    }

    /// <summary>
    /// 新建一样列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public List<T> cloneList<T>(List<T> list)
    {
        List<T> l = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            l.Add(list[i]);
        }
        return l;
    }

    /// <summary>
    /// 给数值添加元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public T[] add_array<T>(T[] array, T item)
    {
        if (array == null)
        {
            return new T[] { item };
        }
        return array.Concat(new T[] { item }).ToArray();
    }

    /// <summary>
    /// string转枚举(string和枚举的key要一样)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <returns></returns>
    public T ToEnum<T>(string str)
    {
        //Debug.Log("字段:::::" + str);
        return (T)Enum.Parse(typeof(T), str);
    }
    /// <summary>
    /// 获取枚举的所有元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public List<T> GetEnumValues<T>() where T : Enum
    {
        return new List<T>((T[])Enum.GetValues(typeof(T)));
    }
    /// <summary>
    /// 去除列表的空key
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public List<T> listRemoveNull<T>(List<T> list)
    {
        List<T> l = new List<T>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].ToString() != "null")
            {
                //Debug.Log("数:1::" + list[i]);
                l.Add(list[i]);
            }
            else
            {
                //Debug.Log("数:2::" + list[i]);

            }
        }
        return l;
    }

    /// <summary>
    /// 获取当前tran在父节点的索引位置
    /// </summary>
    /// <param name="tran"></param>
    /// <returns></returns>
    public int getChildIndex(Transform tran)
    {
        int index = 0;
        for (int i = 0; i < tran.parent.childCount; i++)
        {
            if (tran.parent.GetChild(i) == tran)
            {
                index = i;
                break;
            }
        }
        return index;
    }

    /// <summary>
    /// 显示隐藏子物体
    /// </summary>
    /// <param name="par"></param>
    /// <param name="isShow"></param>
    /// <param name="fun"></param>
    public void setChildActive(GameObject par, bool isShow, Action<GameObject> fun = null)
    {
        if (par != null)
        {
            for (int i = 0; i < par.transform.childCount; i++)
            {
                GameObject g = par.transform.GetChild(i).gameObject;
                g.SetActive(isShow);
                if (fun != null)
                {
                    fun(g);
                }
            }
        }
    }

    public List<GameObject> GetAllObj_fun(GameObject g, Func<GameObject, bool> selectG)
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            GameObject gh = g.transform.GetChild(i).gameObject;
            if (selectG(gh) == true)
            {
                alllist.Add(gh);
            }
            if (g.transform.childCount > 0)
            {
                List<GameObject> glist = GetAllObj_fun(gh, selectG);
                alllist.AddRange(glist);
            }
        }
        return alllist;
    }

    /// <summary>
    /// 获取所有的gameobject
    /// </summary>
    /// <param name="g"></param>
    /// <param name="isHaveActive">true获取所有物体,包括隐藏的,false只获取显示的物体</param>
    /// <param name="haveStr">是否包含的字段</param>
    /// <returns></returns>
    public List<GameObject> GetSelectAllObj(GameObject g, bool isHaveActive = true, string haveStr = "")
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            GameObject gh = g.transform.GetChild(i).gameObject;
            if (isHaveActive == false)
            {

                if (gh.activeInHierarchy == true)
                {

                    if (haveStr == "")
                    {
                        alllist.Add(gh);
                    }
                    else
                    {
                        if (gh.name.Contains(haveStr) == true)
                        {
                            alllist.Add(gh);
                        }
                    }
                }
            }
            else
            {
                if (haveStr == "")
                {
                    alllist.Add(gh);
                }
                else
                {
                    if (gh.name.Contains(haveStr) == true)
                    {
                        alllist.Add(gh);
                    }
                }
            }

            if (isHaveActive == false)
            {
                if (gh.activeInHierarchy == false)
                {
                    continue;
                }
            }
            if (g.transform.childCount > 0)
            {
                List<GameObject> glist = GetObjAll(gh, isHaveActive, haveStr);
                alllist.AddRange(glist);
            }
        }
        return alllist;
    }

    private List<GameObject> GetObjAll(GameObject g, bool isHaveActive, string haveStr)
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            GameObject gh = g.transform.GetChild(i).gameObject;
            if (isHaveActive == false)
            {
                if (gh.activeInHierarchy == true)
                {
                    if (haveStr == "")
                    {
                        alllist.Add(gh);
                    }
                    else
                    {
                        if (gh.name.Contains(haveStr) == true)
                        {
                            alllist.Add(gh);
                        }
                    }
                }
            }
            else
            {
                if (haveStr == "")
                {
                    alllist.Add(gh);
                }
                else
                {
                    if (gh.name.Contains(haveStr) == true)
                    {
                        alllist.Add(gh);
                    }
                }
            }
            if (isHaveActive == false)
            {
                if (gh.activeInHierarchy == false)
                {
                    continue;
                }
            }
            if (gh.transform.childCount > 0)
            {
                alllist.AddRange(GetObjAll(gh, isHaveActive, haveStr));
            }
        }
        return alllist;
    }

    /// <summary>
    /// 通过名字获取对象
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="g"></param>
    /// <returns></returns>
    public GameObject GetGameObjectByName(string objName, GameObject g)
    {
        if (g == null)
        {
            return null;
        }
        GameObject obj = null;
        if (g.name == objName)
        {
            return g;
        }
        else
        {
            Transform tran = g.transform;
            obj = GetObjAll(tran, objName);
        }

        if (obj != null)
        {
            return obj;
        }
        else
        {
            Debug.LogWarning("在" + g.name + "找不到物体:" + objName);
            return null;
        }
    }
    public T GetComponentByName<T>(string objName, GameObject g = null)
    {
        GameObject FindObj = GetGameObjectByName(objName, g);

        if (FindObj != null)
        {
            return FindObj.GetComponent<T>();
        }
        else
        {
            Debug.Log("在" + objName + "找不到该组件:>>>>>>>>>>>>>>>>>>>>>>>>>");
            return default(T);
        }
    }

    private GameObject GetObjAll(Transform t, string tname)
    {
        GameObject g = null;
        for (int i = 0; i < t.childCount; i++)
        {
            //Debug.Log("all:" + t.GetChild(i).gameObject.name);
            if (tname == t.GetChild(i).gameObject.name)
            {
                g = t.GetChild(i).gameObject;
                break;
            }
            if (g == null)
            {
                if (t.GetChild(i).childCount > 0)
                {
                    g = GetObjAll(t.GetChild(i), tname);
                    if (g != null)
                    {
                        break;
                    }
                }
            }
        }
        return g;
    }

    /// <summary>
    /// 寻找第一层的obj
    /// </summary>
    /// <param name="n"></param>
    /// <param name="g"></param>
    /// <returns></returns>
    public GameObject GetOneByName(string n, GameObject g)
    {
        Transform t = g.transform.Find(n);
        if (t != null)
        {
            return t.gameObject;
        }
        else
        {
            return null;
        }

    }

    /// <summary>
    /// 寻找第一层的obj组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="objName"></param>
    /// <param name="g"></param>
    /// <returns></returns>
    public T GetOneComponentByName<T>(string objName, GameObject g)
    {

        Transform FindObj = g.transform.Find(objName);

        if (FindObj != null)
        {
            return FindObj.GetComponent<T>();
        }
        else
        {
            Debug.Log("找不到该组件:" + objName);
            return default(T);
        }
    }
    /// <summary>
    /// 判断一个字符是否是数字
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsNumeric(string value)
    {
        if (value != "")
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        else
        {
            return false;
        }

    }
    /// <summary>
    /// 判断一个字符是否是整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsInt(string value)
    {
        return Regex.IsMatch(value, @"^[+-]?\d*$");
    }
    public bool isNaN(float v)
    {
        if (float.IsNaN(v) || float.IsInfinity(v) || float.IsNegativeInfinity(v) || float.IsPositiveInfinity(v))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public bool isNaN(double v)
    {
        if (double.IsNaN(v) || double.IsInfinity(v) || double.IsNegativeInfinity(v) || double.IsPositiveInfinity(v))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    /// <summary>
    /// 排序
    /// </summary>
    /// <param name="list"></param>
    /// <param name="isup">true 从大到小</param>
    public void sort(List<int> list, bool isup = false)
    {
        if (isup)//降序排列
        {
            list.Sort((x, y) => -x.CompareTo(y));
        }
        else //升序
        {
            list.Sort((x, y) => x.CompareTo(y));
        }

    }

    /// <summary>
    /// update平滑插值
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="end"></param>
    /// <param name="bili"></param>
    /// <returns></returns>
    public Vector3 Lerp(Vector3 ori, Vector3 end, float bili)
    {
        return Vector3.Lerp(ori, end, bili * Time.deltaTime * 60f);
    }
    /// <summary>
    /// update平滑插值
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="end"></param>
    /// <param name="bili"></param>
    /// <returns></returns>
    public Quaternion Lerp(Quaternion ori, Quaternion end, float bili)
    {
        return Quaternion.Lerp(ori, end, bili * Time.deltaTime * 60f);
    }
    /// <summary>
    /// update平滑插值
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="end"></param>
    /// <param name="bili"></param>
    /// <returns></returns>
    public Vector2 Lerp(Vector2 ori, Vector2 end, float bili)
    {
        return Vector2.Lerp(ori, end, bili * Time.deltaTime * 60f);
    }
    /// <summary>
    /// update平滑插值
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="end"></param>
    /// <param name="bili"></param>
    /// <returns></returns>
    public float Lerp(float ori, float end, float bili)
    {
        return Mathf.Lerp(ori, end, bili * Time.deltaTime * 60f);
    }

    /// <summary>
    /// 限制v的最大小值
    /// </summary>
    /// <param name="v"></param>
    /// <param name="small"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public float setMax(float v, float small, float max)
    {
        if (v > max)
        {
            v = max;
        }
        if (v < small)
        {
            v = small;
        }
        return v;
    }
    public int setMax(int v, int small, int max)
    {
        if (v > max)
        {
            v = max;
        }
        if (v < small)
        {
            v = small;
        }
        return v;
    }

    /// <summary>
    /// 重置物体的本地坐标
    /// </summary>
    /// <param name="tran"></param>
    public void resetTran(Transform tran)
    {
        tran.localPosition = Vector3.zero;
        tran.localEulerAngles = Vector3.zero;
        tran.localScale = Vector3.one;
    }

    /// <summary>
    /// 从列表里随机不同的数据出来
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="randomNum"></param>
    /// <returns></returns>
    public List<T> random_list<T>(List<T> list, int randomNum)
    {
        List<T> a = new List<T>();

        List<T> b = cloneList(list);

        if (list.Count <= randomNum)
        {
            a = b;
        }
        else
        {
            for (int i = 0; i < randomNum; i++)
            {
                int index = UnityEngine.Random.Range(0, b.Count);
                a.Add(b[index]);
                b.RemoveAt(index);
            }
        }
        return a;
    }

    /// <summary>
    /// 从列表里随机不同的数据出来
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="randomNum"></param>
    /// <returns></returns>
    public Dictionary<Y, T> random_dic<Y, T>(Dictionary<Y, T> list, int randomNum)
    {
        Dictionary<Y, T> a = new Dictionary<Y, T>();
        Dictionary<Y, T> b = new Dictionary<Y, T>();
        foreach (var item in list.Keys)
        {
            b.Add(item, list[item]);
        }

        if (list.Count <= randomNum)
        {
            a = b;
        }
        else
        {
            for (int i = 0; i < randomNum; i++)
            {
                int index = UnityEngine.Random.Range(0, b.Count);
                int jj = 0;
                foreach (var item in b.Keys)
                {
                    if (jj == index)
                    {
                        a.Add(item, b[item]);
                        b.Remove(item);
                        break;
                    }
                    jj++;
                }
            }
        }
        return a;
    }

    /// <summary>
    /// 秒转成  00：00格式
    /// </summary>
    /// <param name="second"></param>
    /// <returns></returns>
    public string timeToStr(int second)
    {
        var m = second / 60;
        var s = second % 60;
        var h = 0;
        if (m > 60)
        {
            h = m / 60;
            m = m % 60;
        }
        if (h > 0)
        {
            return $"{h:00}:{m:00}:{s:00}";
        }
        else
        {
            return $"{m:00}:{s:00}";
        }

    }

    /// <summary>
    /// string转数字
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public int stringToNum(string str)
    {
        if (IsNumeric(str) == false)
        {
            str = "0";
        }
        return int.Parse(str);
    }

    /// <summary>
    /// 设置显示隐藏
    /// </summary>
    /// <param name="g"></param>
    /// <param name="isAction"></param>
    public void setAciton(GameObject g, bool isAction)
    {
        if (g.activeInHierarchy != isAction)
        {
            g.SetActive(isAction);
        }
    }

    /// <summary>
    /// 限制一个坐标在正方型内
    /// </summary>
    public Vector2 limitToCube(Vector2 pos, float min_mask)
    {
        float ax = Mathf.Abs(pos.x);
        float ay = Mathf.Abs(pos.y);
        if (ax > min_mask || ay > min_mask)
        {
            float off = 1;
            if (ax > ay)
            {
                off = min_mask / (ax / ay);
                off = Mathf.Sqrt(Mathf.Pow(off, 2) + Mathf.Pow(min_mask, 2));
            }
            else
            {
                off = min_mask * (ax / ay);
                off = Mathf.Sqrt(Mathf.Pow(off, 2) + Mathf.Pow(min_mask, 2));
            }
            pos = pos.normalized * off;
        }
        return pos;
    }

    /// <summary>
    /// 随机单位园内点
    /// </summary>
    /// <param name="is_edge">是否只随机边缘</param>
    /// <returns></returns>
    public Vector2 Random_Circle(bool is_edge = false)
    {
        Vector2 vv = UnityEngine.Random.insideUnitCircle;
        if (is_edge == true)
        {
            vv = vv.normalized;
        }
        return vv;
    }

    public int GetLayerID(string layerName)
    {
        for (int i = 0; i < 32; i++) // Unity默认支持32个层
        {
            if (LayerMask.LayerToName(i) == layerName)
            {
                return i;
            }
        }
        return 0;
    }

    // 保留小数
    private float MULT_1 = 10f;
    private float MULT_2 = 100f;
    private float MULT_3 = 1000f;
    private float MULT_4 = 10000f;
    private float INV_MULT_1 = 0.1f;
    private float INV_MULT_2 = 0.01f;
    private float INV_MULT_3 = 0.001f;
    private float INV_MULT_4 = 0.0001f;
    /// <summary>
    /// 保留1小数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public float RoundTo1(float value) => Mathf.Round(value * MULT_1) * INV_MULT_1;
    /// <summary>
    /// 保留2小数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public float RoundTo2(float value) => Mathf.Round(value * MULT_2) * INV_MULT_2;
    public float RoundTo3(float value) => Mathf.Round(value * MULT_3) * INV_MULT_3;
    public float RoundTo4(float value) => Mathf.Round(value * MULT_4) * INV_MULT_4;
    #endregion

    #region 事件
    public void AddListenerMessage(string actionName, Action<EventArg> fun)
    {
        if (ScriptManage.Instance.messageDic.ContainsKey(actionName) == true)
        {
            ScriptManage.Instance.messageDic[actionName].funlist.Add(fun);
        }
        else
        {
            EventObj a = new EventObj();
            a.funlist.Add(fun);
            ScriptManage.Instance.messageDic.Add(actionName, a);
        }
    }
    public void RemoveListenerMessage(string actionName, Action<EventArg> fun = null)
    {
        if (ScriptManage.Instance.messageDic.ContainsKey(actionName))
        {
            if (fun == null)
            {
                ScriptManage.Instance.messageDic.Remove(actionName);
            }
            else
            {
                ScriptManage.Instance.messageDic[actionName].funlist.Remove(fun);
                if (ScriptManage.Instance.messageDic[actionName].funlist.Count <= 0)
                {
                    ScriptManage.Instance.messageDic.Remove(actionName);
                }
            }
        }
    }
    public void sendMessage(string actionName, EventArg arg = null)
    {

        if (ScriptManage.Instance.messageDic.ContainsKey(actionName))
        {
            EventObj a = ScriptManage.Instance.messageDic[actionName];
            for (int i = 0; i < a.funlist.Count; i++)
            {
                a.funlist[i](arg);
            }
        }
        else
        {
            Debug.LogWarning("没有监听：" + actionName);
        }
    }
    #endregion

    #region 3d向量角度方向计算

    public Vector3 zore = new Vector3();

    /// <summary>
    /// 角度转弧度 默认角度转弧度
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="isAngle"></param>
    /// <returns></returns>
    public float radian(float angle, bool isAngle = true)
    {

        if (isAngle == true)
        {
            return angle * (Mathf.PI / 180); //计算出弧度
        }
        else
        {
            return angle * (180 / Mathf.PI); //计算出角度
        }
    }

    /// <summary>
    /// 通过xz计算出y的欧拉角
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float dirToAngleY(float x, float z)
    {
        // 忽略 Y 分量，只考虑 XZ 平面
        Vector3 dirXZ = new Vector3(x, 0, z);

        // 避免零向量
        if (dirXZ.sqrMagnitude < 0.001f)
            return 0f;

        // 计算相对于世界正 Z 轴 (0, 0, 1) 的角度
        // Atan2(x, z) 是因为：在XZ平面上，角度是从Z轴正方向开始，逆时针旋转
        float angleRad = Mathf.Atan2(dirXZ.x, dirXZ.z);
        float angleDeg = angleRad * Mathf.Rad2Deg;

        return angleDeg;
    }

    /// <summary>
    /// 通过yz方向计算出x的旋转角度
    /// </summary>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float dirToAngleX(float x, float y, float z)
    {
        float angle = 0;
        z = Mathf.Sqrt(x * x + z * z);

        if (y == 0 || z == 0)
        {
            if (y == 0)
            {
                if (z < 0)
                {
                    angle = 180;
                }
                else
                {
                    angle = 0;
                }
            }
            if (z == 0)
            {
                if (y < 0)
                {
                    angle = 90;
                }
                else
                {
                    angle = 270;
                }
            }
            if (y == 0 && z == 0)
            {
                angle = 0;
            }
        }
        else
        {

            angle = Mathf.Atan((Mathf.Abs(y) / Mathf.Abs(z)));
            angle = this.radian(angle, false);

            if (z > 0)
            {
                if (y < 0)
                {
                    //angle = angle;
                }
                else
                {
                    angle = 360 - angle;
                }
            }
            else
            {
                if (y < 0)
                {
                    angle = 180 - angle;
                }
                else
                {
                    angle = 180 + angle;
                }
            }
        }
        return angle;

    }

    /// <summary>
    /// 通过方向计算出其欧拉角
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public Vector3 dirToAngle(Vector3 dir)
    {
        Vector3 eulerAngles = Vector3.zero;
        // 确保方向向量已归一化
        if (dir != Vector3.zero)
        {
            // 创建旋转四元数（第二个参数通常是世界空间的向上方向）
            Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up);

            // 转换为欧拉角
            eulerAngles = rotation.eulerAngles;
        }
        return eulerAngles;
    }

    /// <summary>
    /// 计算出看向某个方向的欧拉角
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public Vector3 look_angle(Vector3 dir, Vector3 up)
    {
        Quaternion rotation = Quaternion.LookRotation(dir, up);
        Vector3 eulerAngles = rotation.eulerAngles;
        return rotation.eulerAngles;
    }

    /// <summary>
    /// 通过角度算出向量
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public float[] AngleToDir(float angle)
    {
        float aR = radian(angle);

        float x = (float)Math.Sin(aR);
        float y = (float)Math.Cos(aR);
        float[] r = { x, y };
        return r;
    }

    /// <summary>
    /// 返回两个点的距离的平方
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <returns></returns>
    public float DistanceSquare(Vector3 pos, Vector3 pos1)
    {
        float dic = Mathf.Pow(pos.x - pos1.x, 2) + Mathf.Pow(pos.y - pos1.y, 2) + Mathf.Pow(pos.z - pos1.z, 2);
        return dic;
    }

    /// <summary>
    /// 判断两个点是否在dic距离内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public bool isDic(Vector3 pos, Vector3 pos1, float dic)
    {
        float s = DistanceSquare(pos, pos1);
        if (s <= Mathf.Pow(dic, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 判断两个点是否在dic距离内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public bool isDic(Vector2 pos, Vector2 pos1, float dic)
    {
        float s = DistanceSquare(pos, pos1);
        if (s <= Mathf.Pow(dic, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public float DistanceSquare(Vector2 pos, Vector2 pos1)
    {
        float dic = Mathf.Pow(pos.x - pos1.x, 2) + Mathf.Pow(pos.y - pos1.y, 2);
        return dic;
    }

    /// <summary>
    /// 一个点绕着center，以axis为轴旋转angle度
    /// </summary>
    /// <param name="position"></param>
    /// <param name="center"></param>
    /// <param name="axis"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
        Vector3 resultVec3 = center + point;
        return resultVec3;
    }

    /// <summary>
    /// 一个向量绕着另外的向量旋转
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="zhou"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public Vector3 RotateDir(Vector3 dir, Vector3 zhou, float angle)
    {
        Vector3 rotatedVector;
        rotatedVector.x = dir.x;
        rotatedVector.y = dir.y;
        rotatedVector.z = dir.z;
        if (zhou != Vector3.zero && angle != 0)
        {
            // 确保旋转轴是单位向量
            Vector3 nor_zhou = zhou.normalized;

            // 创建旋转四元数
            Quaternion rotation = Quaternion.AngleAxis(angle, nor_zhou);

            // 应用旋转到向量
            rotatedVector = rotation * rotatedVector;
        }
        return rotatedVector;
    }
    /// <summary>
    /// 限制一个向量在另外一个向量角度范围内
    /// </summary>
    /// <param name="a">改变的向量</param>
    /// <param name="b">固定的向量</param>
    /// <param name="angle">限制的角度</param>
    /// <returns></returns>
    public Vector3 dir_limit(Vector3 a, Vector3 b, float angle)
    {
        a.Normalize();
        b.Normalize();
        float dotProduct = Vector3.Dot(a, b);
        float angleInRadians = Mathf.Acos(dotProduct);
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
        if (angleInDegrees > angle)
        {
            Vector3 rotationAxis = Vector3.Cross(a, b);
            float rotationAngle = angleInDegrees - angle;
            Quaternion rotationQuaternion = Quaternion.AngleAxis(rotationAngle, rotationAxis);
            a = rotationQuaternion * a;
        }
        return a;
    }

    /// <summary>
    /// 贝塞尔曲线
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 curve2(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 aa = Mathf.Pow((1 - t), 2) * a;
        Vector3 bb = 2 * t * (1 - t) * b;
        Vector3 cc = t * t * c;

        return aa + bb + cc;

        //Vector3 aa = a + (b - a) * t;
        //Vector3 bb = b + (c - b) * t;
        //return aa + (bb - aa) * t;
    }

    /// <summary>
    /// 贝塞尔曲线
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector2 curve2(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        Vector2 aa = Mathf.Pow((1 - t), 2) * a;
        Vector2 bb = 2 * t * (1 - t) * b;
        Vector2 cc = t * t * c;

        return aa + bb + cc;
        //Vector2 aa = a + (b - a) * t;
        //Vector2 bb = b + (c - b) * t;
        //return aa + (bb - aa) * t;
    }

    /// <summary>
    /// 将角度转变成区间(-180~180)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public float angleToNor(float v)
    {
        float x = (v + 360000) % 360;
        if (x > 180)
        {
            x = x - 360;
        }
        return x;
    }

    /// <summary>
    /// 计算一点投影到一条直线上
    /// </summary>
    /// <param name="linePoint1">直线上的点1</param>
    /// <param name="linePoint2">直线上的点2</param>
    /// <param name="pointToProject">要投影的点</param>
    /// <returns></returns>
    public Vector3 project_pos(Vector3 linePoint1, Vector3 linePoint2, Vector3 pointToProject)
    {
        // 计算直线的方向向量
        Vector3 lineDirection = (linePoint2 - linePoint1).normalized;

        // 计算直线上的任意点
        Vector3 pointOnLine = linePoint1 + Vector3.Dot(pointToProject - linePoint1, lineDirection) * lineDirection;
        return pointOnLine;
    }
    /// <summary>
    /// 计算一个向量x投影到一个向量y上的长度
    /// </summary>
    /// <param name="x">向量x</param>
    /// <param name="y">向量y</param>
    /// <returns></returns>
    public float project_dic(Vector3 x, Vector3 y)
    {
        //计算向量x在向量y上的投影长度
        float projectionLength = Vector3.Dot(x, y) / y.magnitude;
        return projectionLength;
    }

    /// <summary>
    /// 计算一点投影到一条直线上
    /// </summary>
    /// <param name="linePoint1">直线上的点1</param>
    /// <param name="linePoint2">直线上的点2</param>
    /// <param name="pointToProject">要投影的点</param>
    /// <returns></returns>
    public Vector2 project_pos(Vector2 linePoint1, Vector2 linePoint2, Vector2 pointToProject)
    {
        // 计算直线的方向向量
        Vector2 lineDirection = (linePoint2 - linePoint1).normalized;

        // 计算直线上的任意点
        Vector2 pointOnLine = linePoint1 + Vector2.Dot(pointToProject - linePoint1, lineDirection) * lineDirection;
        return pointOnLine;
    }

    /// <summary>
    /// 将角度转变成区间(-180~180)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Vector3 angleToNor(Vector3 v)
    {
        v.x = angleToNor(v.x);
        v.y = angleToNor(v.y);
        v.z = angleToNor(v.z);
        return v;
    }
    /// <summary>
    /// 将角度转变成区间(-180~180)
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public Vector2 angleToNor(Vector2 v)
    {
        v.x = angleToNor(v.x);
        v.y = angleToNor(v.y);
        return v;
    }
    /// <summary>
    /// 世界角度转本地角度
    /// </summary>
    /// <param name="tran">本地物体</param>
    /// <param name="worldRo">世界角度</param>
    public Vector3 angle_world_to_local(Transform tran, Vector3 worldRo)
    {
        Quaternion worldRotation = Quaternion.Euler(worldRo);

        // 计算相对于父物体的本地旋转
        Quaternion localRotation = Quaternion.Inverse(tran.parent.rotation) * worldRotation;

        // 转换回欧拉角
        Vector3 localEuler = localRotation.eulerAngles;
        return localEuler;
    }
    public Vector3 lerpAngle(Vector3 ori, Vector3 end, float bili)
    {
        Vector3 v;
        v.x = lerpAngle(ori.x, end.x, bili);
        v.y = lerpAngle(ori.y, end.y, bili);
        v.z = lerpAngle(ori.z, end.z, bili);
        return v;
    }

    /// <summary>
    /// 缓动角度
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="bili"></param>
    /// <returns></returns>
    public float lerpAngle(float a, float b, float bili)
    {
        angleFreeRo(ref a, ref b);
        return Mathf.Lerp(a, b, bili);
    }
    /// <summary>
    /// 将角度转换成最近角度
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="end"></param>
    public void angleFreeRo(ref float ori, ref float end)
    {
        ori += 360000;
        ori = ori % 360;
        end += 360000;
        end = end % 360;
        float off = end - ori;
        if (off > 180)
        {
            ori += 360;
        }
        if (off < -180)
        {
            ori -= 360;
        }
    }
    public void angleFreeRo(Vector3 v, Vector3 v1)
    {
        Tools.Instance.angleFreeRo(ref v.x, ref v1.x);
        Tools.Instance.angleFreeRo(ref v.y, ref v1.y);
        Tools.Instance.angleFreeRo(ref v.z, ref v1.z);
    }
    public void angleFreeRo(Vector2 v, Vector2 v1)
    {
        Tools.Instance.angleFreeRo(ref v.x, ref v1.x);
        Tools.Instance.angleFreeRo(ref v.y, ref v1.y);
    }
    public Vector3 lerpAngleQuaternion(Vector3 ori, Vector3 end, float lerp)
    {
        Quaternion q = Quaternion.Lerp(Quaternion.Euler(ori), Quaternion.Euler(end), lerp);
        return angleToNor(q.eulerAngles);
    }
    /// <summary>
    /// z轴缓动看向某个点
    /// </summary>
    /// <param name="tran"></param>
    /// <param name="lookPos"></param>
    /// <param name="lerpV"></param>
    public void look_target(Transform tran, Vector3 lookPos, float lerpV = 0.1f)
    {
        Vector3 direction = lookPos - tran.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 使用Lerp平滑旋转
        tran.rotation = Quaternion.Lerp(tran.rotation, targetRotation, lerpV);

    }
    /// <summary>
    /// z轴缓动看向某个方向
    /// </summary>
    /// <param name="eulerAngles">物体的欧拉角</param>
    /// <param name="lookDir">方向</param>
    /// <param name="lerpV"></param>
    /// <returns></returns>
    public Vector3 look_target(Vector3 eulerAngles, Vector3 lookDir, float lerpV = 0.1f)
    {
        eulerAngles = Tools.Instance.angleToNor(eulerAngles);
        Quaternion rotation = Quaternion.Euler(eulerAngles); ;
        Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        // 使用Lerp平滑旋转
        rotation = Quaternion.Lerp(rotation, targetRotation, lerpV);
        return rotation.eulerAngles;
    }
    /// <summary>
    /// z轴缓动看向某个方向
    /// </summary>
    /// <param name="rotation"></param>
    /// <param name="lookDir"></param>
    /// <param name="lerpV"></param>
    /// <returns></returns>
    public Quaternion look_target(Quaternion rotation, Vector3 lookDir, float lerpV = 0.1f)
    {
        Quaternion targetRotation = Quaternion.LookRotation(lookDir);
        // 使用Lerp平滑旋转
        rotation = Quaternion.Lerp(rotation, targetRotation, lerpV);
        return rotation;
    }

    /// <summary>
    /// 优化射线,多线程射线
    /// </summary>
    /// <param name="okfun">射线回调</param>
    /// <param name="ori">开始点</param>
    /// <param name="dir">方向</param>
    /// <param name="dic">距离</param>
    /// <param name="mask">层级遮罩</param>
    /// <param name="hitNum">最多射到多少碰撞框</param>
    public void RaycasExample(Action<NativeArray<RaycastHit>> okfun, List<Ray> ray, float dic, int mask = -5, int hitNum = 1)
    {
        // Perform a single raycast using RaycastCommand and wait for it to complete
        // Setup the command and result buffers
        var results = new NativeArray<RaycastHit>(ray.Count, Allocator.TempJob);

        var commands = new NativeArray<RaycastCommand>(ray.Count, Allocator.TempJob);

        for (int i = 0; i < ray.Count; i++)
        {
            if (isDebug == true)
            {
                Debug.DrawLine(ray[i].origin, ray[i].origin + ray[i].direction * dic, Color.red, 0.2f);
            }
            commands[i] = new RaycastCommand(ray[i].origin, ray[i].direction, dic, mask, hitNum);
        }

        // Schedule the batch of raycasts
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, ray.Count, default(JobHandle));

        // 等待射线完成
        handle.Complete();
        //results.CopyTo()
        // 获得结果,如果空没有命中
        okfun(results);
        //results[0].distance 小于等于0就是没有碰撞到物体

        // 清除射线
        results.Dispose();
        commands.Dispose();
    }
    #endregion

    #region 2d向量计算

    /// <summary>
    /// 一点绕着另外的点旋转
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="ori"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public float[] rotatePos(Vector2 pos, Vector2 ori, float angle)
    {
        float ra = this.radian(-angle);
        float x = (pos.x - ori.x) * Mathf.Cos(ra) - (pos.y - ori.y) * Mathf.Sin(ra) + ori.x;
        float y = (pos.x - ori.x) * Mathf.Sin(ra) + (pos.y - ori.y) * Mathf.Cos(ra) + ori.y;
        float[] r = { x, y };
        return r;
    }

    private Vector2 huan1 = new Vector2();

    /// <summary>
    /// 计算方向夹角 以two为基准的角度，0~360
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns></returns>
    public float DirAngle(Vector2 one, Vector2 two)
    {
        //let right = new Laya.Vector2(-one.y, one.x);
        Vector2 vv;
        vv.x = -one.y;
        vv.y = one.x;
        float angle = Vector2.Angle(one, two);
        float angleDir = Vector2.Dot(vv, two);
        if (0 < angleDir)
        {
            angle = 360 - angle;
        }
        return 360 - angle;
    }

    /// <summary>
    /// 判断两个点是否在dic距离内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public bool isDic2d(Vector2 pos, Vector2 pos1, float dic)
    {
        float s = Mathf.Pow(pos.x - pos1.x, 2) + Mathf.Pow(pos.y - pos1.y, 2);
        if (s <= Mathf.Pow(dic, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public float Dic(float p1x, float p1y, float p2x, float p2y)
    {
        float s = Mathf.Sqrt(Mathf.Pow(p1x - p2x, 2) + Mathf.Pow(p1y - p2y, 2));
        return s;
    }
    public float DicSqrt(float p1x, float p1y, float p2x, float p2y)
    {
        float s = Mathf.Pow(p1x - p2x, 2) + Mathf.Pow(p1y - p2y, 2);
        return s;
    }

    /// <summary>
    /// 返回两点平方
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <returns></returns>
    public float DicSqrt(Vector3 pos, Vector3 pos1)
    {
        float s = Mathf.Pow(pos.x - pos1.x, 2) + Mathf.Pow(pos.y - pos1.y, 2) + Mathf.Pow(pos.z - pos1.z, 2);
        return s;
    }
    /// <summary>
    /// 返回两点平方
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <returns></returns>
    public float DicSqrt(Vector2 pos, Vector2 pos1)
    {
        float s = Mathf.Pow(pos.x - pos1.x, 2) + Mathf.Pow(pos.y - pos1.y, 2);
        return s;
    }

    /// <summary>
    /// 判断两个点是否在dic距离内
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos1"></param>
    /// <param name="dic"></param>
    /// <returns></returns>
    public bool isDic2d(float p1x, float p1y, float p2x, float p2y, float dic)
    {
        float s = Mathf.Pow(p1x - p2x, 2) + Mathf.Pow(p1y - p2y, 2);
        if (s <= Mathf.Pow(dic, 2))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 将value赋值给change
    /// </summary>
    /// <param name="change"></param>
    /// <param name="value"></param>
    public void equalVec(ref Vector3 change, Vector3 value)
    {
        change.x = value.x;
        change.y = value.y;
        change.z = value.z;
    }
    /// <summary>
    /// 将value赋值给change
    /// </summary>
    /// <param name="change"></param>
    /// <param name="value"></param>
    public void equalVec(ref Vector2 change, Vector2 value)
    {
        change.x = value.x;
        change.y = value.y;
    }

    /// <summary>
    /// 世界坐标转屏幕
    /// </summary>
    /// <param name="c"></param>
    /// <param name="pos"></param>
    /// <param name="rect"></param>
    /// <returns></returns>
    public Vector3 worldToScreen(Camera c, Vector3 pos, RectTransform rect = null)
    {
        Vector3 v = c.WorldToScreenPoint(pos) / ViewManage.Instance.screem_rotia;
        if (rect != null)
        {
            rect.anchoredPosition = v;
        }
        return v;
    }
    #endregion

    #region ui坐标转换
    /// <summary>
    /// 将source的anchoredPosition坐标转化成target父空间下的坐标
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public Vector2 ConvertAnchoredPosition(RectTransform source, RectTransform target)
    {
        // 1. 获取源物体当前anchoredPosition对应的世界坐标
        Vector3 sourceWorldPosition = GetWorldPositionFromAnchoredPosition(source);

        // 2. 将世界坐标转换到目标物体的父级局部空间
        RectTransform targetParent = target.parent as RectTransform;
        Vector2 localPosInTargetParent = WorldToLocalPointInRectangle(targetParent, sourceWorldPosition);

        // 3. 计算目标物体锚点在其父级局部空间中的实际位置
        Vector2 targetAnchorReferencePoint = GetAnchorReferencePoint(target);

        // 4. 计算最终的anchoredPosition：局部坐标 - 锚点参考位置
        Vector2 convertedAnchoredPos = localPosInTargetParent - targetAnchorReferencePoint;

        return convertedAnchoredPos;
    }

    /// <summary>
    /// 根据源物体的anchoredPosition计算其世界坐标
    /// </summary>
    Vector3 GetWorldPositionFromAnchoredPosition(RectTransform rectTransform)
    {
        // 获取锚点在父级空间中的实际位置
        Vector2 anchorReference = GetAnchorReferencePoint(rectTransform);

        // 将anchoredPosition从锚点参考系转换到父级局部空间
        Vector2 localPositionInParent = anchorReference + rectTransform.anchoredPosition;

        // 将父级局部空间坐标转换到世界空间
        if (rectTransform.parent != null)
        {
            return rectTransform.parent.TransformPoint((Vector3)localPositionInParent);
        }

        return (Vector3)localPositionInParent; // 如果没有父级，直接返回
    }

    /// <summary>
    /// 计算RectTransform锚点在其父级局部空间中的参考位置
    /// </summary>
    Vector2 GetAnchorReferencePoint(RectTransform rectTransform)
    {
        if (rectTransform.parent == null)
            return Vector2.zero;

        Rect parentRect = ((RectTransform)rectTransform.parent).rect;

        // 计算锚点矩形的左下角和右上角在父级局部空间中的位置
        Vector2 anchorMinPos = new Vector2(
            parentRect.xMin + rectTransform.anchorMin.x * parentRect.width,
            parentRect.yMin + rectTransform.anchorMin.y * parentRect.height
        );

        Vector2 anchorMaxPos = new Vector2(
            parentRect.xMin + rectTransform.anchorMax.x * parentRect.width,
            parentRect.yMin + rectTransform.anchorMax.y * parentRect.height
        );

        // 返回锚点矩形的中心点（这是anchoredPosition的参考点）
        return (anchorMinPos + anchorMaxPos) * 0.5f;
    }

    /// <summary>
    /// 将世界坐标转换到RectTransform的局部空间
    /// </summary>
    Vector2 WorldToLocalPointInRectangle(RectTransform rectTransform, Vector3 worldPoint)
    {
        if (rectTransform == null)
            return (Vector2)worldPoint;

        // 使用矩阵变换进行精确转换
        Matrix4x4 worldToLocalMatrix = rectTransform.worldToLocalMatrix;
        return (Vector2)worldToLocalMatrix.MultiplyPoint(worldPoint);
    }
    /// <summary>
    /// 动态修改锚点
    /// </summary>
    /// <param name="newPivot"></param>
    /// <param name="m_RectTransform"></param>
    public void set_pivot(Vector2 newPivot, RectTransform m_RectTransform)
    {
        // 1. 获取当前的Pivot和Rect大小
        Vector2 oldPivot = m_RectTransform.pivot;
        Vector2 rectSize = m_RectTransform.rect.size;

        // 2. 计算Pivot变化导致的偏移量（在自身局部空间下的偏移）
        // 偏移量 = (新Pivot - 旧Pivot) * Rect的大小
        Vector2 pivotDelta = newPivot - oldPivot;
        Vector2 sizeScaledPivotDelta = new Vector2(pivotDelta.x * rectSize.x, pivotDelta.y * rectSize.y);

        // 3. 考虑RectTransform的缩放，将局部偏移转换为实际的缩放后偏移
        Vector3 scaledPivotDelta = new Vector3(sizeScaledPivotDelta.x * m_RectTransform.localScale.x, sizeScaledPivotDelta.y * m_RectTransform.localScale.y, 0);

        // 4. 修改Pivot（这会立刻导致物体视觉上的移动）
        m_RectTransform.pivot = newPivot;

        // 5. 【关键】反向移动localPosition，抵消掉因Pivot变化产生的偏移
        // 将计算出的偏移量，从局部空间转换到父对象的空间（考虑自身的旋转）
        Vector3 parentSpaceDelta = m_RectTransform.localRotation * scaledPivotDelta;
        m_RectTransform.localPosition += parentSpaceDelta;
    }
    #endregion

    #region ui监听事件

    public float ModuleY = 810f; //810f;

    public float ModuleX = 720f;

    /// <summary>
    /// 坐标自适应
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="IsZhen">true为鼠标坐标变ui坐标</param>
    /// <returns></returns>
    public Vector3 AdaptPos(Vector3 pos, bool IsZhen = true)
    {

        float bili = 720f / Screen.width;
        float U = (float)Screen.width / (float)Screen.height - 720f / 1080f;
        if (U > 0)
        {
            bili = 1080f / Screen.height;
        }

        if (IsZhen == true)
        {
            return new Vector3(pos.x * bili, pos.y * bili, 0);
        }
        else
        {

            return new Vector3(pos.x / bili, pos.y / bili, 0);
        }

    }


    /// <summary>
    /// 移除所有的事件
    /// </summary>
    /// <param name="go"></param>
    public void removeAllOnEvent(GameObject g)
    {
        EventBase evn = g.GetComponent<EventBase>();
        if (evn != null)
        {
            Component.Destroy(evn);
        }
        Button btn = g.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
        }
    }


    /// <summary>
    /// 按下
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool OnUiDown(GameObject go, Action<PointerEventData> tr, bool isChangeAlpha = true)
    {
        bool isOk = true;
        if (go != null)
        {
            EventBase evn;
            if (go.GetComponent<EventBase>() == null)
            {
                evn = go.AddComponent<EventBase>();
            }
            else
            {
                evn = go.GetComponent<EventBase>();
            }
            evn.CallStartPressFun = null;
            evn.CallStartPressFun += tr;
            if (isChangeAlpha == true)
            {
                Image cc = go.GetComponent<Image>();
                if (cc != null && cc.color.a > 0)
                {
                    evn.CallStartPressFun += (aa) =>
                    {
                        Color color = cc.color;
                        color.a = 0.5f + color.a * 0.1f;
                        cc.color = color;
                    };
                }
            }
        }
        else
        {
            isOk = false;
        }
        return isOk;
    }
    /// <summary>
    /// 拖动中
    /// </summary>
    /// <param name="go"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public bool OnUiDrag(GameObject go, Action<PointerEventData> callback)
    {
        bool isOk = true;
        if (go != null)
        {
            EventBase evn;
            if (go.GetComponent<EventBase>() == null)
            {
                evn = go.AddComponent<EventBase>();
            }
            else
            {
                evn = go.GetComponent<EventBase>();
            }
            evn.CallFunDrag = callback;
        }
        else
        {
            Debug.LogError("拖动的obj为Null");
            isOk = false;
        }
        return isOk;
    }
    /// <summary>
    /// 抬起
    /// </summary>
    /// <param name="go"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public bool OnUiUp(GameObject go, Action<PointerEventData> tr, bool isChangeAlpha = true)
    {
        bool isOk = true;
        if (go != null)
        {
            EventBase evn;
            if (go.GetComponent<EventBase>() == null)
            {
                evn = go.AddComponent<EventBase>();
            }
            else
            {
                evn = go.GetComponent<EventBase>();
            }
            evn.RaiseTheCallback = null;
            evn.RaiseTheCallback += tr;
            if (isChangeAlpha == true)
            {
                Image cc = go.GetComponent<Image>();
                if (cc != null && cc.color.a > 0)
                {
                    evn.RaiseTheCallback += (aa) =>
                    {
                        Color color = cc.color;
                        color.a = (color.a - 0.5f) * 10;
                        cc.color = color;
                    };
                }
            }
        }
        else
        {
            isOk = false;
        }
        return isOk;
    }

    AudioItem _btnM;
    AudioItem btnM
    {
        get
        {
            if (_btnM == null)
            {
                _btnM = MusicManage.Instance.play("btn", false, false);
            }
            if (_btnM != null && _btnM.isLost == true)
            {
                _btnM = MusicManage.Instance.play("btn", false, false);
            }
            return _btnM;
        }
    }

    /// <summary>
    /// 添加按钮事件
    /// </summary>
    /// <param name="g"></param>
    /// <param name="fun"></param>
    public void OnButton(GameObject g, UnityAction fun, bool isNoListener = true, bool isOne = false, bool isMusic = true)
    {
        Button btn = g.GetComponent<Button>();
        if (btn != null)
        {
            if (isNoListener == true)
            {
                btn.onClick.RemoveAllListeners();
            }

            btn.onClick.AddListener(() =>
            {
                if (fun != null)
                {
                    fun();
                    if (isOne == true)
                    {
                        btn.onClick.RemoveAllListeners();
                    }
                }
                if (isMusic == true && MusicManage.Instance != null)
                {
                    if (btnM != null)
                    {
                        btnM.value = MusicManage.Instance.audioValue;
                        btnM.play();
                    }
                }
            });
        }
        else
        {
            Debug.Log("物体：(" + g.name + ")没有Button组件");
        }
    }

    /// <summary>
    /// 添加按钮事件
    /// </summary>
    /// <param name="g"></param>
    /// <param name="fun"></param>
    public void OnButton(GameObject g, UnityAction<GameObject> fun, bool isNoListener = true, bool isOne = false, bool isMusic = true)
    {
        Button btn = g.GetComponent<Button>();
        if (btn != null)
        {
            if (isNoListener == true)
            {
                btn.onClick.RemoveAllListeners();
            }

            btn.onClick.AddListener(() =>
            {
                if (fun != null)
                {
                    fun(g);
                    if (isOne == true)
                    {
                        btn.onClick.RemoveAllListeners();
                    }
                }
                if (isMusic == true && MusicManage.Instance != null)
                {
                    if (btnM != null)
                    {
                        btnM.value = MusicManage.Instance.audioValue;
                        btnM.play();
                    }
                }
            });
        }
        else
        {
            Debug.Log("物体：(" + g.name + ")没有Button组件");
        }
    }

    public List<TouchDataUi> TouchInfo = new List<TouchDataUi>();

    IEnumerator wxBtn;
    public void OnUiButton(GameObject g, UnityAction fun)
    {
        Button btn = g.GetComponent<Button>();
        if (btn != null)
        {
            OnButton(g, fun);
        }
        else
        {
            Vector2 v = new Vector2();
            bool isReSetdown = false;
            //按下
            OnUiDown(g, (PointerEventData evn) =>
            {
                v = evn.position;
                isReSetdown = true;
            });
            //移动
            OnUiDrag(g, (PointerEventData evn) =>
            {
                //Debug.Log("点击::::::::::::移动:"+ isDic2d(evn.position, v, 5));
                if (isDic2d(evn.position, v, 5) == false)
                {
                    isReSetdown = false;
                }
            });
            //抬起
            OnUiUp(g, (PointerEventData evn) =>
            {
                if (isReSetdown == true)
                {
                    if (fun != null)
                    {
                        fun();
                    }
                }
                isReSetdown = false;
            });
        }

    }
    #endregion

    #region ui多点触屏问题
    public Dictionary<int, Touch_info_ui> touch_list = new Dictionary<int, Touch_info_ui>();
    #endregion

    #region ui动画
    GameObject _viewAnimPre = null;
    GameObject viewAnimPre
    {
        get
        {
            if (_viewAnimPre == null)
            {
                _viewAnimPre = GetOneByName("viewAnim", WebSdk.self.gameObject);
            }
            return _viewAnimPre;
        }
        set { _viewAnimPre = value; }
    }
    /// <summary>
    /// 显示动画(tweenPos:Pos_up(上),Pos_down(下),Pos_left(左),Pos_right(右))
    /// </summary>
    /// <param name="view">当前的view</param>
    /// <param name="ty">类型</param>
    /// <param name="fun">回调</param>
    /// <param name="isOne">是否只有当前view有显示动画</param>
    /// <param name="copyG">自定义tween</param>
    public void viewAnim(GameObject view, ViewAnimTy ty = ViewAnimTy.TweenScale, Action fun = null, bool isOne = true, GameObject copyG = null)
    {
        if (isOne == true)
        {
            switch (ty)
            {
                case ViewAnimTy.TweenPos:
                    tp(view, fun, -Screen.width, 0, copyG);
                    break;
                case ViewAnimTy.TweenScale:
                    ts(view, fun, copyG);
                    break;
                case ViewAnimTy.TweenColor:
                    tc(view, fun, copyG);
                    break;
                case ViewAnimTy.TweenRo:
                    tr(view, fun, copyG);
                    break;
                default:
                    break;
            }
        }
        else
        {
            string selectStr = "";
            switch (ty)
            {
                case ViewAnimTy.TweenPos:
                    selectStr = "Pos_";
                    break;
                case ViewAnimTy.TweenScale:
                    selectStr = "Scale_";
                    break;
                case ViewAnimTy.TweenColor:
                    selectStr = "Color_";
                    break;
                case ViewAnimTy.TweenRo:
                    selectStr = "Ro_";
                    break;
                default:
                    break;
            }
            List<GameObject> uiList = GetSelectAllObj(view, true, selectStr);
            for (int i = 0; i < uiList.Count; i++)
            {
                GameObject ui = uiList[i];
                //Debug.Log("名字:::" + ui.gameObject.name+"  zzz:"+selectStr);
                switch (ty)
                {
                    case ViewAnimTy.TweenPos:
                        float x = 0;
                        float y = 0;
                        string uis = ui.name.Split('_')[1];
                        if (uis == "up")
                        {
                            y = Screen.height;
                        }
                        else if (uis == "down")
                        {
                            y = -Screen.height;
                        }
                        else if (uis == "right")
                        {
                            x = Screen.width;
                        }
                        else if (uis == "left")
                        {
                            x = -Screen.width;
                        }
                        else
                        {
                            string[] uiss = uis.Split(':');
                            if (uiss[0] == "x")
                            {
                                x = float.Parse(uiss[1]);
                            }
                            else
                            {
                                y = float.Parse(uiss[1]);
                            }
                        }
                        tp(ui, fun, x, y, copyG);
                        break;
                    case ViewAnimTy.TweenScale:
                        ts(ui, fun, copyG);
                        break;
                    case ViewAnimTy.TweenColor:
                        tc(ui, fun, copyG);
                        break;
                    case ViewAnimTy.TweenRo:
                        tr(ui, fun, copyG);
                        break;
                    default:
                        break;
                }
            }
        }

    }

    /// <summary>
    /// 渐变特效 (如果name是BG的时候不渐变)
    /// </summary>
    /// <param name="view"></param>
    /// <param name="fun"></param>
    /// <param name="speed"></param>
    /// <param name="ty"></param>
    public void viewColorEff(GameObject view, Action fun = null, float speed = 1, int ty = 0)
    {
        if (mono != null)
        {
            List<GameObject> uiList = GetSelectAllObj(view, true);
            List<GameObject> uiListYes = new List<GameObject>();
            List<float> uiA = new List<float>();
            for (int i = 0; i < uiList.Count; i++)
            {
                Text t = uiList[i].GetComponent<Text>();
                Image img = uiList[i].GetComponent<Image>();
                if (uiList[i].name.Split("_")[0] != "BG")
                {
                    bool isAdd = false;
                    float a = 1;
                    if (t != null)
                    {
                        a = t.color.a;
                        isAdd = true;
                        Color c = t.color;
                        c.a = 0;
                        t.color = c;
                    }
                    if (img != null)
                    {
                        a = img.color.a;
                        isAdd = true;
                        Color c = img.color;
                        c.a = 0;
                        img.color = c;
                    }
                    if (isAdd == true)
                    {
                        uiListYes.Add(uiList[i]);
                        uiA.Add(a);
                    }
                }
            }
            mono.StartCoroutine(_viewColorEff(uiListYes, uiA, fun, speed, ty));
        }

    }
    IEnumerator _viewColorEff(List<GameObject> glist, List<float> aList, Action fun, float speed, int ty)
    {
        bool isok = false;
        float tt = 0;
        Vector2 ori = new Vector2(0, Screen.height);
        float aOff = 500;
        float yesDic = 0;
        while (isok == false)
        {
            tt += 0.03f * speed;
            if (ty == 0)
            {
                yesDic = tt * 2000;
            }
            else
            {
                yesDic = tt;
            }

            for (int i = 0; i < glist.Count; i++)
            {
                GameObject g = glist[i];
                if (g == null)
                {
                    isok = true;
                }

                Text t = g.GetComponent<Text>();
                Image img = g.GetComponent<Image>();

                if (t != null)
                {
                    Vector3 pos = g.transform.position;
                    float dic = 0;
                    if (ty == 0)
                    {
                        dic = Mathf.Sqrt(Mathf.Pow(ori.x - pos.x, 2) + Mathf.Pow(ori.y - pos.y, 2));
                    }

                    Color c = t.color;
                    float ss = yesDic - dic;
                    if (ss < 0)
                    {
                        ss = 0;
                    }
                    float a = ss / aOff;
                    if (a > 1)
                    {
                        a = 1;
                    }
                    c.a = a * aList[i];
                    t.color = c;
                }
                if (img != null)
                {
                    Vector3 pos = g.transform.position;
                    float dic = 0;
                    if (ty == 0)
                    {
                        dic = Mathf.Sqrt(Mathf.Pow(ori.x - pos.x, 2) + Mathf.Pow(ori.y - pos.y, 2));
                    }

                    Color c = img.color;
                    float ss = yesDic - dic;
                    if (ss < 0)
                    {
                        ss = 0;
                    }
                    float a = ss / aOff;
                    if (a > 1)
                    {
                        a = 1;
                    }
                    c.a = a * aList[i];
                    img.color = c;
                }
                if (ty == 0)
                {
                    if (yesDic > 3000)
                    {
                        isok = true;
                    }
                }
                else
                {
                    if (yesDic > 1)
                    {
                        isok = true;
                    }
                }
            }
            if (isok == true)
            {
                for (int i = 0; i < glist.Count; i++)
                {
                    GameObject g = glist[i];
                    if (g != null)
                    {
                        Text t = g.GetComponent<Text>();
                        Image img = g.GetComponent<Image>();

                        if (t != null)
                        {
                            Color c = t.color;
                            c.a = aList[i];
                            t.color = c;
                        }
                        if (img != null)
                        {
                            Color c = img.color;
                            c.a = aList[i];
                            img.color = c;
                        }
                    }
                }
                if (fun != null)
                {
                    fun();
                }
                yield break;
            }
            else
            {
                yield return null;
            }

        }
    }

    #region
    void ts(GameObject ui, Action fun, GameObject copyG = null)
    {

        TweenScale tp = ui.GetComponent<TweenScale>();
        if (tp == null)
        {
            tp = ui.AddComponent<TweenScale>();
            if (copyG == null)
            {
                copyG = viewAnimPre;
            }
            TweenScale copyTs = copyG.GetComponent<TweenScale>();
            if (copyTs != null)
            {
                tp.curve = copyTs.curve;
                tp.time = copyTs.time;
            }
            tp.isAwarkPlay = true;
        }
        else
        {
            tp.RePlay();
        }
        if (fun != null)
        {
            tp.Finish = (g, s) =>
            {
                fun();
            };
        }
    }
    void tp(GameObject ui, Action fun, float x, float y, GameObject copyG = null)
    {

        Vector3 oripos = ui.GetComponent<RectTransform>().localPosition;
        TweenPosition tp = ui.GetComponent<TweenPosition>();
        if (tp == null)
        {
            tp = ui.AddComponent<TweenPosition>();
            if (copyG == null)
            {
                copyG = viewAnimPre;
            }
            TweenPosition copyTs = copyG.GetComponent<TweenPosition>();
            if (copyTs != null)
            {
                tp.curve = copyTs.curve;
                tp.time = copyTs.time;
            }
            tp.isAwarkPlay = true;
            tp.form = new Vector3(oripos.x + x, oripos.y + y, oripos.z);
            tp.to = new Vector3(oripos.x, oripos.y, oripos.z);
        }
        else
        {
            tp.RePlay();
        }
        if (fun != null)
        {
            tp.Finish = (g, s) =>
            {
                fun();
            };
        }
    }
    void tr(GameObject ui, Action fun, GameObject copyG = null)
    {
        TweenRotato tp = ui.GetComponent<TweenRotato>();
        if (tp == null)
        {
            tp = ui.AddComponent<TweenRotato>();
            if (copyG == null)
            {
                copyG = viewAnimPre;
            }
            TweenRotato copyTs = copyG.GetComponent<TweenRotato>();
            if (copyTs != null)
            {
                tp.curve = copyTs.curve;
                tp.time = copyTs.time;
            }
            tp.isAwarkPlay = true;
        }
        else
        {
            tp.RePlay();
        }
        if (fun != null)
        {
            tp.Finish = (g, s) =>
            {
                fun();
            };
        }
    }
    void tc(GameObject ui, Action fun, GameObject copyG = null)
    {
        TweenColor tp = ui.GetComponent<TweenColor>();
        if (tp == null)
        {
            tp = ui.AddComponent<TweenColor>();
            if (copyG == null)
            {
                copyG = viewAnimPre;
            }
            TweenColor copyTs = copyG.GetComponent<TweenColor>();
            if (copyTs != null)
            {
                tp.curve = copyTs.curve;
                tp.time = copyTs.time;
            }
            tp.isAwarkPlay = true;
        }
        else
        {
            tp.RePlay();
        }
        if (fun != null)
        {
            tp.Finish = (g, s) =>
            {
                fun();
            };
        }
    }
    #endregion


    /// <summary>
    /// 标亮某个ui
    /// </summary>
    /// <param name="g">ui</param>
    /// <param name="tip">提示文本</param>
    /// <param name="offx">提示文本的偏移 x</param>
    /// <param name="offy">提示文本的偏移 y/param>
    /// <param name="isTip1">0提示文本向下 2提示文本向上 1整个背景提示 4手指指向ui</param>
    /// <param name="fun"></param>
    public void showSignUi(GameObject g, string tip = "", float offx = 0, float offy = 0, int isTip1 = 0, Action fun = null, bool isNowShow = false)
    {
        // The legacy View/Sign prefab is intentionally not part of the pure-game build.
        // Complete the optional callback without blocking gameplay on a removed overlay.
        if (SignView.self == null)
        {
            fun?.Invoke();
            return;
        }
        if (g == null)
        {
            return;
        }
        if (g.activeInHierarchy == false)
        {
            return;
        }
        if (is_can_sign == false)
        {
            return;
        }
        is_can_sign = false;

        LaterFun(() =>
        {
            is_can_sign = true;
        }, 0.1f);
        float off = 1;// Screen.height/ ViewManage.Instance.height ;
        //Debug.Log("偏移" + off);
        offx *= off;
        offy *= off;
        _showSignUi(g, tip, offx, offy, isTip1, fun, isNowShow);

    }

    IEnumerator sign_no;
    bool is_can_sign = true;
    void _showSignUi(GameObject g, string tip = "", float offx = 0, float offy = 0, int isTip1 = 0, Action fun = null, bool isNowShow = false)
    {

        Transform par = null;
        GameObject ttt = null;

        if (SignView.self != null)
        {
            ClearObj(SignView.self.m_par);
            TweenColor tc = SignView.self.GetComponent<TweenColor>();
            tc.RePlay();
        }
        SignView.ShowView(true, 99);
        SignView.self.m_par.SetActive(false);
        SignView.self.m_tip1.SetActive(false);

        float llt = 0.1f;
        if (isNowShow == true)
        {
            llt = 0.02f;
        }
        LaterFun(() =>
        {
            SignView.ShowView(true, 99);
            TweenColor tc = SignView.self.GetComponent<TweenColor>();
            tc.RePlay();
            SignView.fun = fun;
            SignView.self.m_par.transform.localPosition = new Vector3(0, 0, 0);
            SignView.self.setTip(g, tip, offx, offy, isTip1);
            SignView.self.m_par.SetActive(false);

            LaterFun(() =>
            {
                ClearObj(SignView.self.m_par);

                if (g != null)
                {
                    ttt = GameObject.Instantiate(g);
                    par = g.transform.parent;

                    ttt.transform.SetParent(SignView.self.m_par.transform, false);
                    ttt.transform.position = g.transform.position;

                    RectTransform tRect = ttt.GetComponent<RectTransform>();
                    RectTransform gRect = g.GetComponent<RectTransform>();
                    tRect.sizeDelta = gRect.sizeDelta;
                    Button yesBtn = g.GetComponent<Button>();
                    Button gbtn = ttt.GetComponent<Button>();

                    //SignView.self.setTipPos(ttt, offx, offy);
                    if (gbtn != null)
                    {
                        gbtn.onClick = yesBtn.onClick;

                        gbtn.onClick.AddListener(() =>
                        {
                            SignView.HideView();
                            if (isTip1 == 0 || isTip1 == 2)
                            {
                                if (SignView.fun != null)
                                {
                                    //Debug.Log("ssssssssssssssssssss333333333eee");
                                    SignView.fun();
                                    //SignView.fun = null;
                                }
                            }
                        });
                    }
                    else
                    {
                        OnUiDown(ttt, (ent) =>
                        {
                            GameObject.Destroy(ttt);
                            SignView.HideView();
                            if (isTip1 == 0 || isTip1 == 2)
                            {
                                if (SignView.fun != null)
                                {
                                    //Debug.Log("ssssssssssssssssss6666666666666eee");
                                    SignView.fun();
                                    //SignView.fun = null;
                                }
                            }
                        });
                    }
                }
                SignView.self.m_par.transform.localPosition = new Vector3(10000, 0, 0);
                SignView.self.m_par.SetActive(true);
                LaterFun(() =>
                {
                    SignView.self.m_par.transform.localPosition = new Vector3(0, 0, 0);
                }, llt);
            }, llt);
        }, llt);

        if (sign_no != null)
        {
            mono.StopCoroutine(sign_no);
        }
        sign_no = LaterFun(() =>
        {
            SignView.DestroyView();
        }, 20);
    }

    public void setUiPos(GameObject mmm, GameObject posG, float offx = 0, float offy = 0)
    {
        RectTransform gRect = posG.GetComponent<RectTransform>();
        Vector2 v = gRect.anchoredPosition;
        v.x += offx;
        v.y += offy;
        RectTransform mRect = mmm.GetComponent<RectTransform>();
        mRect.pivot = gRect.pivot;
        mRect.anchorMax = gRect.anchorMax;
        mRect.anchorMin = gRect.anchorMin;
        mRect.anchoredPosition = v;

    }

    /// <summary>
    /// 数值跳动效果
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="num">初始数字</param>
    /// <param name="num1">目标数字</param>
    /// <param name="t">跳的时间</param>
    /// <param name="f">保留小数</param>
    /// <param name="qian">前缀</param>
    public void shuzi_eff(Text text, float num, float num1, float t = 2, string f = "F0", string qian = "+")
    {
        mono.StartCoroutine(_shuzi(text, num, num1, t, f, qian));
    }

    IEnumerator _shuzi(Text text, float num, float num1, float t, string f, string qian)
    {

        bool isOk = false;
        float vv = num;
        text.text = vv + "";
        float offT = 0.05f;
        float off = (num1 - num) / (t / offT);
        float tt = 0;
        if (off < 0)
        {
            qian = "";
        }
        while (isOk == false)
        {
            if (text == null)
            {
                isOk = true;
                break;
            }
            else
            {
                vv += off;
                tt += offT;
                text.text = qian + vv.ToString(f);
                if (tt >= t)
                {
                    isOk = true;
                    text.text = qian + num1.ToString(f);

                }
                yield return new WaitForSeconds(offT);
            }
        }

    }
    #endregion

    #region 游戏工具
    public void setTime(float time)
    {
        if (time <= 0.5f)
        {
            TipView.DestroyView();
        }
        Time.timeScale = time;
    }

    /// <summary>
    /// 提示
    /// </summary>
    /// <param name="info">内容</param>
    /// <param name="t">自动关闭时间</param>
    /// <param name="ty">0提示不能点击，1全屏动画提示，2提示能点击</param>
    public void showTip(string info, float t = 1.5f, int ty = 0)
    {
        if (Time.timeScale > 0)
        {
            t = t * Time.timeScale;
            TipView.t = t;
            TipView.tip = info;
            TipView.ty = ty;
            TipView.ShowView(true, 200);
        }
    }

    /// <summary>
    /// 摄像机缓动看向物体
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="pos"></param>
    /// <param name="t"></param>
    /// <param name="fun"></param>
    public void cameraMove(GameObject camera, GameObject pos, float t = 2, Action<GameObject, string> fun = null)
    {
        _cameraMove(camera, pos.transform.eulerAngles, pos.transform.position, t, fun);
    }
    public void cameraMove(GameObject camera, Vector3 pos, Vector3 eulerAngles, float t = 2, Action<GameObject, string> fun = null)
    {
        _cameraMove(camera, eulerAngles, pos, t, fun);
    }
    void _cameraMove(GameObject camera, Vector3 eulerAngles, Vector3 pos, float t = 2, Action<GameObject, string> fun = null)
    {
        TweenPosition tp = camera.GetComponent<TweenPosition>();
        TweenRotato tr = camera.GetComponent<TweenRotato>();

        if (t <= 0)
        {
            if (tp != null)
            {
                tp.stop();
            }
            if (tr != null)
            {
                tr.stop();
            }
            camera.transform.position = pos;
            camera.transform.eulerAngles = eulerAngles;

        }
        else
        {

            if (tp == null)
            {
                tp = camera.AddComponent<TweenPosition>();
            }
            tp.form = camera.transform.position;
            tp.to = pos;

            tp.time = t;
            tp.isLocal = false;
            tp.RePlay();
            tp.Finish = fun;
            if (tr == null)
            {
                tr = camera.AddComponent<TweenRotato>();
            }

            tr.form = angleToNor(camera.transform.eulerAngles);
            camera.transform.eulerAngles = tr.form;
            tr.to = angleToNor(eulerAngles);
            tr.to.x = ct(tr.form.x, tr.to.x);
            tr.to.y = ct(tr.form.y, tr.to.y);
            tr.to.z = ct(tr.form.z, tr.to.z);
            tr.time = t;
            tr.isLocal = false;
            tr.RePlay();
        }
    }
    float ct(float ori, float end)
    {
        float off = ori - end;
        if (off < -180 || off > 180)
        {
            off = angleToNor(off);
            end = ori + off;
        }

        return end;
    }
    /// <summary>
    /// 找到最近的旋转角度
    /// </summary>
    /// <param name="ori"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public Vector3 angle_near_ro(Vector3 ori, Vector3 end)
    {
        Vector3 to;
        to.x = ct(ori.x, end.x);
        to.y = ct(ori.y, end.y);
        to.z = ct(ori.z, end.z);
        return to;
    }

    #endregion

}


/// <summary>
/// ui效果
/// </summary>
public enum ViewAnimTy
{
    TweenPos = 0,
    TweenScale = 1,
    TweenColor = 2,
    TweenRo = 3,
}

public class Touch_info_ui
{
    public void set_touch(Touch _touch)
    {
        touch = _touch;
        position = _touch.position;
        fingerId = _touch.fingerId;
        noNum = 0;
        state = Touch_info_ui_state.active;
        set_down();
        set_end();
    }
    public Touch_info_ui_state state;
    public Touch touch;
    public Vector2 position;
    public Vector2 down_position;
    public long down_time;
    public int fingerId = -1;
    public int down_fingerId = -1;
    public int noNum = 0;
    void set_down()
    {
        if (touch.phase == TouchPhase.Began)
        {
            down_position = touch.position;
            down_fingerId = touch.fingerId;
            down_time = Tools.Instance.timeToSeconds(DateTime.Now);
        }
    }
    void set_end()
    {
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            noNum = 1;
            state = Touch_info_ui_state.end;
        }
    }
}
public enum Touch_info_ui_state
{
    none = 0,
    active = 1,//激活
    end = 2,//抬起
}

public class ToggleUse
{
    Dictionary<string, GameObject> gg = new Dictionary<string, GameObject>();
    Action<GameObject> btnFun;
    GameObject par;
    string ori_n;
    public ToggleUse(Dictionary<string, GameObject> _gg, Action<GameObject> _btnFun, GameObject _par, string _ori_n)
    {
        gg = _gg;
        btnFun = _btnFun;
        par = _par;
        ori_n = _ori_n;
        foreach (var item in gg.Values)
        {
            Tools.Instance.OnButton(item, (btn) =>
            {
                _setToggle(btn);
            });
        }
    }

    void _setToggle(GameObject btn)
    {
        for (int i = 0; i < par.transform.childCount; i++)
        {
            GameObject g = par.transform.GetChild(i).gameObject;
            GameObject select = Tools.Instance.GetOneByName("select", g);
            if (select != null)
            {
                if (g == btn)
                {
                    select.SetActive(true);
                    if (btnFun != null)
                    {
                        btnFun(btn);
                    }
                }
                else
                {
                    select.SetActive(false);
                }

            }
        }
    }
    public void setToggle(string nn)
    {
        if (gg.ContainsKey(nn))
        {
            _setToggle(gg[nn]);
        }
    }
    public void setToggle(int index)
    {
        string nn = ori_n + index;
        setToggle(nn);
    }
    public void setToggle(GameObject btn)
    {
        _setToggle(btn);
    }
}
