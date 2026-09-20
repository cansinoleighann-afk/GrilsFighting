//自动生成，请勿手动修改！来源：D:/work/unityPro/adCon/Assets/EditorTools/Excel\参数.xlsx
#pragma warning disable 0108
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试
/// </summary>
public class ProConfig : ScriptableObject
{
    [Header("时间")]
    public int time;
    [Header("地图")]
    public GameObject map;
    [Header("坐标")]
    public List<Vector3> pos = new List<Vector3>();
    [Header("物品数据")]
    public List<string> objDic = new List<string>();

    static ProConfig _self;
    public static ProConfig self
    {
        get
        {
            if (_self == null)
            {
                _self = Resources.Load<ProConfig>("Excel/参数");
            }
            return _self;
        }
    }
}
