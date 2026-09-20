using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadSlider : MonoBehaviour
{
    public Text jidu;
    public float tt = 2;
    public Slider ss = null;
    // Use this for initialization
    void Start()
    {
        jidu.text = "0%";
    }
    public float v = 0;
    // Update is called once per frame
    void Update()
    {
        if (jidu != null)
        {
            if (v < 1)
            {
                if (v > 0.8)
                {
                    v += Time.deltaTime / 20;
                }
                else
                {
                    v += Time.deltaTime / tt;
                }
                ss.value = v;
                jidu.text = (v * 100).ToString("0.0") + "%";
            }
            else
            {
                v = 1;
                ss.value = v;
                jidu.text = (v * 100).ToString("0.0") + "%";
            }
        }

    }
}
