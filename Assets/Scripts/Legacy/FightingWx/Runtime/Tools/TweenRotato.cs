using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class TweenRotato : MonoBehaviour
{
    // Use this for initialization
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float time = 1;

    public Vector3 form = Vector3.zero;
    public Vector3 to = Vector3.one;

    public float startTime = 0;
    public TweenMore more = TweenMore.one;

    public Action<GameObject, string> Finish;

    public bool ActionRePlay = false;

    public bool isAwarkPlay = false;

    public string Data = "";

    public bool isLocal = true;
    [HideInInspector]
    public bool isPlay = false;

    public bool isSetForm = false;

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
            if (isLocal == true)
            {
                if (tran1 != null)
                {
                    form.x = tran1.localEulerAngles.x;
                    form.y = tran1.localEulerAngles.y;
                    form.z = tran1.localEulerAngles.z;
                }
                else
                {
                    form.x = tran.localEulerAngles.x;
                    form.y = tran.localEulerAngles.y;
                    form.z = tran.localEulerAngles.z;
                }
            }
            else
            {
                if (tran1 != null)
                {
                    form.x = tran1.eulerAngles.x;
                    form.y = tran1.eulerAngles.y;
                    form.z = tran1.eulerAngles.z;
                }
                else
                {
                    form.x = tran.eulerAngles.x;
                    form.y = tran.eulerAngles.y;
                    form.z = tran.eulerAngles.z;
                }
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
        if(lastHuan.z== -100000)
        {
            lastHuan.x = huan.x;
            lastHuan.y = huan.y;
            lastHuan.z = huan.z;
        }
        if (isLocal == true)
        {
            if (tran1 != null)
            {
                tran1.localEulerAngles = Lerp(lastHuan, huan); ;// Lerp(tran1.localEulerAngles, huan);
            }
            else
            {
                tran.localEulerAngles = Lerp(lastHuan, huan); ;// Lerp(tran.localEulerAngles, huan);
            }

        }
        else
        {
            //Debug.Log("坐标::::" + tran.eulerAngles + "  最后:" + huan);
            if (tran1 != null)
            {
                tran1.eulerAngles = Lerp(lastHuan, huan); ;// Lerp(tran1.eulerAngles, huan);
            }
            else
            {
                tran.eulerAngles = Lerp(lastHuan, huan); ;// Lerp(tran.eulerAngles, huan);
            }
            
        }
        lastHuan.x = huan.x;
        lastHuan.y = huan.y;
        lastHuan.z = huan.z;
    }
    float lerpBili = 0.6f;
    Vector3 Lerp(Vector3 ori, Vector3 end)
    {
        return Vector3.Lerp(ori, end, lerpBili * Time.deltaTime * 60f);
    }

    private Vector3 huan = new Vector3();
    private Vector3 lastHuan = new Vector3(0,0,-100000);

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
        if (isSetForm == true)
        {
            if (tran1 != null)
            {
                if (isLocal == true)
                {
                    tran1.localEulerAngles = form;
                }
                else
                {
                    tran1.eulerAngles = form;
                }
            }
            else
            {
                if (isLocal == true)
                {
                    tran.localEulerAngles = form;
                }
                else
                {
                    tran.eulerAngles = form;
                }
            }
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


