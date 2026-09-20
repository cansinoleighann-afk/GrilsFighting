using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class FindMonoObj : EditorWindow
{
    private MonoScript targetScript;
    private Vector2 scrollPosition;
    private List<GameObject> foundObjects = new List<GameObject>();
    private Dictionary<string, List<GameObject>> objectsByScene = new Dictionary<string, List<GameObject>>();
    private bool searchInPrefabs = true;
    private bool searchInOpenScenes = true;
    private string searchFilter = "";

    [MenuItem("Tools/高级脚本引用查找")]
    public static void ShowWindow()
    {
        GetWindow<FindMonoObj>("脚本引用查找器");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("查找挂载了指定脚本的所有物体", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 脚本选择
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("目标脚本:", GUILayout.Width(60));
        targetScript = (MonoScript)EditorGUILayout.ObjectField(targetScript, typeof(MonoScript), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 搜索选项
        EditorGUILayout.LabelField("搜索范围:", EditorStyles.boldLabel);
        searchInOpenScenes = EditorGUILayout.Toggle("当前打开的場景", searchInOpenScenes);
        searchInPrefabs = EditorGUILayout.Toggle("Project中的Prefab", searchInPrefabs);

        EditorGUILayout.Space();

        // 过滤选项
        searchFilter = EditorGUILayout.TextField("过滤关键词:", searchFilter);

        EditorGUILayout.Space();

        // 搜索按钮
        GUI.enabled = targetScript != null;
        if (GUILayout.Button("开始搜索", GUILayout.Height(30)))
        {
            SearchReferences();
        }
        GUI.enabled = true;

        EditorGUILayout.Space();

        // 结果显示
        if (foundObjects.Count > 0)
        {
            EditorGUILayout.LabelField($"搜索结果: 共找到 {foundObjects.Count} 个物体", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var kvp in objectsByScene)
            {
                if (kvp.Value.Count == 0) continue;

                EditorGUILayout.LabelField($"--- {kvp.Key} ({kvp.Value.Count}个物体) ---", EditorStyles.miniBoldLabel);

                foreach (GameObject obj in kvp.Value)
                {
                    if (!string.IsNullOrEmpty(searchFilter) && !obj.name.ToLower().Contains(searchFilter.ToLower()))
                        continue;

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("  ", GUILayout.Width(10));
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), true);

                    if (GUILayout.Button("定位", GUILayout.Width(50)))
                    {
                        Selection.activeGameObject = obj;
                        EditorGUIUtility.PingObject(obj);
                    }

                    if (GUILayout.Button("复制路径", GUILayout.Width(70)))
                    {
                        EditorGUIUtility.systemCopyBuffer = GetGameObjectPath(obj);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }
        else if (targetScript != null)
        {
            EditorGUILayout.HelpBox("未找到任何引用该脚本的物体", MessageType.Info);
        }
    }

    private void SearchReferences()
    {
        if (targetScript == null) return;

        foundObjects.Clear();
        objectsByScene.Clear();

        System.Type targetType = targetScript.GetClass();
        if (targetType == null || !targetType.IsSubclassOf(typeof(MonoBehaviour)))
        {
            Debug.LogError("选择的不是有效的MonoBehaviour脚本");
            return;
        }

        // 搜索当前打开的场景
        if (searchInOpenScenes)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded)
                {
                    SearchInScene(scene, targetType);
                }
            }
        }

        // 搜索Prefab
        if (searchInPrefabs)
        {
            SearchInPrefabs(targetType);
        }

        Debug.Log($"搜索完成！共找到 {foundObjects.Count} 个挂载了 {targetType.Name} 的物体");
        Repaint();
    }

    private void SearchInScene(Scene scene, System.Type targetType)
    {
        GameObject[] rootObjects = scene.GetRootGameObjects();

        foreach (GameObject root in rootObjects)
        {
            SearchInGameObject(root, targetType, scene.name);
        }
    }

    private void SearchInGameObject(GameObject obj, System.Type targetType, string sceneName)
    {
        // 检查当前物体
        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp != null && comp.GetType() == targetType)
            {
                if (!foundObjects.Contains(obj))
                {
                    foundObjects.Add(obj);

                    if (!objectsByScene.ContainsKey(sceneName))
                        objectsByScene[sceneName] = new List<GameObject>();

                    objectsByScene[sceneName].Add(obj);
                }
                break;
            }
        }

        // 递归检查子物体
        foreach (Transform child in obj.transform)
        {
            SearchInGameObject(child.gameObject, targetType, sceneName);
        }
    }

    private void SearchInPrefabs(System.Type targetType)
    {
        // 查找所有Prefab
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                // 检查Prefab自身
                CheckPrefabForScript(prefab, targetType, path);

                // 检查Prefab内的所有子物体
                foreach (Transform child in prefab.transform)
                {
                    CheckPrefabForScript(child.gameObject, targetType, path);
                }
            }
        }
    }

    private void CheckPrefabForScript(GameObject obj, System.Type targetType, string prefabPath)
    {
        Component[] components = obj.GetComponents<Component>();
        foreach (Component comp in components)
        {
            if (comp != null && comp.GetType() == targetType)
            {
                if (!foundObjects.Contains(obj))
                {
                    foundObjects.Add(obj);

                    string sceneName = $"[Prefab] {System.IO.Path.GetFileName(prefabPath)}";
                    if (!objectsByScene.ContainsKey(sceneName))
                        objectsByScene[sceneName] = new List<GameObject>();

                    objectsByScene[sceneName].Add(obj);
                }
                break;
            }
        }
    }

    private string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform current = obj.transform.parent;

        while (current != null)
        {
            path = current.name + "/" + path;
            current = current.parent;
        }

        return path;
    }
}