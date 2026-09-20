using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
#if PT_wx
using WeChatWASM;
#endif

public class DataTools
{
    /// <summary>
    /// 默认保存类型  1本地,2网络
    /// </summary>
    static DataSaveTy saveTy = DataSaveTy.local;

    #region 数据处理
    static char sp1 = 'ě';
    static char sp2 = 'è';
    static char sp3 = 'é';
    //缓存数据
    static Dictionary<string, int> saveInt = new Dictionary<string, int>();
    static Dictionary<string, float> saveFloat = new Dictionary<string, float>();
    static Dictionary<string, string> saveStr = new Dictionary<string, string>();
    static Dictionary<string, string> saveList = new Dictionary<string, string>();
    static Dictionary<string, string> saveDic = new Dictionary<string, string>();
    static readonly HashSet<string> savedKeys = new HashSet<string>();
    static bool savedKeysLoaded;
    const string SavedKeysKey = "__DataToolsSavedKeys";

    /// <summary>
    /// 总数据
    /// </summary>
    public static Dictionary<string, string> netSave_data
    {
        get
        {
            return localData_to_netData();
        }
        set
        {
            netData_to_localData(value);
        }
    }
    //存数据到缓存
    static void netData_to_localData(Dictionary<string, string> vv)
    {
        foreach (string key in vv.Keys)
        {
            string value = vv[key];
            if (saveInt.ContainsKey(key) == true)
            {
                if (IsNumeric(value) == true)
                {
                    saveInt[key] = int.Parse(value);
                }
                else
                {
                    Debug.LogError("网络数据：key:" + key + "  value:" + value + "不是数字");
                }
            }
            if (saveFloat.ContainsKey(key) == true)
            {
                if (IsNumeric(value) == true)
                {
                    saveFloat[key] = float.Parse(value);
                }
                else
                {
                    Debug.LogError("网络数据：key:" + key + "  value:" + value + "不是数字");
                }
            }
            if (saveStr.ContainsKey(key) == true)
            {
                saveStr[key] = value;
            }
            if (saveList.ContainsKey(key) == true)
            {
                saveList[key] = value;
            }
            if (saveDic.ContainsKey(key) == true)
            {
                saveDic[key] = value;
            }
        }
    }
    static Dictionary<string, string> localData_to_netData()
    {
        Dictionary<string, string> vv = new Dictionary<string, string>();
        foreach (string key in saveInt.Keys)
        {
            string value = saveInt[key].ToString();
            vv.Add(key, value);
        }
        foreach (string key in saveFloat.Keys)
        {
            string value = saveFloat[key].ToString();
            vv.Add(key, value);
        }
        foreach (string key in saveStr.Keys)
        {
            string value = saveStr[key];
            vv.Add(key, value);
        }
        foreach (string key in saveList.Keys)
        {
            string value = saveList[key];
            vv.Add(key, value);
        }
        foreach (string key in saveDic.Keys)
        {
            string value = saveDic[key];
            vv.Add(key, value);
        }
        return vv;
    }
    static bool IsNumeric(string value)
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

    //从缓存取数据
    static int get_save(string key, int value, bool is_change_value = true)
    {
        int vv = 0;
        if (saveInt.ContainsKey(key) == true)
        {
            vv = saveInt[key];
            if (is_change_value == true)
            {
                saveInt[key] = value;
            }
        }
        else
        {
            if (is_change_value == true)
            {
                saveInt.Add(key, value);
            }
            else
            {
                init_save();
                vv = GetIntFun(key);
                saveInt.Add(key, vv);
            }
        }
        return vv;
    }
    static float get_save(string key, float value, bool is_change_value = true)
    {
        float vv = 0;
        if (saveFloat.ContainsKey(key) == true)
        {
            vv = saveFloat[key];
            if (is_change_value == true)
            {
                saveFloat[key] = value;
            }
        }
        else
        {
            if (is_change_value == true)
            {
                saveFloat.Add(key, value);
            }
            else
            {
                init_save();
                vv = GetFloatFun(key);
                saveFloat.Add(key, vv);
            }
        }
        return vv;
    }

