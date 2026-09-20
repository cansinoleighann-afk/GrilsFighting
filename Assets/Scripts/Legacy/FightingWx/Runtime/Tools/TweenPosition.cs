using UnityEngine;
using System.Collections;
using System;

public class TweenPosition : MonoBehaviour
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

    public bool isUi = false;

    private Transform tran;

    private RectTransform tran1;

    private void Awake()
    {
        tran1 = gameObject.GetComponent<RectTransform>();
        tran = transform.GetComponent<Transform>();
        if (isUi == true)
        {
            tran = null;
        }
    }

    void Start()
    {
        if (isSetForm == true)
        {
            if (isLocal == true)
            {
                if (tran1 != null)
                {
                    form.x = tran1.localPosition.x;
                    form.y = tran1.localPosition.y;
                    form.z = tran1.localPosition.z;
                }
                else
                {
                    form.x = tran.localPosition.x;
                    form.y = tran.localPosition.y;
                    form.z = tran.localPosition.z;
                }

            }
            else
            {
                if (tran1 != null)
                {
                    form.x = tran1.position.x;
                    form.y = tran1.position.y;
                    form.z = tran1.position.z;
                }
                else
                {
                    form.x = tran.position.x;
                    form.y = tran.position.y;
                    form.z = tran.position.z;
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
                laterT += 0.02f;
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
        startT += Time.deltaTime * lineOff;
        bool isOk = false;
        if (lineOff < 0)
        {
            if (startT <= 0)
            {
                startT = 0;
                isOk = true;
            }
        }
        else
        {
            if (startT >= time)
            {
                startT = time;
                isOk = true;
            }
        }
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3(isOk);
        if (isOk == true)
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
        startT += Time.deltaTime * lineOff;
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3();

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
        startT += Time.deltaTime * lineOff;
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3();
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
        startT += Time.deltaTime * lineOff;
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3();

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

    void setV3(bool ok = false)
    {
        //Debug.Log("ssss:" + Time.deltaTime+"   ttt:"+Time.deltaTime);
        if (isLocal == true)
        {
            if (tran1 != null)
            {
                if (isUi == true)
                {
                    tran1.anchoredPosition = Lerp(tran1.anchoredPosition, huan, ok);

                }
                else
                {
                    tran1.localPosition = Lerp(tran1.localPosition, huan, ok);
                }
                //tran1.localPosition = Lerp(tran1.localPosition, huan);
            }
            else
            {
                //tran.localPosition = huan;
                tran.localPosition = Lerp(tran.localPosition, huan, ok);
            }

        }
        else
        {
            if (tran1 != null)
            {
                //tran1.position = huan;
                tran1.position = Lerp(tran1.position, huan, ok);
            }
            else
            {
                //tran.position = huan;
                tran.position = Lerp(tran.position, huan, ok);

            }
        }
    }
    float lerpBili = 1f;
    Vector3 Lerp(Vector3 ori, Vector3 end, bool isOk = false)
    {
        if (isOk)
        {
            return end;
        }
        else
        {
            return Vector3.Lerp(ori, end, lerpBili * Time.deltaTime * 60f);
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
    /// <summary>
    /// 反方向播放
    /// </summary>
    public void RePlayFan()
    {
        if (gameObject.activeInHierarchy == false)
        {
            return;
        }
        laterT = 0;
        startT = time;
        lineOff = -1;
        isPlay = true;
    }

    public void stop()
    {
        if (tran1 != null)
        {
            if (isUi == false)
            {
                if (isLocal == true)
                {
                    tran1.localPosition = form;
                }
                else
                {
                    tran1.position = form;
                }
            }
            else
            {
                tran1.anchoredPosition = form;
            }
        }
        else
        {
            if (isLocal == true)
            {
                tran.localPosition = form;
            }
            else
            {
                tran.position = form;
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
