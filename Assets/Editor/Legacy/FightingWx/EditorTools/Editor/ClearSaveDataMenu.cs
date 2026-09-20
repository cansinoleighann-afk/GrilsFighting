using UnityEditor;
using UnityEngine;

public static class ClearSaveDataMenu
{
    [MenuItem("Tools/清空本地存档")]
    private static void ClearSaveData()
    {
        if (!EditorUtility.DisplayDialog("清空本地存档", "将清空当前项目的本地存档数据，此操作无法撤销。", "清空", "取消"))
        {
            return;
        }

        DataTools.clearData();
        PlayerPrefs.Save();
        Debug.Log("本地存档已清空。");
    }
}
