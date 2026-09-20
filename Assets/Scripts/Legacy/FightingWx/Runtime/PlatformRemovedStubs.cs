// Compatibility seam for the original gameplay code.
// All advertising, social, WebGL bridge, WeChat, Google and analytics calls are intentionally no-ops.
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class AdCon
{
    public static readonly AdCon self = new AdCon();
    public readonly NoOpAdUnit bannerAd = new NoOpAdUnit();
    public readonly NoOpAdUnit interAd = new NoOpAdUnit();
    public readonly NoOpAdUnit videoAd = new NoOpAdUnit();
    public readonly NoOpGrid gezi = new NoOpGrid();
    public readonly NoOpRank rank = new NoOpRank();
    public readonly NoOpOverlay ovData = null;
    public readonly List<string> ignoreView = new List<string>();
    public Action load_json_okFun { get; set; }
    public void video(Action<bool> completed, string message = "") => completed?.Invoke(true);
    public void view_change() { }
    public void view_load(GameObject view) { }
}

public sealed class NoOpAdUnit { public bool isShowBanner; public void show(bool value = true) { } public void hide() { } }
public sealed class NoOpGrid { public void show(int index = 0) { } public void hide(int index = -1) { } }
public sealed class NoOpRank { public void open_quan(GameObject view) { } public void open_quan(RectTransform view) { } public void close_quan() { } }
public sealed class NoOpOverlay { public int zhiWanTest; public NoOpOverlayData data = new NoOpOverlayData(); public void show_event(string eventName) { } }
public sealed class NoOpOverlayData { }

// The source View prefabs were intentionally removed. These types retain only
// the old call signatures so gameplay code can continue without presentation,
// advertising, or platform behavior.
public static class WXAdView { public static void show_view(int type = 0) { } }
public sealed class SignView : MonoBehaviour
{
    public static SignView self => null;
    public static Action fun;
    public GameObject m_par;
    public GameObject m_tip1;
    public static void ShowView(bool show, int layer = 0) { }
    public static void HideView() { }
    public static void DestroyView() { }
    public void setTip(GameObject target, string text, float offsetX, float offsetY, int type) { }
}
public static class TipView
{
    public static float t;
    public static string tip;
    public static int ty;
    public static void ShowView(bool show, int layer = 0) { }
    public static void DestroyView() { }
}

public sealed class WebSdk
{
    public static readonly WebSdk self = new WebSdk();
    public float screen_bili = 1f;
    public GameObject gameObject => null;
    public readonly Dictionary<string, object> wxTestValue = new Dictionary<string, object>();
    public string ConnectJS(string methodNamePath, object data = null) => string.Empty;
    public void login(string value, Action completed, Action feedCompleted = null) => completed?.Invoke();
    public void send_AddEvent(string eventName, Dictionary<string, string> data = null) { }
    public void send_ssSendAll(string value, Action<string, string> completed = null) => completed?.Invoke(string.Empty, string.Empty);
}

// Retained solely for the legacy editor platform selector. It does not enable
// any SDK, advertising, analytics, WebGL bridge, or mobile platform behavior.
public enum PlatformTy
{
    wx,
    tt,
    vivo,
    oppo,
    android,
    ios,
    web,
    qq,
    mi,
    ry,
    hw,
    ks,
    mini_4399,
    web_4399
}
