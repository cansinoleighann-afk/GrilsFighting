
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SetAtlas : EditorWindow
{
    public string abpath = "Assets/Resources/View";
    //public GameObject objlist;
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;

    //利用构造函数来设置窗口名称
    SetAtlas()
    {
        this.titleContent = new GUIContent("设置view里image为TexturePack图集");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/设置view里image为TexturePack图集")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(SetAtlas));
    }

    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
    }

    void OnGUI()
    {
        Rect fileRect = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));

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
        if (GUILayout.Button("开始设置"))
        {
            List<string> paths= getPath();
            getPath_Sprite();
            set_atlas(paths);
        }
    }

    void set_atlas(List<string> paths)
    {
        for (int i = 0;i < paths.Count;i++)
        {
            string path = paths[i];
            GameObject prefabInstance = PrefabUtility.LoadPrefabContents(path);
            bool isChange = false;
            Tools.Instance.GetAllObj_fun(prefabInstance, (g) =>
            {
                Image image = g.GetComponent<Image>();
                if(image != null && image.sprite!=null)
                {
                    Sprite sp=null;
                    string sss = image.sprite.name;
                    if (spriteList.ContainsKey(prefabInstance.name) == true)
                    {
                        List<Sprite> sprites= spriteList[prefabInstance.name];
                        sp= sprites.Find(s=>s.name == sss);
                    }
                    if(sp==null)
                    {
                        foreach (string key in spriteList.Keys)
                        {
                            List<Sprite> sprites = spriteList[key];
                            sp = sprites.Find(s => s.name == sss);
                            if (sp != null)
                            {
                                break;
                            }
                        }
                    }
                    if (sp != null)
                    {
                        image.sprite = sp;
                        isChange = true;
                        EditorUtility.SetDirty(image);
                    }
                }
                return false;
            });
            if (isChange == true)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, path);
            }
        }
    }

    public List<string> getPath()
    {
        //string folderPath = "Assets/Resources/View";
        List<string> paths = new List<string>();

        // 查找所有Sprite资源
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { abpath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            paths.Add(path);
            Debug.Log("物体路径：：："+path);
        }
        return paths;
    }

    Dictionary<string,List<Sprite>> spriteList= new Dictionary<string,List<Sprite>>();
    public List<string> getPath_Sprite()
    {
        string folderPath = "Assets/Resources/Atlas";
        List<string> paths = new List<string>();

        // 查找所有Sprite资源
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LoadAllSpritesFromMultiple(path);
            // Resources.LoadAll<Sprite>(path);
        }
        return paths;
    }
    public Sprite[] LoadAllSpritesFromMultiple(string assetPath)
    {
        // 加载Texture2D资产
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

        if (texture == null)
        {
            Debug.LogError($"无法加载纹理: {assetPath}");
            return null;
        }

        // 获取该纹理的所有子资产（Sprite）
        Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
        Sprite[] sprites = new Sprite[assets.Length];


        string[] sss= assetPath.Split('/');
        sss= sss[sss.Length - 1].Split('.');
        string an = sss[0];
        if(spriteList.ContainsKey(an)==false)
        {
            spriteList.Add(an,new List<Sprite>());
        }
        for (int i = 0; i < assets.Length; i++)
        {
            sprites[i] = assets[i] as Sprite;
            spriteList[an].Add(sprites[i]);
        }
        Debug.Log($"从 {assetPath} 加载了 {sprites.Length} 个Sprite  ："+ an);
        return sprites;
    }
    //public Sprite getSprite(string gName,string spriteN)
    //{

    //}

    List<GameObject> get_veiw_gameobj()
    {
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