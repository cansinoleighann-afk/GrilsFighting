using UnityEngine;
using System.Collections;
using System;

public class TweenCurvePos : MonoBehaviour
{
    [Header("移动变化")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Header("曲线变化")]
    public AnimationCurve curveDir = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Header("曲线变化方向")]
    public Vector3 curve_off_dir = Vector3.up;

    public TweenMore more = TweenMore.one;

    public float time = 1;

    public Vector3 form = Vector3.zero;
    public Vector3 to = Vector3.one;

    public Action<string> Finish;

    public bool isAwarkPlay = false;

    public string Data = "";

    public bool isLocal = true;
    [HideInInspector]
    public bool isPlay = false;

    private void Awake()
    {

    }

    void Start()
    {
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

    float startT = 0;
    void updateOne()
    {
        startT += 0.018f * lineOff;
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
        setV3(isOk, startT);
        if (isOk == true)
        {
            isPlay = false;
            if (Finish != null)
            {
                Finish( Data);
            }
        }
    }

    void updateLoop()
    {
        startT += Time.deltaTime * lineOff;
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3(false,startT);

        if (startT >= time)
        {
            startT = 0;
            if (Finish != null)
            {
                Finish( Data);
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
        setV3(false, startT);
        if (startT >= time)
        {
            lineOff = -1;
        }
        if (startT < 0)
        {
            isPlay = false;
            if (Finish != null)
            {
                Finish( Data);
            }
        }
    }

    void updateLineLoop()
    {
        startT += Time.deltaTime * lineOff;
        huan.x = form.x + (curve.Evaluate(startT / time) * (to.x - form.x));
        huan.y = form.y + (curve.Evaluate(startT / time) * (to.y - form.y));
        huan.z = form.z + (curve.Evaluate(startT / time) * (to.z - form.z));
        setV3(false, startT);

        if (startT >= time)
        {
            lineOff = -1;
        }
        if (startT < 0)
        {
            lineOff = 1;
            if (Finish != null)
            {
                Finish( Data);
            }
        }
    }

    void setV3(bool ok = false,float _tt=0)
    {
        _tt = _tt / time;
        _tt *= 2;
        if (_tt > 1)
        {
            _tt = 2 - _tt;
        }
        Vector3 offDir = curveDir.Evaluate(_tt) * curve_off_dir;
        //Debug.Log("曲线变化：：：：" + _tt + "   :" + offDir);
        if (isLocal == true)
        {
            transform.localPosition = Lerp(transform.localPosition, huan+ offDir, ok);
        }
        else
        {
            transform.position = Lerp(transform.position, huan+ offDir, ok);
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
        if (isLocal == true)
        {
            transform.localPosition = form;
        }
        else
        {
            transform.position = form;
        }
        isPlay = false;
    }
}
