using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class TweenColor : MonoBehaviour
{

    // Use this for initialization
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public float time = 1;

    public Color form = Color.white;
    public Color to = Color.white;

    public float startTime = 0;
    public TweenMore more = TweenMore.one;

    public Action<GameObject, string> Finish;

    public bool ActionRePlay = true;

    public bool isAwarkPlay = false;

    public string Data = "";

    [HideInInspector]
    public bool isPlay = false;

    public bool isSetForm = true;

    private Image image;
    private Text text;
    private Material mat;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();

        text= gameObject.GetComponent<Text>();

        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        if (mesh != null)
        {
            mat = mesh.material;
        }
        SkinnedMeshRenderer skin= gameObject.GetComponent<SkinnedMeshRenderer>();
        if (skin != null)
        {
            mat = skin.material;
        }
    }

    void Start()
    {
        if (isSetForm == true)
        {
            if (image != null || text != null)
            {
                if (image != null)
                {
                    form.r = image.color.r;
                    form.g = image.color.g;
                    form.b = image.color.b;
                    form.a = image.color.a;
                }
                if (text != null)
                {
                    form.r = text.color.r;
                    form.g = text.color.g;
                    form.b = text.color.b;
                    form.a = text.color.a;
                }
            }
            else
            {
                form.r = mat.color.r;
                form.g = mat.color.g;
                form.b = mat.color.b;
                form.a = mat.color.a;
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

    private void Update()
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
        startT += Time.deltaTime* lineOff;
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
        huan.r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
        huan.g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
        huan.b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
        huan.a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
        setV3();
        if (isOk)
        {
            isPlay = false;
            if (lineOff > 0)
            {
                huan.r = to.r;
                huan.g = to.g;
                huan.b = to.b;
                huan.a = to.a;
            }
            else
            {
                huan.r = form.r;
                huan.g = form.g;
                huan.b = form.b;
                huan.a = form.a;
            }
            setV3();
            if (Finish != null)
            {
                Finish(gameObject, Data);
            }
        }
    }

    void updateLoop()
    {
        huan.r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
        huan.g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
        huan.b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
        huan.a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
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
        huan.r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
        huan.g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
        huan.b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
        huan.a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
        setV3();

        startT += Time.deltaTime * lineOff;
        if (startT >= time)
        {
            lineOff = -1;
        }
        if (startT < 0)
        {
            isPlay = false;
            huan.r = form.r;
            huan.g = form.g;
            huan.b = form.b;
            huan.a = form.a;
            setV3();
            if (Finish != null)
            {
                Finish(gameObject, Data);
            }
        }
    }

    void updateLineLoop()
    {
        huan.r = form.r + (curve.Evaluate(startT / time) * (to.r - form.r));
        huan.g = form.g + (curve.Evaluate(startT / time) * (to.g - form.g));
        huan.b = form.b + (curve.Evaluate(startT / time) * (to.b - form.b));
        huan.a = form.a + (curve.Evaluate(startT / time) * (to.a - form.a));
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
        if(image!=null || text != null)
        {
            if (image != null)
            {
                image.color = huan;
            }
            if (text != null)
            {
                text.color = huan;
            }
        }
        if (mat != null)
        {
            mat.color = huan;
        }
    }

    private Color huan = new Color();


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
        if (image != null || text != null)
        {
            if (image != null)
            {
                image.color = form;
            }
            if (text != null)
            {
                text.color = form;
            }
        }
        else
        {
            mat.color = form;
        }
    }

    private void OnEnable()
    {
        if (ActionRePlay == true)
        {
            RePlay();
        }
    }

    //private RectTransform tran;


}