    static string get_save(string key, string value, bool is_change_value = true)
    {
        string vv = "";
        if (saveStr.ContainsKey(key) == true)
        {
            vv = saveStr[key];
            if (is_change_value == true)
                saveStr[key] = value;
        }
        else
        {
            if (is_change_value == true)
                saveStr.Add(key, value);
            else
            {
                init_save();
                vv = GetStringFun(key);
                if (string.IsNullOrEmpty(vv) == true)
                {
                    vv = "";
                }
                saveStr.Add(key, vv);
            }

        }
        return vv;
    }

    static string list_to_string(List<string> _v)
    {
        string v = "";
        for (int i = 0; i < _v.Count; i++)
        {
            if (v == "")
            {
                v = _v[i];
            }
            else
            {
                v += sp1 + _v[i];
            }
        }
        return v;
    }

    static List<string> string_to_list(string v)
    {
        List<string> list = new List<string>();
        if (string.IsNullOrEmpty(v) == false)
        {
            string[] sss = v.Split(sp1);
            for (int i = 0; i < sss.Length; i++)
            {
                list.Add(sss[i]);
            }
        }
        return list;
    }

    static string get_save_list(string key, List<string> value, bool is_change_value = true)
    {
        string vv = "";
        if (saveList.ContainsKey(key) == true)
        {
            vv = saveList[key];
            if (is_change_value == true)
                saveList[key] = list_to_string(value);
        }
        else
        {
            if (is_change_value == true)
                saveList.Add(key, list_to_string(value));
            else
            {
                init_save();
                vv = GetStringFun(key);
                if (string.IsNullOrEmpty(vv) == true)
                {
                    vv = "";
                }
                saveList.Add(key, vv);
            }
        }
        return vv;
    }
    static string dic_to_string(Dictionary<string, string> _v)
    {
        string v = "";
        foreach (string k in _v.Keys)
        {
            if (v == "")
            {
                v = k + sp2 + _v[k];
            }
            else
            {
                v += sp1 + k + sp2 + _v[k];
            }
        }
        return v;
    }
    static Dictionary<string, string> string_to_dic(string v)
    {
        Dictionary<string, string> list = new Dictionary<string, string>();
        if (string.IsNullOrEmpty(v) == false)
        {
            string[] sss = v.Split(sp1);
            for (int i = 0; i < sss.Length; i++)
            {
                string[] vvv = sss[i].Split(sp2);
                if (vvv.Length == 2)
                {
                    list.Add(vvv[0], vvv[1]);
                }
            }
        }
        return list;
    }

    static string dicList_to_string(Dictionary<string, List<string>> _v)
    {
        string v = "";
        foreach (string k in _v.Keys)
        {
            string list = "";
            if (_v[k] != null)
            {
                for (int i = 0; i < _v[k].Count; i++)
                {
                    if (i == 0)
                    {
                        list = _v[k][i];
                    }
                    else
                    {
                        list += sp3 + _v[k][i];
                    }
                }
            }
            if (v == "")
            {

                v = k + sp2 + list;
            }
            else
            {
                v += sp1 + k + sp2 + list;
            }
        }
        return v;
    }
    static Dictionary<string, List<string>> string_to_dicList(string v)
    {
        Dictionary<string, List<string>> list = new Dictionary<string, List<string>>();
        if (string.IsNullOrEmpty(v) == false)
        {
            string[] sss = v.Split(sp1);
            for (int i = 0; i < sss.Length; i++)
            {
                string[] vvv = sss[i].Split(sp2);
                if (vvv.Length == 2)
                {
                    List<string> vvv2 = new List<string>();
                    if (string.IsNullOrEmpty(vvv[1]) == false)
                    {
                        string[] vvvStr = vvv[1].Split(sp3);
                        for (int i2 = 0; i2 < vvvStr.Length; i2++)
                        {
                            vvv2.Add(vvvStr[i2]);
                        }
                    }
                    list.Add(vvv[0], vvv2);
                }
            }
        }
        return list;
    }

