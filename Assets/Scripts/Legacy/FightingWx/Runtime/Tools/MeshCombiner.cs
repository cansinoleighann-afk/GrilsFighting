using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// 动态批合网格
/// </summary>
public class MeshCombiner : MonoBehaviour
{
    static List<MeshCombiner> all_str=new List<MeshCombiner>();
    public static float loadValue
    {
        get
        {
            float v= (200f-all_str.Count ) / 200f;
            if (v < 0) v = 0;
            return v;
        }
    }
    public static float mesh_set_value
    {
        get
        {
            return all_str.Count;
        }
    }
    List<MeshRenderer> meshRenderers = new List<MeshRenderer>(); // 要合并的MeshFilters数组
    void Start()
    {
        if (no_delete_string.Contains("no_") == false)
        {
            no_delete_string.Add("no_");
        }
        List<GameObject> gg = GetObjAll(gameObject);
        for (int i = 0; i < gg.Count; i++)
        {
            MeshRenderer fr = gg[i].GetComponent<MeshRenderer>();
            meshRenderers.Add(fr);
        }
        all_str.Add(this);
        start();
        //MergeMeshes();
    }
    [Header("不被删除的空节点包含名字，默认包含no_")]
    public List<string> no_delete_string = new List<string>();
    [HideInInspector]
    public List<GameObject> MeshComList = new List<GameObject>();

    void start()
    {
        if (all_str.Count > 0)
        {
            for (int i = 0;i < all_str.Count; i++)
            {
                if (all_str[i]!=null)
                {
                    all_str[i].MergeMeshes();
                    break;
                }
            }
        }
    }
    bool is_start_mesh = false;
    public void MergeMeshes()
    {
        if(is_start_mesh==true)
        {
            return;
        }
        is_start_mesh = true;
        ie_MergeMeshes = _MergeMeshes();
        MonoBehaviour mono = this;
        if(Tools.Instance.mono!=null)
        {
            mono=Tools.Instance.mono;
        }
        mono.StartCoroutine(ie_MergeMeshes);
    }
    IEnumerator ie_MergeMeshes;
    void stop_ie()
    {
        if (ie_MergeMeshes != null)
        {
            MonoBehaviour mono = this;
            if (Tools.Instance.mono != null)
            {
                mono = Tools.Instance.mono;
            }
            mono.StopCoroutine(ie_MergeMeshes);
        }
    }
    IEnumerator _MergeMeshes()
    {
        // Group mesh renderers by material
        Dictionary<Material, List<MeshFilter>> materialToMeshFilters = new Dictionary<Material, List<MeshFilter>>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material material = renderer.sharedMaterial;
            MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();

            if (meshFilter == null || meshFilter.sharedMesh == null || material == null)
            {
                Debug.LogError("空材质》》》》》" + renderer.name);
                continue;
            }

            if (!materialToMeshFilters.ContainsKey(material))
            {
                materialToMeshFilters[material] = new List<MeshFilter>();
            }
            materialToMeshFilters[material].Add(meshFilter);
        }
        // Merge meshes with the same material
        List<GameObject> re = new List<GameObject>();
        foreach (KeyValuePair<Material, List<MeshFilter>> entry in materialToMeshFilters)
        {
            Material material = entry.Key;
            List<MeshFilter> meshFilters = entry.Value;
            int meshValue = 0;
            if (meshFilters.Count > 1)
            {
                int layer = meshFilters[0].gameObject.layer;
                List<CombineInstance> combineInstances = new List<CombineInstance>();
                for (int i = 0; i < meshFilters.Count; i++)
                {
                    //combineInstances[i].mesh = meshFilters[i].sharedMesh;
                    //combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
                    CombineInstance ddd = new CombineInstance();
                    ddd.mesh = meshFilters[i].sharedMesh;
                    ddd.transform = meshFilters[i].transform.localToWorldMatrix;
                    combineInstances.Add(ddd);
                    re.Add(meshFilters[i].gameObject);
                    meshFilters[i].transform.SetParent(transform);

                    meshValue += meshFilters[i].mesh.vertexCount;
                    if (meshValue > 60000)
                    {
                        meshValue = 0;
                        mesh_join(combineInstances, material, layer);
                        combineInstances.Clear();
                        if (this != null)
                        {
                            yield return null;
                        }
                        else
                        {
                            stop_ie();
                        }
                    }
                }
                if (combineInstances.Count > 1)
                {
                    mesh_join(combineInstances, material, layer);
                    if (this != null)
                    {
                        yield return null;
                    }
                    else
                    {
                        stop_ie();
                    }
                }
            }
        }

