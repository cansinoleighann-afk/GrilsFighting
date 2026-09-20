using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class ReadTxt
{
    public static string path = "Resources/Font/font.txt";
    public static string idleStr = "1234567890!！%:：;；“”‘’,，.。?？(（)）_-=+/*qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
    public static void read_text(GameObject g)
    {
#if UNITY_EDITOR
        Tools.Instance.LaterFun(() =>
        {
            if (g != null)
            {
                List<Text> list = new List<Text>();
                Tools.Instance.GetAllObj_fun(g, (ggg) =>
                {
                    Text tt = ggg.GetComponent<Text>();
                    if (tt != null)
                    {
                        list.Add(tt);
                    }
                    return false;
                });
                string str = "";
                if (g.GetComponent<Text>() != null)
                {
                    str += g.GetComponent<Text>().text;
                }
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        str += list[i].text;
                    }
                }
                write_text(str);
            }
        }, 0.1f);
#endif
    }
    public static void read_text(string ss)
    {
#if UNITY_EDITOR
        if (ss != "")
        {
            write_text(ss);
        }
#endif
    }
    public static void read_text_static(GameObject g)
    {
        if (g != null)
        {
            List<Text> list = new List<Text>();
            Tools.Instance.GetAllObj_fun(g, (ggg) =>
            {
                Text tt = ggg.GetComponent<Text>();
                if (tt != null)
                {
                    list.Add(tt);
                }
                return false;
            });
            string str = "";
            if (g.GetComponent<Text>() != null)
            {
                str += g.GetComponent<Text>().text;
            }
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    str += list[i].text;
                }
            }
            write_text(str);
        }
    }
    static void write_text(string str)
    {

        string filePath = Application.dataPath + "/" + path;
        string oldStr = idleStr;
        if (!File.Exists(filePath))
        {
            Debug.Log("文件不存在" + filePath);
        }
        else
        {
            oldStr += File.ReadAllText(filePath);
            string ss = "";
            char[] chars = str.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (oldStr.Contains(chars[i]) == false)
                {
                    ss += chars[i];
                }
            }
            str = ss;
        }
        str = oldStr + str;

        str = str.Replace(" ", "");
        str = str.Replace("\n", "");
        str = str.Replace("\r", "");
        str = new string(str.Distinct().ToArray());
        File.WriteAllText(filePath, str);
    }
}

