using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class SetShadow : EditorWindow
{
    // Start is called before the first frame update
    [SerializeField]//必须要加
    protected List<GameObject> Gameobjectlist = new List<GameObject>();
    //序列化对象
    protected SerializedObject _serializedObject;
    //序列化属性
    protected SerializedProperty _assetLstProperty;


    //是否有激活的active
    public bool isHaveActive = true;

    public bool isOpen = true;

    //利用构造函数来设置窗口名称
    SetShadow()
    {
        this.titleContent = new GUIContent("设置阴影");
    }

    //添加菜单栏用于打开窗口
    [MenuItem("Tools/设置阴影")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(SetShadow));
    }
    protected void OnEnable()
    {
        //使用当前类初始化
        _serializedObject = new SerializedObject(this);
        //获取当前类中可序列话的属性
        _assetLstProperty = _serializedObject.FindProperty("Gameobjectlist");
    }
    void OnGUI()
    {

        Rect fileRect2 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isHaveActive = EditorGUI.Toggle(fileRect2, "是否接受阴影", isHaveActive);

        Rect fileRect1 = EditorGUILayout.GetControlRect(GUILayout.Width(position.width - 25));
        isOpen = EditorGUI.Toggle(fileRect1, "是否开关阴影", isOpen);

        _serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        //绘制对象
        //GUILayout.Space(10);
        //objlist = (GameObject)EditorGUILayout.ObjectField("Buggy Game Object", objlist, typeof(GameObject), true);
        EditorGUILayout.PropertyField(_assetLstProperty, true);

        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {//提交修改
            _serializedObject.ApplyModifiedProperties();
        }

        if (GUILayout.Button("开始设置"))
        {
            for (int i = 0; i < Gameobjectlist.Count; i++)
            {
                List<GameObject> ggg = Tools.Instance.GetAllObj_fun(Gameobjectlist[i], (gg) =>
                {
                    return true;
                });
                for (int j = 0; j < ggg.Count; j++)
                {
                    setShadow(ggg[j], isHaveActive);
                }
            }
        }
    }

    public void setShadow(GameObject g,bool ishaveActive)
    {
        MeshRenderer mesh= g.GetComponent<MeshRenderer>();
        if(mesh != null)
        {
            mesh.receiveShadows = ishaveActive;
            if (isOpen==true)
            {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;//开
            }
            else
            {
                mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//关
            }
            Debug.Log("修改：：："+mesh.name);
        }
        SkinnedMeshRenderer skin = g.GetComponent<SkinnedMeshRenderer>();
        if (skin != null)
        {
            skin.receiveShadows = ishaveActive;
            if (isOpen == true)
            {
                skin.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;//开
            }
            else
            {
                skin.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;//关
            }
            Debug.Log("修改：：：" + skin.name);
        }
    }

}
