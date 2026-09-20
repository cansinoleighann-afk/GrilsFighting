using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

public class ComponentRemover : EditorWindow
{
    private GameObject targetObject;
    private int selectedComponentIndex = 0;
    private string[] componentNames;
    private Vector2 scrollPosition;

    [MenuItem("Tools/删除Component组件")]
    public static void ShowWindow()
    {
        GetWindow<ComponentRemover>("删除Component");
    }

    void OnGUI()
    {
        GUILayout.Label("删除Component", EditorStyles.boldLabel);

        // 选择目标GameObject
        targetObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject", targetObject, typeof(GameObject), true);

        if (targetObject != null)
        {
            // 获取所有组件
            List<Component> components = get_all_component(targetObject);
            componentNames = components.Select(c => c.GetType().Name).ToArray();

            if (components.Count > 0)
            {
                // 显示组件选择下拉菜单
                selectedComponentIndex = EditorGUILayout.Popup("选择 Component", selectedComponentIndex, componentNames);

                // 显示所有组件列表（可选）
                GUILayout.Label("所有 Components:", EditorStyles.boldLabel);
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));
                for (int i = 0; i < components.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"{i + 1}. {componentNames[i]}");
                    if (GUILayout.Button("移除", GUILayout.Width(80)))
                    {
                        RemoveComponent(components[i]);
                        //return; // 刷新UI
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                // 删除选中组件按钮
                if (GUILayout.Button("移除选择 Component"))
                {
                    if (selectedComponentIndex >= 0 && selectedComponentIndex < components.Count)
                    {
                        RemoveComponent(components[selectedComponentIndex]);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No components found on this GameObject.", MessageType.Info);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please select a GameObject first.", MessageType.Info);
        }
    }
    List<Component> get_all_component(GameObject g)
    {
        List<Component> ccc = new List<Component>();
        Tools.Instance.GetAllObj_fun(g, (gg) =>
        {
            Component[] components = gg.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component c= ccc.Find(s => s.GetType().Name == components[i].GetType().Name);
                if (c==null)
                {
                    ccc.Add(components[i]);
                }
            }
            return false;
        });
        ccc.Sort((x, y) => x.GetType().Name.CompareTo(y.GetType().Name));
        return ccc;
    }

    private void RemoveComponent(Component component)
    {
        if (component == null) return;

        // 特殊处理Transform组件
        if (component is Transform)
        {
            EditorUtility.DisplayDialog("无法删除", "转换组件无法被移除", "OK");
            return;
        }

        // 确认对话框
        if (EditorUtility.DisplayDialog("确认移除",
            $"您确定要移除吗？ {component.GetType().Name} from {targetObject.name}?",
            "移除", "取消"))
        {
            Tools.Instance.GetAllObj_fun(targetObject, (gg) =>
            {
                Component[] components = gg.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].GetType().Name == component.GetType().Name)
                    {
                        Undo.DestroyObjectImmediate(components[i]);
                        Debug.Log($"移除了组件 {component.GetType().Name} 从 {gg}物体中");
                    }
                }
                return false;
            });
            EditorUtility.SetDirty(targetObject);
            
        }
    }
}