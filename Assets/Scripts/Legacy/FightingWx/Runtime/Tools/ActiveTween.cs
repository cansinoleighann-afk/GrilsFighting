
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActiveTween : MonoBehaviour
{
    public List<GameObject> obj = new List<GameObject>();

    public bool is_ramdow = true;
    public float ramdow_time = 10;
    public float ramdow_off = 5;
    public float hide_time = 2;

    public bool one_active = false;

    public MonoBehaviour call_fun;
    public string fun_name;

    float ramdow_yesT = 0;
    float ramdowT = 0;
    // Use this for initialization
    void Start()
    {
        ramdow_yesT=UnityEngine.Random.Range(ramdow_time-ramdow_off, ramdow_time + ramdow_off);
        if(one_active == true)
        {
            Tools.Instance.LaterFun(() =>
            {
                play();
            }, ramdow_yesT);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(is_ramdow==true)
        {
            
            ramdowT += Time.deltaTime;
            if (ramdowT > ramdow_yesT)
            {
                play();
            }
        }
    }

    public void play()
    {
        ramdowT = 0;
        ramdow_yesT = UnityEngine.Random.Range(ramdow_time - ramdow_off, ramdow_time + ramdow_off);
        int index = UnityEngine.Random.Range(0, obj.Count);
        for (int i = 0; i < obj.Count; i++)
        {
            obj[i].SetActive(true);
        }
        Tools.Instance.LaterFun(() => {
            for (int i = 0; i < obj.Count; i++)
            {
                if (obj[i] != null)
                {
                    obj[i].SetActive(false);
                }
            }
        }, hide_time);
        if (call_fun != null)
        {
            if (fun_name != "")
            {
                call_fun.SendMessage(fun_name);
            }
        }
    }
}