    static string get_save_dic(string key, Dictionary<string, string> value, bool is_change_value = true)
    {
        string vv = "";
        if (saveDic.ContainsKey(key) == true)
        {
            vv = saveDic[key];
            if (is_change_value == true)
                saveDic[key] = dic_to_string(value);
        }
        else
        {
            if (is_change_value == true)
                saveDic.Add(key, dic_to_string(value));
            else
            {
                init_save();
                vv = GetStringFun(key);
                if (string.IsNullOrEmpty(vv) == true)
                {
                    vv = "";
                }
                saveDic.Add(key, vv);
            }

        }
        return vv;
    }

    #endregion

    #region 保存获取处理方法
    static void init_save()
    {
        if (init_save_fun != null)
        {
            init_save_fun();
            init_save_fun = null;
        }
    }
    public static Action init_save_fun = null;
    // 微信小游戏不依赖 Unity WebGL 的 IndexedDB PlayerPrefs，直接使用微信 Storage。
    // 其他平台继续使用 Unity PlayerPrefs，并在每次写入后立即提交，避免退后台时丢失未落盘数据。
#if PT_wx && !UNITY_EDITOR && !PT_web
    static void SetInt(string key, int value)
    {
        WXBase.StorageSetIntSync(key, value);
    }

    static void SetFloat(string key, float value)
    {
        WXBase.StorageSetFloatSync(key, value);
    }

    static void SetString(string key, string value)
    {
        WXBase.StorageSetStringSync(key, value);
    }

    static int GetInt(string key)
    {
        return WXBase.StorageGetIntSync(key, 0);
    }

    static float GetFloat(string key)
    {
        return WXBase.StorageGetFloatSync(key, 0f);
    }

    static string GetString(string key)
    {
        return WXBase.StorageGetStringSync(key, "");
    }

    static bool HasKey(string key)
    {
        return WXBase.StorageHasKeySync(key);
    }

    static void DeleteKey(string key)
    {
        WXBase.StorageDeleteKeySync(key);
    }
#elif PT_tt && !UNITY_EDITOR && !PT_web
    static void SetInt(string key, int value)
    {
        TTSDK.TT.Save(value, key);
    }

    static void SetFloat(string key, float value)
    {
        TTSDK.TT.Save(value, key);
    }

    static void SetString(string key, string value)
    {
        TTSDK.TT.Save(value, key);
    }

    static int GetInt(string key)
    {
        return TTSDK.TT.LoadSaving<int>(key);
    }

    static float GetFloat(string key)
    {
        return TTSDK.TT.LoadSaving<float>(key);
    }

    static string GetString(string key)
    {
        return TTSDK.TT.LoadSaving<string>(key) ?? "";
    }

    static bool HasKey(string key)
    {
        // TTSDK 的旧接口未提供 HasKey；DataTools 的索引 key 会记录新写入的数据。
        return !string.IsNullOrEmpty(GetString(key));
    }

#else
    static void SetInt(string key, int value)
    {
        UnityEngine.PlayerPrefs.SetInt(key, value);
        UnityEngine.PlayerPrefs.Save();
    }

    static void SetFloat(string key, float value)
    {
        UnityEngine.PlayerPrefs.SetFloat(key, value);
        UnityEngine.PlayerPrefs.Save();
    }

    static void SetString(string key, string value)
    {
        UnityEngine.PlayerPrefs.SetString(key, value);
        UnityEngine.PlayerPrefs.Save();
    }

    static int GetInt(string key)
    {
        return UnityEngine.PlayerPrefs.GetInt(key);
    }

    static float GetFloat(string key)
    {
        return UnityEngine.PlayerPrefs.GetFloat(key);
    }

    static string GetString(string key)
    {
        return UnityEngine.PlayerPrefs.GetString(key);
    }

    static bool HasKey(string key)
    {
        return UnityEngine.PlayerPrefs.HasKey(key);
    }

