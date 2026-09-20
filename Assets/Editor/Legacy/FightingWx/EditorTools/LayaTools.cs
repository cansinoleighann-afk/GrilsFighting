
using System.Collections.Generic;

using UnityEngine;

public static class  LayaTools
{
    /// <summary>
    /// 获取所有的gameobject
    /// </summary>
    /// <param name="g"></param>
    /// <param name="isHaveActive"></param>
    /// <returns></returns>
    public static List<GameObject> getAllObj(GameObject g,bool isHaveActive)
    {
        List<GameObject> alllist = new List<GameObject>();
        alllist.Add(g);
        for (int i = 0; i < g.transform.childCount; i++)
        {
            if (isHaveActive == false)
            {
                if (g.transform.GetChild(i).gameObject.activeInHierarchy == true)
                {
                    alllist.Add(g.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                alllist.Add(g.transform.GetChild(i).gameObject);
            }

                if (isHaveActive == false)
                {
                    if (g.transform.GetChild(i).gameObject.activeInHierarchy == false)
                    {
                        continue;
                    }
                }
                if (g.transform.GetChild(i).childCount > 0)
                {
                    List<GameObject> glist = GetObjAll(g.transform.GetChild(i).gameObject,isHaveActive);
                    alllist.AddRange(glist);
                }
            

        }
        return alllist;
    }

    private static List<GameObject> GetObjAll(GameObject g,bool isHaveActive)
    {
        List<GameObject> alllist = new List<GameObject>();
        for (int i = 0; i < g.transform.childCount; i++)
        {
            if (isHaveActive == false)
            {
                if (g.transform.GetChild(i).gameObject.active == true)
                {
                    alllist.Add(g.transform.GetChild(i).gameObject);
                }
            }
            else
            {
                alllist.Add(g.transform.GetChild(i).gameObject);
            }
            if (isHaveActive == false)
            {
                if (g.transform.GetChild(i).gameObject.active == false)
                {
                    continue;
                }
            }
            if (g.transform.GetChild(i).childCount > 0)
            {
                alllist.AddRange(GetObjAll(g.transform.GetChild(i).gameObject,isHaveActive));
            }
        }
        return alllist;
    }

}

public enum LayaSupportTy
{
    material=0,
    mesh=1,
    texture,
    animator,
    fbx,
}