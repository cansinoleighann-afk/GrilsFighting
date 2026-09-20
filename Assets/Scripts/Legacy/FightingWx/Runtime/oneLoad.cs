using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class oneLoad : MonoBehaviour
{
    //加载完成回调
    public Action okFun;
    /// <summary>
    /// 显示加载
    /// </summary>
    public void show()
    {
        load_T = 0f;
        loadValue = 0f;
        jjj = 0f;
        jjjStr = ".";

        // The migrated prefab starts at zero scale. Restore the scene's
        // authored scale, or use one when no visible scale was authored.
        transform.localScale = visibleScale;
        RefreshVisuals();
        enabled = true;
    }
    //设置加载完成
    public void set_ok()
    {
        load_speed = loadTime / 10f;
    }
    //设置加载速度
    public void set_load_speed(float speed = 0.02f)
    {
        load_speed = speed;
        if(load_speed> loadTime / 10f){
            load_speed = loadTime / 10f;
        }
    }
    //加载进度
    public Action<float> loadValueFun;

    #region
    public Image jinDu;
    public Text loadTex;
    public float loadTime = 4;
    static oneLoad _self;
    public static oneLoad self
    {
        get
        {
            if (_self == null)
            {
                GameObject gg = ResLoad.self.loadAll("Main/oneLoad");
            }
            return _self;
        }
    }
    float loadValue = 0;
    float load_T = 0;
    float jjj = 0;
    string jjjStr = "...";
    float load_speed = 0.02f;
    Vector3 visibleScale;

    private void Awake()
    {
        _self = this;
        visibleScale = transform.localScale;
        if (visibleScale == Vector3.zero)
        {
            visibleScale = Vector3.one;
        }
        load_T = 0f;
        loadValue = 0f;
        RefreshVisuals();
        enabled = false;
    }

    private void Update()
    {
        load_T += load_speed;
        loadValue = load_T / loadTime;
        if (jinDu != null)
        {
            jinDu.fillAmount = Mathf.Clamp01(loadValue);
        }
        loadValueFun?.Invoke(loadValue);
        if (loadValue > 1)
        {
            okFun?.Invoke();
            okFun = null;
            Destroy(gameObject);
        }
        jjj += 0.02f;
        if (jjj > 1)
        {
            jjj = 0;
            jjjStr += ".";
            if (jjjStr.Length > 3)
            {
                jjjStr = ".";
            }
            if (loadTex != null)
            {
                loadTex.text = "加载中" + jjjStr;
            }
        }
    }

    private void RefreshVisuals()
    {
        if (jinDu != null)
        {
            jinDu.fillAmount = 0f;
        }

        if (loadTex != null)
        {
            loadTex.text = "加载中" + jjjStr;
        }
    }

    private void OnDestroy()
    {
        if (_self == this)
        {
            _self = null;
        }
    }
    #endregion
}
