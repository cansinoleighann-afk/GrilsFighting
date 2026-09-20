using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class ViewManage
{
    public Dictionary<string, GameObject> ViewPreList = new Dictionary<string, GameObject>();

    public static ViewManage Instance = null;

    public GameObject open_anim = null;

    public GameObject Ui = null;

    public GameObject UiPre = null;

    public List<GameObject> UiList = new List<GameObject>();

    public GameObject View = null;

    public string viewPath = "View/";

    public Vector2 canvasSize;
    float _screem_rotia = 1;
    /// <summary>
    /// 屏幕的坐标/canvas坐标=screem_rotia
    /// </summary>
    public float screem_rotia
    {
        get
        {
            return _screem_rotia/ WebSdk.self.screen_bili;
        }
    }

    //是否是竖排
    public bool isPortrait = false;

    public ViewManage(GameObject _view)
    {
        if (Instance == null)
        {
            Instance = this;
            initAll(_view);
        }
    }

    public GameObject newUi(int order)
    {
        GameObject g = null;
        for (int i = 0; i < this.View.transform.childCount; i++)
        {
            Canvas ui = View.transform.GetChild(i).GetComponent<Canvas>();
            if (ui.sortingOrder == order)
            {
                g = ui.gameObject;
                break;
            }
        }

        if (g == null)
        {
            g = GameObject.Instantiate(UiPre);
            Tools.Instance.ClearObj(g);
            g.transform.SetParent(this.View.transform, false);
        }
        Canvas cv = g.GetComponent<Canvas>();
        if (order>=1000)
        {
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        cv.sortingOrder = order;
        return g;
    }

    private void initAll(GameObject _view)
    {
        if(_view== null)
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] glist = scene.GetRootGameObjects();
            for (int i = 0; i < glist.Length; i++)
            {
                if (glist[i].name == "View")
                {
                    this.View = glist[i];

                    break;
                }
            }
        }
        else
        {
            View= _view;
        }

        View.SetActive(true);
        Ui = Tools.Instance.GetGameObjectByName("Ui", View);
        Tools.Instance.ClearObj(Ui);

        UiPre = GameObject.Instantiate(Ui,View.transform.parent);
        setUiScale(UiPre.GetComponent<CanvasScaler>());
        open_anim = Tools.Instance.GetGameObjectByName("open_anim", View);


    }
    /// <summary>
    /// 自适应大小
    /// </summary>
    void setUiScale(CanvasScaler cs)
    {
        canvasSize = cs.GetComponent<RectTransform>().sizeDelta;
        _screem_rotia = Screen.width / canvasSize.x;
        Debug.Log("ui比例：：：："+_screem_rotia+"  微信比例:"+ WebSdk.self.screen_bili);
    }

    private void _LoadView(string viewname, int order, Action<GameObject> okFun)
    {
        GameObject view = null;

        GameObject uipar = newUi(order);
        uipar.SetActive(true);
        if (uipar == null)
        {
            return;
        }
        for (int i = 0; i < uipar.transform.childCount; i++)
        {
            if (uipar.transform.GetChild(i).name == viewname)
            {
                view = uipar.transform.GetChild(i).gameObject;
            }
        }
        if (view == null)
        {
            GameObject pre = null;

            if (ViewPreList.ContainsKey(viewname) == false)
            {
                //Debug.Log("ui名字:" + viewname);
                if (AssetBundleConfig.self.abList.ContainsKey(viewname) == true)
                {
                    showQuan();
                    ResLoad.self.AsyncLoadAB(AssetBundleConfig.self.abList[viewname], (pre1, arg1) =>
                    {
                        ViewPreList[viewname] = pre1;
                        view = GameObject.Instantiate(pre1);
                        view.transform.SetParent(uipar.transform, false);
                        view.name = viewname;
                        okFun(view);
                        hideQuan();
                    });
                }
                else
                {
                    pre = Resources.Load<GameObject>(this.viewPath + viewname);
                    ViewPreList[viewname] = pre;
                    view = GameObject.Instantiate(pre);
                    view.transform.SetParent(uipar.transform, false);
                    view.name = viewname;
                    okFun(view);
                }
            }
            else
            {
                pre = ViewPreList[viewname];
                view = GameObject.Instantiate(pre);
                view.transform.SetParent(uipar.transform, false);
                view.name = viewname;
                okFun(view);
            }
        }
        else
        {
            view.name = viewname;
            okFun(view);
        }
    }
    public List<GameObject> all_show_view=new List<GameObject>();
    /// <summary>
    /// 获取最上层的view
    /// </summary>
    /// <returns></returns>
    public GameObject get_show_view_top()
    {
        Canvas top_canvas = null;
        GameObject topView= null;
        int top_layer = -100;
        for (int i = 0; i < View.transform.childCount; i++)
        {
            Transform g = View.transform.GetChild(i);
            Canvas canvas= g.GetComponent<Canvas>();
            bool isHave = false;
            for (int j = 0; j < g.childCount; j++)
            {
                // 忽略常驻功能浮层的子物体
                if (AdCon.self.ignoreView.Contains(g.GetChild(j).name))
                    continue;

                if (g.GetChild(j).name!="none" && g.GetChild(j).gameObject.activeInHierarchy == true)
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave==true && canvas.sortingOrder > top_layer)
            {
                top_layer= canvas.sortingOrder;
                top_canvas= canvas;
            }
        }
        if (top_canvas != null)
        {
            for (int i = top_canvas.transform.childCount-1; i >=0; i--)
            {
                GameObject g= top_canvas.transform.GetChild (i).gameObject;
                if(g.gameObject.activeInHierarchy == true)
                {
                    topView= g;
                    break;
                }
            }
        }
        return topView;
    }
    /// <summary>
    /// 切换最上层ui的回调
    /// </summary>
    public Action<GameObject> top_view_fun;
    public void add_show_view(GameObject v=null)
    {
        all_show_view.RemoveAll(item => item == null);
        all_show_view.RemoveAll(item => item.name == "none");
        if (v != null)
        {
            all_show_view.Add(v);
            View_event scr=v.GetComponent<View_event>();
            if (scr == null)
            {
                scr=v.AddComponent<View_event>();
            }
        }
    }
    public void LoadView(string viewname, bool isTop = true, int order = 0, Action<GameObject> okFun = null)
    {
        if (ViewManage.Instance.Ui == null)
        {
            return;
        }
        _LoadView(viewname, order, (view) => {
            if (isTop == true)
            {
                view.transform.SetAsLastSibling();
            }
            view.SetActive(true);
            okFun(view);
            ResLoad.self.set_view_font(view);
            AdCon.self.view_load(view);
            add_show_view(view);
        });
    }

    public void ShowView(GameObject view, bool isTop = true, int order = 0)
    {
        GameObject uipar = newUi(order);
        if (uipar != view.transform.parent)
        {
            view.transform.SetParent(uipar.transform);
        }
        if (isTop == true)
        {
            view.transform.SetAsLastSibling();
        }
        uipar.SetActive(true);
        view.SetActive(true);
        add_show_view(view);
    }

    GameObject quan = null;
    public void showQuan(bool isMask = false)
    {
        GameObject uipar = newUi(150);
        if (quan == null)
        {
            GameObject pre = Resources.Load<GameObject>("Main/ViewQuan");
            quan = GameObject.Instantiate(pre);
            quan.transform.SetParent(uipar.transform, false);
        }
        uipar.SetActive(true);
        quan.SetActive(true);
        if (isMask == false)
        {
            Tools.Instance.GetOneByName("quan", quan).SetActive(true);
            Tools.Instance.GetOneByName("tip", quan).SetActive(true);
            Image i = quan.GetComponent<Image>();
            if (i != null)
            {
                Color c = i.color;
                c.a = 0.8f;
                i.color = c;
            }
        }
        else
        {
            Tools.Instance.GetOneByName("quan", quan).SetActive(false);
            Tools.Instance.GetOneByName("tip", quan).SetActive(false);
            Image i = quan.GetComponent<Image>();
            if (i != null)
            {
                Color c = i.color;
                c.a = 0f;
                i.color = c;
            }
        }
    }
    public void hideQuan()
    {
        if (quan != null)
        {
            quan.SetActive(false);
            GameObject.Destroy(quan);
        }
    }

    /// <summary>
    /// 预加载view
    /// </summary>
    /// <param name="plist"></param>
    /// <param name="fun"></param>
    public void preLoad(List<string> plist, Action<float, bool> fun)
    {
        int max = plist.Count;
        int pp = 0;
        for (int i = 0; i < plist.Count; i++)
        {
            ResLoad.self.asyncLoad(viewPath + plist[i], (pro, isok, g, arg) => {
                if (isok == true)
                {
                    pp++;
                    if (ViewPreList.ContainsKey(plist[i]) == false)
                    {
                        GameObject pre = Resources.Load<GameObject>(this.viewPath + arg.strList[0]);
                        ViewPreList[arg.strList[0]] = pre;
                    }
                    if (fun != null)
                    {
                        if (pp == max)
                        {
                            fun(pp, true);
                        }
                        else
                        {
                            fun(pp, false);
                        }
                    }
                }
            }, new EventArg(new List<string>() { plist[i] }, null));
        }
    }

    /// <summary>
    /// 隐藏所有view
    /// </summary>
    public void HideAll()
    {
        for (int i = 0; i < View.transform.childCount; i++)
        {
            Transform g = View.transform.GetChild(i);
            for (int j = 0; j < g.childCount; j++)
            {
                g.GetChild(j).gameObject.SetActive(false);
            }
        }
    }

    public void removeAllView()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < View.transform.childCount; i++)
        {
            Transform tt = View.transform.GetChild(i);
            for (int j = 0; j < tt.childCount; j++)
            {
                tt.GetChild(j).gameObject.name = "none";
                list.Add(tt.GetChild(j).gameObject);
            }
        }
        for (int i = 0; i < list.Count; i++)
        {
            GameObject.Destroy(list[i]);
        }
    }

    public void clearAll()
    {
        ViewPreList = null;
        ViewPreList = new Dictionary<string, GameObject>();
        Resources.UnloadUnusedAssets();
    }

    #region 获取保存ui显示状态
    class view_active
    {
        public GameObject view;
        public bool isActive = false;
        public view_active(GameObject g)
        {
            isActive = g.activeSelf;
            view = g;
        }
    }
    List<view_active> view_Actives = new List<view_active>();
    /// <summary>
    /// 隐藏所有view且保存状态
    /// </summary>
    public void hide_view_active()
    {
        view_Actives.Clear();
        for (int i = 0; i < View.transform.childCount; i++)
        {
            Transform g = View.transform.GetChild(i);
            for (int j = 0; j < g.childCount; j++)
            {
                view_Actives.Add(new view_active(g.GetChild(j).gameObject));
            }
        }
        HideAll();
    }
    /// <summary>
    /// 显示所有view
    /// </summary>
    public void show_view_active()
    {
        for (int i = 0; i < view_Actives.Count; i++)
        {
            if (view_Actives[i].view != null)
            {
                view_Actives[i].view.SetActive(view_Actives[i].isActive);
            }
        }
    }
    #endregion
}
public class View_event:MonoBehaviour
{
    private void OnEnable()
    {
        AdCon.self.view_change();
    }
    private void OnDisable()
    {
        gameObject.SetActive(false);
        AdCon.self.view_change();
    }
}
