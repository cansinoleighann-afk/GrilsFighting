using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TweenScale : MonoBehaviour
{
    // Use this for initialization
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float time = 1;

    public Vector3 form = Vector3.zero;
    public Vector3 to = Vector3.one;

    public float startTime = 0;
    public TweenMore more = TweenMore.one;

    public Action<GameObject, string> Finish;

    public bool ActionRePlay = true;

    public bool isAwarkPlay = false;

    public string Data = "";
    [HideInInspector]
    public bool isPlay = false;

    public bool isSetForm = true;

    private Transform tran;

    private RectTransform tran1;

    private void Awake()
    {
        tran1 = gameObject.GetComponent<RectTransform>();

        tran = transform.GetComponent<Transform>();

    }

    void Start()
    {
        if (isSetForm == true)
        {
            if (tran1 != null)
            {
                form.x = tran1.localScale.x;
                form.y = tran1.localScale.y;
                form.z = tran1.localScale.z;
            }
            else
            {
                form.x = tran.localScale.x;
                form.y = tran.localScale.y;
                form.z = tran.localScale.z;
            }
        }
        if (gameObject.activeInHierarchy == true)
        {
            if (isAwarkPlay == true)
            {
                RePlay();
            }
        }
    }

    private void LateUpdate()
    {
        if (gameObject.activeInHierarchy == true)
        {
            if (isPlay == true)
            {
                laterT += Time.deltaTime;
                if (laterT >= startTime)
                {
                    if (more == TweenMore.one)
                    {
                        updateOne();
                    }
                    else if (more == TweenMore.loop)
                    {
                        updateLoop();
                    }
                    else if (more == TweenMore.lineOne)
                    {
                        updateLine();
                    }
                    else if (more == TweenMore.lineloop)
                    {
                        updateLineLoop();
                    }
                }

            }
        }

    }

    float startT = 0;
    void updateOne()
    {
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3();
        startT += Time.deltaTime;
        if (startT >= time)
        {
            isPlay = false;
            huan.x = to.x;
            huan.y = to.y;
            huan.z = to.z;
            setV3();
            if (Finish != null)
            {
                Finish(gameObject, Data);
            }
        }
    }


    void updateLoop()
    {
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3();
        startT += Time.deltaTime;
        if (startT >= time)
        {
            startT = 0;
            if (Finish != null)
            {
                Finish(gameObject, Data);
            }
        }
    }
    private int lineOff = 1;
    void updateLine()
    {
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3();

        startT += Time.deltaTime * lineOff;
        if (startT >= time)
        {
            lineOff = -1;
        }
        if (startT < 0)
        {
            isPlay = false;
            if (Finish != null)
            {
                Finish(gameObject, Data);
            }
        }
    }

    void updateLineLoop()
    {
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3();

        startT += Time.deltaTime * lineOff;
        if (startT >= time)
        {
            lineOff = -1;
        }
        if (startT < 0)
        {
            lineOff = 1;
            if (Finish != null)
            {
                Finish(gameObject, Data);
            }
        }
    }

    void setV3()
    {
        if (tran1 != null)
        {
            tran1.localScale = huan;
        }
        else
        {
            tran.localScale = huan;
        }
    }

    private Vector3 huan = new Vector3();


    private float laterT = 0;
    public void RePlay()
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        stop();
        laterT = 0;
        startT = 0;
        lineOff = 1;
        isPlay = true;
    }

    public void stop()
    {
        if (tran1 != null)
        {
            tran1.localScale = form;
        }
        else
        {
            tran.localScale = form;
        }
        isPlay = false;

    }

    private void OnEnable()
    {
        if (ActionRePlay == true)
        {
            RePlay();
        }
    }

}

/// <summary>
/// tween类型
/// </summary>
public enum TweenMore : byte
{
    /// <summary>
    /// 运行一次
    /// </summary>
    one = 0,

    /// <summary>
    /// 循环
    /// </summary>
    loop = 1,

    /// <summary>
    /// 反向重复
    /// </summary>
    lineloop = 2,

    /// <summary>
    /// 一次反向重复
    /// </summary>
    lineOne = 3,

}
