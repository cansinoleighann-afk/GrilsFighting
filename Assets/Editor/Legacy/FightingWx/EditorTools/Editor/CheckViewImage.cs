
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CheckViewImage : EditorWindow
{
    public bool is_move = false;
    public string no_move_name = "kk-";

    public string abpath = "Assets/Assetbundle/Auto";
    //public GameObject objlist;
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;

    //利用构造函数来设置窗口名称
    CheckViewImage()
    {
        this.titleContent = new GUIContent("检测ui没用的图片");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/检测ui没用的图片")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(CheckViewImage));
    }

    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
    }

    void OnGUI()
    {
        Rect fileRect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        is_move = EditorGUI.Toggle(fileRect, "是否移动多余的ui出去", is_move);
        no_move_name = EditorGUILayout.TextField("不移动的前缀", no_move_name);

        if (GUILayout.Button("开始检测"))
        {
            List<string> spriteList = GetAllSprite2DPaths();
            List<GameObject> list = get_veiw_gameobj();
            for (int i = 0; i < list.Count; i++)
            {
                GameObject g = list[i];
                List<Image> images = new List<Image>();
                Tools.Instance.GetAllObj_fun(g, (ima) =>
                {
                    Image ii = ima.GetComponent<Image>();
                    if (ii != null && ii.sprite != null)
                    {
                        images.Add(ii);
                        string ppp = AssetDatabase.GetAssetPath(ii.sprite);
                        spriteList.Remove(ppp);
                    }
                    return true;
                });
            }
            string ss = "";
            string folderPath = "Assets/Ui/noui/";

            for (int i = 0; i < spriteList.Count; i++)
            {
                string pathUi = spriteList[i];
                ss += "\n" + pathUi;
                //Debug.Log("没用的路径图：" + spriteList[i]);
                if (is_move == true)
                {
                    bool isMove = true;
                    if (no_move_name != "" && pathUi.Contains(no_move_name) == true)
                    {
                        isMove = false;
                    }
                    if (isMove == true)
                    {
                        string fileName = Path.GetFileName(pathUi);
                        string newPath = Path.Combine(folderPath, fileName);
                        int counter = 0;
                        // 自动生成唯一文件名
                        do
                        {
                            if (counter > 0)
                            {
                                newPath = Path.Combine(folderPath, counter + "__" + fileName);
                            }
                            counter++;
                        } while (AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(newPath) != null);
                        string result = AssetDatabase.MoveAsset(pathUi, newPath);
                    }
                }
            }
            Debug.Log("没用图片：" + ss);
            AssetDatabase.Refresh();
        }


        if (mouseOverWindow == this)
        {//鼠标位于当前窗口
            if (Event.current.type == EventType.DragUpdated)
            {//拖入窗口未松开鼠标
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;//改变鼠标外观
            }
            else if (Event.current.type == EventType.DragExited)
            {//拖入窗口并松开鼠标
                Focus();//获取焦点，使unity置顶(在其他窗口的前面)
                        //Rect rect=EditorGUILayout.GetControlRect();
                        //rect.Contains(Event.current.mousePosition);//可以使用鼠标位置判断进入指定区域
                if (DragAndDrop.paths != null)
                {
                    int len = DragAndDrop.paths.Length;
                    for (int i = 0; i < len; i++)
                    {
                        //Debug.Log("路径:" + DragAndDrop.paths[i]);//输出拖入的文件或文件夹路径
                        abpath = DragAndDrop.paths[i];
                    }
                }
            }
        }

        abpath = EditorGUILayout.TextField(abpath);
        if (GUILayout.Button("加前缀"))
        {
            set_name();
        }
    }

    public static List<string> GetAllSprite2DPaths()
    {
        string folderPath = "Assets/Ui";
        List<string> spritePaths = new List<string>();

        // 查找所有Sprite资源
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("/noui") == false && path.Contains("/AtlasPack") == false)
            {
                spritePaths.Add(path);
            }
        }
        return spritePaths;
    }

    List<GameObject> get_veiw_gameobj()
    {
        string abpath = "Assets/Resources/View";
        DirectoryInfo direction = new DirectoryInfo(abpath);
        FileInfo[] files = direction.GetFiles();
        List<GameObject> glist = new List<GameObject>();
        for (int i = 0; i < files.Length; ++i)
        {
            if (!files[i].Name.Contains(".meta"))
            {
                if (files[i].Name.Contains(".prefab") == true)
                {
                    string assetpath = abpath + "/" + files[i].Name;
                    GameObject mat = AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
                    glist.Add(mat);
                }
            }
        }
        return glist;
    }

    public void set_name()
    {
        // 查找所有Sprite资源
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { abpath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains(no_move_name) == false)
            {
                string currentName = System.IO.Path.GetFileNameWithoutExtension(path);
                string newName = (no_move_name + currentName).Replace("{original}", currentName);

                RenameAsset(path, newName);
                Debug.Log("路径：：：：" + path+"  :"+ currentName+"  :"+ newName);
            }
        }
        AssetDatabase.Refresh();
    }

    public void RenameAsset(string assetPath, string newName)
    {
        // 确保在编辑器环境下运行
        if (!Application.isEditor)
        {
            Debug.LogWarning("只能在编辑器模式下重命名资源");
            return;
        }

        // 获取资源的当前名称
        string currentName = System.IO.Path.GetFileNameWithoutExtension(assetPath);

        // 如果名称相同，则不需要重命名
        if (currentName == newName)
        {
            return;
        }

        // 构建新的资源路径
        string directory = System.IO.Path.GetDirectoryName(assetPath);
        string extension = System.IO.Path.GetExtension(assetPath);
        string newPath = System.IO.Path.Combine(directory, newName + extension);

        // 执行重命名
        string result = AssetDatabase.RenameAsset(assetPath, newName);

        if (!string.IsNullOrEmpty(result))
        {
            Debug.LogError("重命名失败: " + result);
        }
        else
        {
            Debug.Log($"资源重命名成功: {assetPath} -> {newPath}");
        }
    }
}