using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoading : MonoBehaviour
{
    public string gameScene = "SampleScene";

    //public Loading loading = null;
    //// Use this for initialization
    //void Start()
    //{
    //    StartCoroutine(load());
    //}

    //IEnumerator load()
    //{
    //    AsyncOperation op = SceneManager.LoadSceneAsync(gameScene);
    //    while (!op.isDone)
    //    {
    //        if (loading != null)
    //        {
    //            loading.value = op.progress;
    //        }
    //        yield return null;
    //    }
    //    loading.value = 1;
    //    Debug.Log("加载完成>>>>>>>>>>>");
    //}
}