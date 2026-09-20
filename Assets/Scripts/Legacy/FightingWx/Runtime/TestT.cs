using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestT : MonoBehaviour
{
    public static TestT self = null;
    [Header("是否使用测试数据")]
    public bool isUse_Value = true;
    public float versions = 0;
    public List<TestData> dicValue = new List<TestData>();
    private void Awake()
    {
#if UNITY_EDITOR || PT_web
        self = this;
#endif
        float v = DataTools.getFloat("versions");
        if (isUse_Value && versions != v)
        {
            DataTools.clearData();
            DataTools.save("versions", versions);
        }

    }
    //private void Start()
    //{
    //    WXAdView.show_view(0);
    //}
    public static string get_str(string key, string value)
    {
        string str = value;
        if (self != null && self.isUse_Value == true)
        {
            TestData dd = self.dicValue.Find(s => s.key == key);
            if (dd != null && dd.value != "")
            {
                str = dd.value;
            }
        }

        return str;
    }
    public static int get_int(string key, int value)
    {
        int str = value;
        if (self != null && self.isUse_Value == true)
        {
            TestData dd = self.dicValue.Find(s => s.key == key);
            if (dd != null && dd.value != "")
            {
                str = int.Parse(dd.value);
            }
        }
        return str;
    }
    public static float get_float(string key, float value)
    {
        float str = value;
        if (self != null && self.isUse_Value == true)
        {
            TestData dd = self.dicValue.Find(s => s.key == key);
            if (dd != null && dd.value != "")
            {
                str = float.Parse(dd.value);
            }
        }
        return str;
    }

    /// <summary>
    /// 获取微信数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ori"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public T get_wx_data<T>(T ori, string key)//where T : object
    {
#if PT_wx
        if (WebSdk.self.wxTestValue.ContainsKey(key) == true)
        {
            ori = (T)Convert.ChangeType(WebSdk.self.wxTestValue[key], typeof(T));
        }
#endif
        return ori;
    }


    #region 画线
    public void clear_line()
    {
#if UNITY_EDITOR
        list_list_line.Clear();
#endif
    }
    /// <summary>
    /// 新增obj标签
    /// </summary>
    /// <param name="p"></param>
    /// <param name="nn"></param>
    public void newTestObj(Vector3 p, string nn)
    {
#if UNITY_EDITOR
        GameObject g = new GameObject(nn);
        g.transform.SetParent(transform);
        g.transform.position = p;
        AddGrayDotIcon(g);
#endif
    }
    /// <summary>
    /// 画一个正方形
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    public void drawCube(Vector3 center, float size, float time = 5)
    {
#if UNITY_EDITOR
        cubePos = center;
        cubeSize = size;
        cubeTime = time;
#endif
    }
    /// <summary>
    /// 画一条路径
    /// </summary>
    /// <param name="list"></param>
    public void debug_line(List<Vector3> list)
    {
#if UNITY_EDITOR
        if (list != null && list.Count > 0)
        {
            list_list_line.Add(new List<Vector3>(list));
        }
#endif
    }
    /// <summary>
    /// 画一个圆形
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="radius"></param>
    public void drawCircle(Vector3 pos, float radius, float tt = 5)
    {
#if UNITY_EDITOR
        dPos.x = pos.x; dPos.y = pos.y; dPos.z = pos.z;
        dPosT = tt;
        dPosRadius = radius;
#endif
    }

#if UNITY_EDITOR
    Vector3 cubePos = Vector3.zero;
    float cubeSize = 1;
    float cubeTime = 0;
    Vector3 dPos = Vector3.zero;
    float dPosRadius = 1;
    float dPosT = 0;
    List<List<Vector3>> list_list_line = new List<List<Vector3>>();
    void OnDrawGizmos()
    {
        if (list_list_line.Count > 0)
        {
            for (int k = 0; k < list_list_line.Count; k++)
            {
                List<Vector3> list_line = list_list_line[k];
                int ck = 10 - k;
                Gizmos.color = new Color((ck * 3f) % 10 / 10f, (ck * 4f) % 10 / 10f, (ck * 5f) % 10 / 10f, 1);

                // 绘制路径点
                for (int i = 0; i < list_line.Count; i++)
                {
                    Gizmos.DrawSphere(list_line[i], 1);
                }

                // 绘制连接线
                for (int i = 0; i < list_line.Count - 1; i++)
                {
                    Gizmos.DrawLine(list_line[i], list_line[i + 1]);
                }
            }
        }
        if (dPosT > 0)
        {
            dPosT -= Time.deltaTime;
            Gizmos.color = Color.red;
            _DrawCircle(dPos, dPosRadius);
        }
        if (cubeTime > 0)
        {
            cubeTime -= Time.deltaTime;
            Gizmos.color = Color.red;
            _DrawCube(cubePos, cubeSize);
        }
    }
    // 绘制正方形的方法
    void _DrawCube(Vector3 center, float size)
    {
        float halfSize = size / 2f;
        Vector3 topLeft = center + new Vector3(-halfSize, 0, halfSize);
        Vector3 topRight = center + new Vector3(halfSize, 0, halfSize);
        Vector3 bottomLeft = center + new Vector3(-halfSize, 0, -halfSize);
        Vector3 bottomRight = center + new Vector3(halfSize, 0, -halfSize);

        // 绘制四条边
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
    // 绘制圆形的方法
    void _DrawCircle(Vector3 center, float radius, int segments = 20)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
    public void AddGrayDotIcon(GameObject selectedObj)
    {
        // 关键：获取圆点图标
        // 内置圆点图标名称为 "DotFill"
        GUIContent iconContent = EditorGUIUtility.IconContent("sv_label_0"); //DotFill
        Texture2D dotIcon = iconContent.image as Texture2D;

        if (dotIcon != null)
        {
            EditorGUIUtility.SetIconForObject(selectedObj, dotIcon);
            EditorUtility.SetDirty(selectedObj);
        }
        else
        {
            Debug.LogWarning("未找到圆点图标");
        }
    }
#endif
    #endregion
}
[Serializable]
public class TestData
{
    public string key;
    public string say = "";
    public string value;
}
