
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
public class SetPlatform : EditorWindow
{
    public PlatformTy Tty = PlatformTy.wx;
    SetPlatform()
    {
        this.titleContent = new GUIContent("设置平台");
    }
    //添加菜单栏用于打开窗口
    [MenuItem("Tools/设置平台")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(SetPlatform));
    }

    void OnGUI()
    {
        init_pt_list();
        Rect fileRect2 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        Tty = (PlatformTy)EditorGUILayout.EnumPopup(Tty);//EnumFlagsField

        if (GUILayout.Button("切换平台 当前平台"+ nowPT()))
        {
            SetWX();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.TextArea("当前平台:"+ nowPT());
    }
    List<string> pt_list = new List<string>();
    List<string> pt_list_say=new List<string>();
    void init_pt_list()
    {
        if (pt_list.Count <= 0)
        {
            pt_list.Add("PT_" + PlatformTy.wx.ToString());
            pt_list_say.Add("微信");
            pt_list.Add("PT_" + PlatformTy.tt.ToString());
            pt_list_say.Add("抖音");
            pt_list.Add("PT_" + PlatformTy.vivo.ToString());
            pt_list_say.Add("vivo");
            pt_list.Add("PT_" + PlatformTy.oppo.ToString());
            pt_list_say.Add("oppo");
            pt_list.Add("PT_" + PlatformTy.android.ToString());
            pt_list_say.Add("android");
            pt_list.Add("PT_" + PlatformTy.ios.ToString());
            pt_list_say.Add("苹果");
            pt_list.Add("PT_" + PlatformTy.web.ToString());
            pt_list_say.Add("web网页");
            pt_list.Add("PT_" + PlatformTy.qq.ToString());
            pt_list_say.Add("qq");
            pt_list.Add("PT_" + PlatformTy.ry.ToString());
            pt_list_say.Add("荣耀");
            pt_list.Add("PT_" + PlatformTy.hw.ToString());
            pt_list_say.Add("华为");
            pt_list.Add("PT_" + PlatformTy.ks.ToString());
            pt_list_say.Add("快手");
            pt_list.Add("PT_" + PlatformTy.mini_4399.ToString());
            pt_list_say.Add("小程序4399");
            pt_list.Add("PT_" + PlatformTy.web_4399.ToString());
            pt_list_say.Add("web端4399");
        }
    }
    //[InitializeOnLoadMethod]
    void SetWX()
    {
        AddDefines(Tty);
    }
    public void AddDefines(PlatformTy ty)
    {
        //string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        //List<string> ss = new List<string>(currentSymbols.Split(';'));
        //currentSymbols += ";WEB_wx";
        //for (int i = 0; i < ss.Count; i++)
        //{
        //    Debug.Log("宏定义：：：：：：" + ss[i]);
        //}
        //PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentSymbols);
        string yesPT = "PT_" + ty.ToString();

        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        List<string> list = new List<string>(currentSymbols.Split(';'));

        List<string> re = new List<string>();
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < pt_list.Count; j++)
            {
                if (list[i] == pt_list[j])
                {
                    re.Add(list[i]);
                }
            }
        }

        for (int i = 0; i < re.Count; i++)
        {
            list.Remove(re[i]);
        }
        list.Add(yesPT);
        string yesSymbol = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (yesSymbol == "")
            {
                yesSymbol = list[i];
            }
            else
            {
                yesSymbol += ";" + list[i];
            }
        }
        Debug.Log("修改成：：：：" + currentSymbols + "  完成后:" + yesSymbol);
        if (currentSymbols != yesSymbol)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, yesSymbol);
            AssetDatabase.Refresh();
        }
    }

    string nowPT()
    {
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        string yes = "PT_wx";
        
        List<string> list = new List<string>(currentSymbols.Split(';'));
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < pt_list.Count; j++)
            {
                if (list[i] == pt_list[j])
                {
                    //yes = list[i];
                    yes= pt_list_say[j];
                }
            }
        }
        return yes;
    }

}
#endif