    static void DeleteAll()
    {
        UnityEngine.PlayerPrefs.DeleteAll();
        UnityEngine.PlayerPrefs.Save();
    }

#endif

    public static Action<string, int> SetIntFun = SetInt;
    public static Action<string, float> SetFloatFun = SetFloat;
    public static Action<string, string> SetStringFun = SetString;
    public static Func<string, int> GetIntFun = GetInt;
    public static Func<string, float> GetFloatFun = GetFloat;
    public static Func<string, string> GetStringFun = GetString;
    static void ensureSavedKeysLoaded()
    {
        if (savedKeysLoaded)
        {
            return;
        }

        savedKeysLoaded = true;
        string keys = GetStringFun(SavedKeysKey);
        if (string.IsNullOrEmpty(keys))
        {
            return;
        }

        foreach (string key in keys.Split(sp1))
        {
            if (!string.IsNullOrEmpty(key))
            {
                savedKeys.Add(key);
            }
        }
    }

    static void trackSavedKey(string key)
    {
        if (key == SavedKeysKey)
        {
            return;
        }

        ensureSavedKeysLoaded();
        if (savedKeys.Add(key))
        {
            SetStringFun(SavedKeysKey, string.Join(sp1.ToString(), savedKeys));
        }
    }

    /// <summary>
    /// Returns whether a key has been stored through DataTools in Unity PlayerPrefs.
    /// </summary>
    public static bool hasData(string key)
    {
        ensureSavedKeysLoaded();
        return savedKeys.Contains(key) || HasKey(key);
    }
    #endregion

    #region 保存
    public static void save(string _key, DateTime _v, DataSaveTy _ty = DataSaveTy.none, Action<long, long> _fun = null)
    {
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        string key = _key;
        string _vv = Tools.Instance.timeToSeconds(_v).ToString();
        if (string.IsNullOrEmpty(_vv) == true)
        {
            _vv = "0";
        }
        string ss = get_save(key, _vv);
        if (string.IsNullOrEmpty(ss) == true)
        {
            ss = "0";
        }
        init_save();
        trackSavedKey(key);
        SetStringFun(key, _vv);
        if (ss != _vv)
        {
            _fun?.Invoke(long.Parse(ss), long.Parse(_vv));
        }
    }

    public static void save(string _key, int _v, DataSaveTy _ty = DataSaveTy.none, Action<int, int> _fun = null)
    {
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        string key = _key;
        int ss = get_save(key, _v);
        init_save();
        trackSavedKey(key);
        SetIntFun(key, _v);
        if (ss != _v)
        {
            _fun?.Invoke(ss, _v);
        }
    }

    public static void save(string _key, float _v, DataSaveTy _ty = DataSaveTy.none, Action<float, float> _fun = null)
    {
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        string key = _key;
        float ss = get_save(key, _v);
        init_save();
        trackSavedKey(key);
        SetFloatFun(key, _v);
        if (ss != _v)
        {
            _fun?.Invoke(ss, _v);
        }
    }

    public static void save(string _key, string _v, DataSaveTy _ty = DataSaveTy.none, Action<string, string> _fun = null)
    {
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        string key = _key;
        string ss = get_save(key, _v);
        init_save();
        trackSavedKey(key);
        SetStringFun(key, _v);
        if (ss != _v)
        {
            _fun?.Invoke(ss, _v);
        }
    }

    public static void save(string _key, List<string> _v, DataSaveTy _ty = DataSaveTy.none, Action<string, string> _fun = null)
    {
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        string key = _key;
        string ss = get_save_list(key, _v);
        string vv = saveList[key];
        init_save();
        trackSavedKey(key);
        SetStringFun(key, vv);
        if (ss != vv)
        {
            _fun?.Invoke(ss, vv);
        }
    }