        for (int i = 0; i < re.Count; i++)
        {
            bool isDes = isOkDes(re[i]);
            if (isDes == true)
            {
                GameObject.Destroy(re[i].gameObject);
            }
            else
            {
                Component.Destroy(re[i].GetComponent<MeshFilter>());
                Component.Destroy(re[i].GetComponent<MeshRenderer>());
            }
        }
        join_ok();
        start();
    }
    public Action okFun;
    void join_ok()
    {
        okFun?.Invoke();
        if(this!=null)
        {
            all_str.Remove(this);
        }
        all_str.RemoveAll(item => item == null);
        start();
    }

    void mesh_join(List< CombineInstance> ccc,Material material, int layer)
    {
        Mesh combinedMesh = new Mesh();
        CombineInstance[] combineInstances = new CombineInstance[ccc.Count];
        for (int i = 0;i < ccc.Count;i++)
        {
            combineInstances[i]= ccc[i];
        }
        try
        {
            combinedMesh.CombineMeshes(combineInstances, true, true);
        }
        catch (System.Exception)
        {
            Debug.LogError("超出大小材质：" + material.name);
            throw;
        }
        // Create a new GameObject to hold the combined mesh
        GameObject combinedObject = new GameObject("Combined Mesh (" + material.name + ")");
        combinedObject.layer = layer;
        combinedObject.transform.SetParent(transform);
        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer combinedMeshRenderer = combinedObject.AddComponent<MeshRenderer>();

        combinedMeshFilter.sharedMesh = combinedMesh;
        combinedMeshRenderer.sharedMaterial = material;
        MeshComList.Add(combinedObject);
    }

    /// <summary>
    /// 是否包含其他组件
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    bool isOkDes(GameObject go)
    {
        if (go == null)
        {
            return true; // 如果GameObject为空，则直接返回false
        }
        else
        {
            for (int i = 0; i < no_delete_string.Count; i++)
            {
                if (go.name.Contains(no_delete_string[i]) == true)
                {
                    return false;
                }
            }
        }
        // Transform组件总是存在的，所以我们不检查它
        // 我们可以使用Component.GetComponents()来获取所有组件的数组，然后检查数组的长度
        // 但由于Transform总是存在，我们只需要检查除Transform外的其他组件
        bool hasOtherComponent = false;
        if (go.transform.childCount == 0)
        {
            // 遍历GameObject的所有组件
            Component[] components = go.GetComponents<Component>();
            if (components.Length == 3)
            {
                int ok = 0;
                foreach (var component in components)
                {
                    // 忽略Transform组件
                    if (component.GetType() == typeof(Transform))
                    {
                        ok++;
                    }
                    if (component.GetType() == typeof(MeshFilter))
                    {
                        ok++;
                    }
                    if (component.GetType() == typeof(MeshRenderer))
                    {
                        ok++;
                    }
                }
                if (ok == 3)
                {
                    hasOtherComponent = true;
                }
            }
            // 如果没有找到除Transform外的其他组件，则返回true
        }

        return hasOtherComponent;
    }
    #region 获取所有物体

    /// <summary>
    /// 获取所有的gameobject
    /// </summary>
    /// <param name="g"></param>
    /// <param name="isHaveActive">true获取所有物体,包括隐藏的,false只获取显示的物体</param>
    /// <param name="haveStr">是否包含的字段</param>
    /// <returns></returns>
    public List<GameObject> GetObjAll(GameObject g)
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            GameObject gh = g.transform.GetChild(i).gameObject;
            if (gh.activeInHierarchy == true)
            {
                MeshRenderer fr = gh.GetComponent<MeshRenderer>();
                if (fr != null && fr.sharedMaterials.Length == 1)
                {
                    alllist.Add(gh);
                }
                if (gh.transform.childCount > 0)
                {
                    if (gh.GetComponent<MeshCombiner>() == null)
                    {
                        List<GameObject> glist = GetObjAll(gh);
                        alllist.AddRange(glist);
                    }
                }
            }
        }
        return alllist;
    }

    #endregion
    private void OnDestroy()
    {
        stop_ie();
    }
}
