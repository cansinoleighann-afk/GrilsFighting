//自动生成，请勿手动修改！来源：D:/work/unityPro/adCon/Assets/EditorTools/Excel\列表数据.xlsx
#pragma warning disable 0108
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试
/// </summary>
public class ListConfig : ScriptableObject
{
    [Header("数据列表")]
    public List<ListConfigItem> list = new List<ListConfigItem>();

    static ListConfig _self;
    public static ListConfig self
    {
        get
        {
            if (_self == null)
            {
                _self = Resources.Load<ListConfig>("Excel/列表数据");
            }
            return _self;
        }
    }
}

[Serializable]
public class ListConfigItem
{
    [Header("key值")]
    public string key;
    [Header("炮台大小")]
    public int scale;
    [Header("攻击力")]
    public float attack;
    [Header("列表")]
    public List<int> ll = new List<int>();
    [Header("列表")]
    public List<string> ll_str = new List<string>();
    [Header("物体")]
    public GameObject obj;
    [Header("坐标")]
    public Vector3 pos;
    [Header("图标")]
    public Sprite icon;
}
