using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class ScriptManage 
{
    private static ScriptManage _instance;
    public static ScriptManage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ScriptManage();

            }
            return _instance;
        }
    }
    ScriptManage()
    {

    }
    public int ScriptId = 0;
    /// <summary>
    /// 所有代码对象
    /// </summary>
    public Dictionary<string, BaseScript> AllScript = new Dictionary<string, BaseScript>();
    /// <summary>
    /// 所有事件对象
    /// </summary>
    public Dictionary<string, EventObj> messageDic = new Dictionary<string, EventObj>();
}

public class EventObj
{
    public List<Action<EventArg>> funlist=new List<Action<EventArg>>();
}

public class eventBase
{
    public eventBase(string s, Action<EventArg> f)
    {
        fun = f;
        funstr = s;
    }
    public Action<EventArg> fun = null;
    public string funstr = "";
}

public class EventArg
{
    public EventArg(List<string> str, List<GameObject> obj)
    {
        this.strList = str;
        this.objList = obj;
    }
    public List<string> strList = null;
    public List<GameObject> objList = null;
}
