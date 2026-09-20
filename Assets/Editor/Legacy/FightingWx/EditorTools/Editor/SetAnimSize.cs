using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SetAnimSize : EditorWindow
{
    // Start is called before the first frame update
    [SerializeField]//必须要加
    protected List<GameObject> objlist = new List<GameObject>();
    //public GameObject objlist;
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;

    public bool isAll = false;
    public bool isAllWeb = true;
    public bool isUseTxt = false;
    public string abpath = "Assets/Assetbundle/Auto";
    public string abQian = "";

    public string path = "/assetbundle/";
    //利用构造函数来设置窗口名称
    SetAnimSize()
    {
        this.titleContent = new GUIContent("减少动画资源大小");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/减少动画资源大小")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(SetAnimSize));
    }

    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
    }

    void OnGUI()
    {

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
        if (GUILayout.Button("优化动画（保存路径 YesClip）"))
        {
            List<AnimationClip> clip= SetAnimTxt();
            SetAnimPath(clip);
            AssetDatabase.Refresh();
        }
    }


    string scrPath = "/YesClip/clip.txt";
    void xie(string str)
    {
        //str = setStr(str, abList);
        byte[] byteArray = System.Text.Encoding.Default.GetBytes(str);
        using (FileStream fs = File.Create(Application.dataPath + scrPath))
        {
            fs.Write(byteArray, 0, byteArray.Length);
        }
    }


    void SetAnimPath(List<AnimationClip> animList)
    {
        for (int j = 0; j < animList.Count; j++)
        {
            Debug.Log("动画>>>>>>>>>" + animList[j]);
            AnimationClip theAnimation =AnimationClip.Instantiate( animList[j]);
            try
            {
                //去除scale曲线
                foreach (EditorCurveBinding theCurveBinding in AnimationUtility.GetCurveBindings(theAnimation))
                {
                    string name = theCurveBinding.propertyName.ToLower();
                    if (name.Contains("scale"))
                    {
                        AnimationUtility.SetEditorCurve(theAnimation, theCurveBinding, null);
                    }
                }

                //浮点数精度压缩到f3
                AnimationClipCurveData[] curves = null;
                curves = AnimationUtility.GetAllCurves(theAnimation);
                Keyframe key;
                Keyframe[] keyFrames;
                for (int ii = 0; ii < curves.Length; ++ii)
                {
                    AnimationClipCurveData curveDate = curves[ii];
                    if (curveDate.curve == null || curveDate.curve.keys == null)
                    {
                        continue;
                    }
                    
                    keyFrames = curveDate.curve.keys;//动画节点
                    for (int i = 0; i < keyFrames.Length; i++)
                    {
                        key = keyFrames[i];

                        key.value = float.Parse(key.value.ToString("f3"));
                        key.inTangent = float.Parse(key.inTangent.ToString("f3"));
                        key.outTangent = float.Parse(key.outTangent.ToString("f3"));
                        key.inWeight= float.Parse(key.inWeight.ToString("f3"));
                        key.outWeight = float.Parse(key.outWeight.ToString("f3"));
                        keyFrames[i] = key;
                    }
                    curveDate.curve.keys = keyFrames;
                    theAnimation.SetCurve(curveDate.path, curveDate.type, curveDate.propertyName, curveDate.curve);
                }
                AssetDatabase.CreateAsset(theAnimation, "Assets/YesClip/" + animList[j].name + ".anim");
            }
            catch (System.Exception e)
            {
                Debug.LogError(string.Format("CompressAnimationClip Failed !!! animationPath : {0} error: {1}", e));
            }
        }
    }

    List<AnimationClip> SetAnimTxt()
    {
        DirectoryInfo direction = new DirectoryInfo(abpath);
        FileInfo[] files = direction.GetFiles();
        Dictionary<string, string> dictxt = new Dictionary<string, string>();
        List<AnimationClip> animList = new List<AnimationClip>();
        for (int i = 0; i < files.Length; ++i)
        {
            if (!files[i].Name.Contains(".meta"))
            {
                if (files[i].Name.Contains(".anim") == true)
                {
                    string assetpath = abpath + "/" + files[i].Name;
                    AnimationClip anim = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetpath);
                    animList.Add(anim);
                }
            }
        }
        //AssetDatabase.SaveAssets()
        return animList;
    }

}