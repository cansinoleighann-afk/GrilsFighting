using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Newtonsoft.Json;

/// <summary>
/// ui事件
/// </summary>
public class EventBase : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IInitializePotentialDragHandler, IDropHandler
{
    private void Awake()
    {
#if PT_wx
is_touch_err = false;
#endif
        //test_tip = Tools.Instance.GetComponentByName<Text>("tip", gameObject);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
    }


    public void OnPointerDown(PointerEventData eventData)
    {

    }


    public void OnPointerEnter(PointerEventData eventData)
    {

    }


    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }



    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrop(PointerEventData eventData)
    {

    }


    #region 微信能正常使用的
    public Action<PointerEventData> CallStartPressFun = null;
    /// <summary>
    /// 按下
    /// </summary>
    /// <param name="eventData"></param>
    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        down_time = Tools.Instance.timeToSeconds(DateTime.Now);
        down_position = eventData.position;
        data = eventData;
        if (CallStartPressFun != null)
        {
            CallStartPressFun(eventData);
        }
    }
    public Action<PointerEventData> CallFunDrag = null;
    /// <summary>
    /// 拖动中
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        data = eventData;
        if (CallFunDrag != null)
        {
            CallFunDrag(eventData);
        }
    }
    public Action<PointerEventData> RaiseTheCallback = null;
    /// <summary>
    /// 抬起
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        data = eventData;
        //test_tip.text = "正常抬起" + fingerId;
        up_btn();
    }
    void up_btn()
    {
        clear_up();
        if (RaiseTheCallback != null)
        {
            RaiseTheCallback(data);
        }
    }
    PointerEventData data = null;
    #endregion

    #region 多点触屏问题
    //Text test_tip = null;
    int down_fingerId = -1;
    int fingerId = -1;
    Touch_info_ui my_touch;
    Vector2 down_position;
    long down_time=-1;
    bool is_touch_err = true;
    void Update()
    {
        if (down_time >0 )
        {
            if (my_touch == null)
            {
                foreach (int fid in Tools.Instance.touch_list.Keys)
                {
                    Touch_info_ui info = Tools.Instance.touch_list[fid];
                    if (info.state == Touch_info_ui_state.active)
                    {
                        long off = info.down_time - down_time;
                        if (off >= 0 && off < 50)
                        {
                            if (Tools.Instance.isDic2d(down_position, info.down_position, 1))
                            {
                                down_fingerId = info.fingerId;
                                fingerId = info.fingerId;
                                my_touch = info;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                //test_tip.text = JsonConvert.SerializeObject(new Touch_info_ui_test(my_touch));
                if (my_touch.state == Touch_info_ui_state.end || my_touch.state == Touch_info_ui_state.none)
                {
                    //Debug.LogError("多点bug抬起：" + fingerId);
                    //test_tip.text = "多点bug抬起" + JsonConvert.SerializeObject(new Touch_info_ui_test(my_touch));
                    up_btn();
                }
            }

        }
    }

    void clear_up()
    {
        my_touch = null;
        down_fingerId = -1;
        fingerId= -1;
        down_time = -1;
    }
    #endregion
}

public class TouchDataUi
{
    public int TouchIndex = 0;
    public GameObject TouchObj;
    public Vector2 TouchPos = Vector2.zero;

    public bool IsUp = true;
}