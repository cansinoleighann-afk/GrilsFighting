using UnityEngine;
using UnityEditor;

public class EventStr 
{
    private static EventStr _instance;
    public static EventStr Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EventStr();

            }
            return _instance;
        }
    }
    EventStr()
    {
        
    }
    #region ui
    /// <summary>
    /// 显示隐藏 开始界面
    /// </summary>
    public string ShowStartView = "StartView";
    /// <summary>
    /// 显示隐藏 游戏界面
    /// </summary>
    public string ShowGameView = "GameView";
    /// <summary>
    /// 显示隐藏 加载界面
    /// </summary>
    public string ShowLoadView = "LoadView";
    /// <summary>
    /// 显示隐藏 提示界面
    /// </summary>
    public string ShowTipView = "TipView";
    /// <summary>
    /// 显示隐藏 游戏结束界面
    /// </summary>
    public string ShowGameEndView = "GameEndView";
    /// <summary>
    /// 显示隐藏 设置界面
    /// </summary>
    public string ShowSettingView = "SettingView";

    /// <summary>
    /// 显示隐藏 提示
    /// </summary>
    public string Tip = "Tip";

    /// <summary>
    /// 更新金币
    /// </summary>
    public string UpdateGold = "UpdateGold";
    /// <summary>
    /// 更新能量
    /// </summary>
    public string UpdateNeng = "UpdateNeng";

    /// <summary>
    /// 游戏赢了
    /// </summary>
    public string GameWin = "GameWin";

    /// <summary>
    /// 游戏结束
    /// </summary>
    public string GameWinCul = "GameWinCul";
    #endregion

    #region scene
    /// <summary>
    /// 显示隐藏 开始场景
    /// </summary>
    public string ShowStartScene = "StartScene";
    /// <summary>
    /// 显示隐藏 游戏场景
    /// </summary>
    public string ShowGameScene = "GameScene";

    /// <summary>
    /// 刷新 游戏中ui
    /// </summary>
    public string refreshGame = "refreshGame";
    #endregion

}