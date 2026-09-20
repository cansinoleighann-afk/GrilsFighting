using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class ResetFont : MonoBehaviour
{
    [Header("物体")]
    public GameObject gameobj;

    [Header("字体")]
    public Font font;

    [Header("脚本总文件夹")]
    public List<string> scriptsPath = new List<string>()
    {
        "Assets/Scripts","Assets/Resources/AssetConfig"
    };


    [ContextMenu("替换物体的字体")]
    void reset_a()
    {
        Text[] texts = gameobj.GetComponentsInChildren<Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].font != font)
            {
                texts[i].font = font;
                Debug.Log("修改了物体：" + texts[i].name);
            }
        }
    }

    [ContextMenu("读取物体gameobj字保存到本地txt")]
    void read_text()
    {
        ReadTxt.read_text_static(gameobj);
    }

    [ContextMenu("读取脚本总文件夹中所有中文字并保存到本地txt")]
    void ExtractStringsFromScripts()
    {
        for (int i = 0;i < scriptsPath.Count;i++)
        {
            ReadTxt.read_text(ExtractStringsFromScripts(scriptsPath[i]));
        }
    }

    private string ExtractStringsFromScripts(string folderPath)
    {
 
        string ssss = "";
        var allowedExtensions = new[] { ".cs", ".asset" };
        Debug.Log("路径：" + folderPath + "，是否存在:" + Directory.Exists(folderPath));
        var allFiles = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
        var files = allFiles
            .Where(f => allowedExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
            .ToArray();


        Regex quoteRegex = new Regex("\"(.*?)\"", RegexOptions.Compiled);

        foreach (var file in files)
        {
            string content = File.ReadAllText(file);
            MatchCollection matches = quoteRegex.Matches(content);

            foreach (Match match in matches)
            {
                string str = match.Groups[1].Value;

                str = Regex.Replace(str, @"\{.*?\}", "");

                str = Regex.Replace(str, @"[^一-龥]", "");

                str = str.Trim();

                if (!string.IsNullOrEmpty(str))
                {
                    for (int i = 0; i < str.Length; i++)
                    {
                        char c = str[i];
                        if (ssss.Contains(c) == false)
                        {
                            ssss += c;
                        }
                    }
                }
            }
        }
        Debug.Log("文件数：：：：：：：：" + files.Length + "   :" + ssss);
        return ssss;
    }
}