    public static void save(string _key, List<int> _v, DataSaveTy _ty = DataSaveTy.none, Action<string, string> _fun = null)
    {
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        string key = _key;
        List<string> _vList = new List<string>();
        for (int i = 0; i < _v.Count; i++)
        {
            _vList.Add(_v[i] + "");
        }
        string ss = get_save_list(key, _vList);
        string vv = saveList[key];
        init_save();
        trackSavedKey(key);
        SetStringFun(key, vv);
        if (ss != vv)
        {
            _fun?.Invoke(ss, vv);
        }
    }

    public static void save(string _key, Dictionary<string, string> _v, DataSaveTy _ty = DataSaveTy.none, Action<string, string> _fun = null)
    {
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        string key = _key;
        string ss = get_save_dic(key, _v);
        string vv = saveDic[key];
        init_save();
        trackSavedKey(key);
        SetStringFun(key, vv);
        if (ss != vv)
        {
            _fun?.Invoke(ss, vv);
        }
    }

    public static void save(string _key, Dictionary<string, List<string>> _v, DataSaveTy _ty = DataSaveTy.none, Action<string, string> _fun = null)
    {
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        string key = _key;
        string vv = dicList_to_string(_v);
        string ss = get_save(key, vv);
        init_save();
        trackSavedKey(key);
        SetStringFun(key, vv);
        if (ss != vv)
        {
            _fun?.Invoke(ss, vv);
        }
    }
    #endregion

    #region 获取
    public static int getInt(string key, DataSaveTy _ty = DataSaveTy.none)
    {
        int v = 0;

        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        v = get_save(key, 0, false);
        return v;
    }
    public static float getFloat(string key, DataSaveTy _ty = DataSaveTy.none)
    {
        float v = 0;
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        v = get_save(key, 0f, false);
        return v;
    }
    public static string getString(string key, DataSaveTy _ty = DataSaveTy.none)
    {
        string v = "";
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        v = get_save(key, "", false);
        return v;
    }
    public static DateTime getDataTime(string key, DataSaveTy _ty = DataSaveTy.none)
    {
        string v = "";
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        v = get_save(key, "", false);
        DateTime origin;
        if (string.IsNullOrEmpty(v) == false && v != "0")
        {
            origin = Tools.Instance.secondsToTime(long.Parse(v));
        }
        else
        {
            origin = new DateTime(1970, 1, 1, 8, 0, 0, 0, DateTimeKind.Local);
        }
        return origin;
    }
    public static List<string> getList(string key, DataSaveTy _ty = DataSaveTy.none)
    {
        string v = "";
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        v = get_save_list(key, null, false);
        List<string> list = string_to_list(v);
        return list;
    }
    public static Dictionary<string, string> getDic(string key, DataSaveTy _ty = DataSaveTy.none)
    {
        string v = "";
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        v = get_save_dic(key, null, false);
        Dictionary<string, string> list = string_to_dic(v);
        return list;
    }

    public static Dictionary<string, List<string>> getDicList(string key, DataSaveTy _ty = DataSaveTy.none)
    {
        string v = "";
        DataSaveTy ty = saveTy;
        if (_ty != DataSaveTy.none)
        {
            ty = _ty;
        }
        v = get_save(key, "", false);
        Dictionary<string, List<string>> list = string_to_dicList(v);
        return list;
    }

    #endregion

    public static void clearData()
    {
#if PT_wx && !UNITY_EDITOR && !PT_web
        ensureSavedKeysLoaded();
        foreach (string key in savedKeys)
        {
            DeleteKey(key);
        }
        DeleteKey(SavedKeysKey);
#elif PT_tt && !UNITY_EDITOR && !PT_web
        // TTSDK 的当前接口没有删除存档 API，只清空本次运行的内存缓存。
#else
        DeleteAll();
#endif
        saveInt.Clear();
        saveFloat.Clear();
        saveStr.Clear();
        saveList.Clear();
        saveDic.Clear();
        savedKeys.Clear();
        savedKeysLoaded = true;
    }
}

public enum DataSaveTy
{
    /// <summary>
    /// 默认
    /// </summary>
    none = 0,
    /// <summary>
    /// 本地
    /// </summary>
    local = 1,
    /// <summary>
    /// 网络
    /// </summary>
    net = 2
}
