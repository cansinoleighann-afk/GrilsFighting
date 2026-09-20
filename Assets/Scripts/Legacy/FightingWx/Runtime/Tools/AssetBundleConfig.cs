
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleConfig
{
    private static AssetBundleConfig _self;
    public static AssetBundleConfig self { get { if (_self == null) { _self = new AssetBundleConfig(); } return _self; } }
    #region 自动生产路径é
    public string webglV = "WebGL283056";
    
    public string car2_2_car22 = "car2_2;car22";
    public string car3_4_car34 = "car3_4;car34";
    public string car3_6_car36 = "car3_6;car36";
    public string car4_3_car43 = "car4_3;car43";
    public string car5_8_car58 = "car5_8;car58";
    public string car6_1_car61 = "car6_1;car61";
    public string car6_2_car62 = "car6_2;car62";
    public string car6_3_car63 = "car6_3;car63";
    public string carLookPre1_carlookpre = "carLookPre1;carlookpre";
    public string OrderView_taskview = "OrderView;taskview";
    public string bbb_test = "bbb;test";
    public string wheel_ori_wheelori = "wheel_ori;wheelori";
    public Dictionary<string,string> abList = new Dictionary<string, string> { 
        { "car2_2","car2_2;car22"},{ "car3_4","car3_4;car34"},{ "car3_6","car3_6;car36"},{ "car4_3","car4_3;car43"},{ "car5_8","car5_8;car58"},{ "car6_1","car6_1;car61"},{ "car6_2","car6_2;car62"},{ "car6_3","car6_3;car63"},{ "carLookPre1","carLookPre1;carlookpre"},{ "OrderView","OrderView;taskview"},{ "bbb","bbb;test"},{ "wheel_ori","wheel_ori;wheelori"}
    };
    //图集
    public Dictionary<string,string> abAtlasList = new Dictionary<string, string> { 
        
    };
    public Dictionary<string,string> abPcList = new Dictionary<string, string> { 
        { "car2_2;car22","Assets/res/Gril/Car/car/car2_2.prefab"},{ "car3_4;car34","Assets/res/Gril/Car/car/car3_4.prefab"},{ "car3_6;car36","Assets/res/Gril/Car/car/car3_6.prefab"},{ "car4_3;car43","Assets/res/Gril/Car/car/car4_3.prefab"},{ "car5_8;car58","Assets/res/Gril/Car/car/car5_8.prefab"},{ "car6_1;car61","Assets/res/Gril/Car/car/car6_1.prefab"},{ "car6_2;car62","Assets/res/Gril/Car/car/car6_2.prefab"},{ "car6_3;car63","Assets/res/Gril/Car/car/car6_3.prefab"},{ "carLookPre1;carlookpre","Assets/res/Gril/garage/Pre/carLookPre1.prefab"},{ "OrderView;taskview","Assets/Resource2/View/OrderView.prefab"},{ "bbb;test","Assets/EffectNo/res/mesh/bbb.prefab"},{ "wheel_ori;wheelori","Assets/res/Gril/Car/wheel/wheel_ori.prefab"}
    };
    #endregion é

    string _pathAB = ""; 
    /// <summary>
    /// ab网络资源加载的路径
    /// </summary>
    public string pathAB 
    { 
        get 
        {
            if (_pathAB == "")
            {
                //本地的assetbundle路径
                _pathAB = Application.streamingAssetsPath;
            }
            return _pathAB; 
        }
        set
        {
            _pathAB=value;
        }
    }

    /// <summary>
    /// 是否使用ab加载
    /// </summary>
    public bool is_useAb = false;

    /// <summary>
    /// 清理ab包时不移除的资源列表
    /// </summary>
    public List<string> noClearPack = new List<string>()
    {
        "view","atlas"
    };

    /// <summary>
    /// 加载ab或者resource
    /// </summary>
    /// <param name="n">物体名字</param>
    /// <param name="resourcePath">如果是resourc加载就填路径</param>
    /// <param name="fun"></param>
    /// <param name="arg"></param>
    public void loadSelectRes(string n,string resourcePath, Action< GameObject, EventArg> fun, EventArg arg = null,bool isLoadOtherPath=false)
    {
        if (is_useAb==true && abList.ContainsKey(n) == true)
        {
            ResLoad.self.AsyncLoadAB(abList[n], (pre, arg1) =>
            {
                if (fun != null)
                {
                    fun(pre, arg1);
                }
            },arg);
        }
        else
        {
            GameObject pre= ResLoad.self.loadAll(resourcePath+n,null, isLoadOtherPath);
            if (fun != null)
            {
                fun(pre, arg);
            }
        }
    }
}