using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManage : MonoBehaviour
{
    static MusicManage _Instance;
    public static MusicManage Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject aaa = new GameObject("MusicManage");
                _Instance = aaa.AddComponent<MusicManage>();
                aaa.AddComponent<AudioListener>();
                DontDestroyOnLoad(aaa);
            }
            return _Instance;
        }
        set
        {
            _Instance = value;
        }
    }

    public string pc_music_path = "Audio/";

    private void Awake()
    {
        Instance = this;
        int ii = DataTools.getInt("ismusic");
        if (ii == 0)
        {
            DataTools.save("audioValue", 1f);
            DataTools.save("musicValue", 1f);
            audioValue = 1;
            musicValue = 1;
            DataTools.save("ismusic", 1);
        }
        else
        {
            _audioValue = DataTools.getFloat("audioValue");
            _musicValue = DataTools.getFloat("musicValue");
        }
        _isZhen = DataTools.getInt("isZhen");
    }

    #region 设置声音大小
    float _musicValue = 2;
    public float musicValue
    {
        get
        {
            return _musicValue;
        }
        set
        {
            if (value >= 1)
            {
                value = 1;
                if (bgMusic != null)
                {
                    bgMusic.value = 1;
                    bgMusic.play(false, true);
                }
            }
            if (value <= 0)
            {
                value = 0;
                if (bgMusic != null)
                {
                    bgMusic.value = 0;
                    bgMusic.stop();
                }
            }
            value = Mathf.Round(value * 10f) / 10f;
            if (_musicValue != value)
            {
                _musicValue = value;
                DataTools.save("musicValue", _musicValue);
                if (bgMusic != null)
                {
                    if (bgMusic.au != null)
                    {
                        bgMusic.au.volume = value;
                    }
                }
            }
        }
    }

    float _audioValue = 2;
    public float audioValue
    {
        get
        {
            return _audioValue;
        }
        set
        {
            if (value > 1)
            {
                value = 1;
            }
            if (value < 0)
            {
                value = 0;
                foreach (AudioItem item in Allmusic.Values)
                {
                    if (item != null)
                    {
                        item.stop();
                    }
                }
            }

            value = Mathf.Round(value * 10f) / 10f;
            if (_audioValue != value)
            {
                _audioValue = value;
                DataTools.save("audioValue", _audioValue);
            }
        }
    }

    int _isZhen = 0;
    public bool isZhen
    {
        get
        {
            if (_isZhen == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        set
        {
            int i = 0;
            if (value == false)
            {
                i = 1;
            }
            _isZhen = i;
            DataTools.save("isZhen", i);
        }
    }
    #endregion

    /// <summary>
    /// 预加载声音地址
    /// </summary>
    public static Dictionary<string, string> music = new Dictionary<string, string>()
    {
    };

    public int MusicId = 0;
    /// <summary>
    /// 所有声音对象
    /// </summary>
    public Dictionary<string, AudioItem> Allmusic = new Dictionary<string, AudioItem>();

    /// <summary>
    /// resource加载的声音对象
    /// </summary>
    public Dictionary<string, AudioClip> AllClip = new Dictionary<string, AudioClip>();

    #region 震动
    public void zhen()
    {
        if (_isZhen == 0)
        {
#if PT_wx
            WeChatWASM.VibrateShortOption duan = new WeChatWASM.VibrateShortOption();
            WeChatWASM.WX.VibrateShort(duan);
#endif

#if PT_tt
#if !UNITY_EDITOR   
            TTSDK.VibrateShortParam vibrateShortParam = new TTSDK.VibrateShortParam();
            TTSDK.TT.VibrateShort(vibrateShortParam);
#endif
#endif

#if PT_vivo
            WebSdk.self.ConnectJS("qg.vibrateShort()");
#endif

#if PT_oppo
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                QGMiniGame.QG.VibrateShort();
            }
#endif
#if PT_qq
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("fun", "vibrateShort");
            WebSdk.self.send_ssSendAll(JsonConvert.SerializeObject(dic));
#endif
        }
    }
    public void zhen_big()
    {
        if (_isZhen == 0)
        {
#if PT_wx
            WeChatWASM.VibrateLongOption duan = new WeChatWASM.VibrateLongOption();
            WeChatWASM.WX.VibrateLong(duan);
#endif
#if PT_vivo
            WebSdk.self.ConnectJS("qg.vibrateLong()");
#endif
#if PT_tt
#if !UNITY_EDITOR
            TTSDK.VibrateLongParam vibrateShortParam = new TTSDK.VibrateLongParam();
            TTSDK.TT.VibrateLong(vibrateShortParam);
#endif
#endif
#if PT_oppo
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                QGMiniGame.QG.VibrateShort();
            }
#endif
        }
    }
    #endregion

    #region 播放声音
    /// <summary>
    /// 播放
    /// </summary>
    /// <param name="ty"></param>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    /// <returns></returns>
    public AudioItem play(string key, bool isLoop = false, bool isPlayDes = true)
    {
        //return new AudioItem(null,"",false, MusicType.music);
        //if (audioValue <= 0)
        //{
        //    return null;
        //}
        bool isQQ = false;
#if PT_qq
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            isQQ = true;
        }
#endif
        if (isQQ == false)
        {
            return _playU(key, isLoop, isPlayDes);
        }
        else
        {
            return new AudioItem(key, isLoop, isPlayDes);
        }
    }

    /// <summary>
    /// 电脑播放音频
    /// </summary>
    /// <param name="ty"></param>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    /// <returns></returns>
    AudioItem _playU(string nnn, bool isLoop, bool isPlayDes)
    {

        MusicId++;
        string musicstr = "music_" + MusicId;
        AudioClip clip = getClip(nnn);

        GameObject g = getMusicObj();
        g.name = nnn;
        AudioItem gm = new AudioItem(g, nnn, isLoop);
        gm.data = musicstr;
        AudioSource au = gm.au;
        au.clip = clip;
        au.loop = isLoop;
        au.volume = _audioValue;
        if(_audioValue > 0)
        {
            au.Play();
        }
        if (clip != null)
        {
            float t = clip.length;
            if (isPlayDes == true)
            {
                Tools.Instance.LaterFun(() =>
                {
                    GameObject.Destroy(g);
                    clearAllHuan();
                }, t + 0.2f);
            }
        }

        Allmusic.Add(musicstr, gm);
        //Debug.Log("声音>>>>>>>>>>>>" + Allmusic.Count);
        return gm;
    }


    /// <summary>
    /// 循环播放,每次循环完后才能停止播放
    /// </summary>
    /// <param name="key"></param>
    /// <param name="isLoop"></param>
    /// <param name="isPlayDes"></param>
    /// <returns></returns>
    public AudioItem playLookOk(string key)
    {
        //return new AudioItem(null,"",false, MusicType.music);
        if (audioValue <= 0)
        {
            return new AudioItem(null, "", false);
        }
        //Debug.Log("播放音效>>>>>>>>>>>>>>>>>>>>>:"+key);

#if UNITY_WEBGL && !UNITY_EDITOR
    return _playULook(key);
#else
        return _playULook(key);
#endif
        //return _playWX(name, isLoop, isPlayDes);

    }

    /// <summary>
    /// 电脑播放音频
    /// </summary>
    /// <param name="ty"></param>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    /// <returns></returns>
    AudioItem _playULook(string nnnn)
    {
        MusicId++;
        string musicstr = "music_" + MusicId;
        string path = "";
        path = "Audio/";
        string nn = nnnn;
        nn = nnnn.Split('.')[0];
        path = path + nn;
        AudioClip clip = getClip(nnnn);

        GameObject g = getMusicObj();
        AudioItem gm = new AudioItem(g, nnnn, false, MusicType.audio, true);
        gm.data = musicstr;
        AudioSource au = gm.au;
        au.clip = clip;
        au.loop = false;
        au.volume = _audioValue;
        au.Play();
        Allmusic.Add(musicstr, gm);
        gm.play();
        //Debug.Log("声音>>>>>>>>>>>>" + Allmusic.Count);
        return gm;
    }


    /// <summary>
    /// 背景音乐对象
    /// </summary>
    public AudioItem bgMusic = null;

    public void playBG(string bg)
    {
        bool isQQ = false;
#if PT_qq
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            isQQ = true;
        }
#endif
        if (isQQ)
        {
            if (bgMusic != null)
            {
                bgMusic.stop();
            }
            bgMusic = new AudioItem(bg, true, false);
        }
        else
        {
            _playBGU(bg);
        }
    }
    AudioItem _playBGU(string bg)
    {
        AudioItem gm = bgMusic;
        if (bgMusic == null)
        {
            MusicId++;
            //Debug.Log("音乐id：1：" + MusicId);
            string musicstr = "music_" + MusicId;
            GameObject g = getMusicObj();
            gm = new AudioItem(g, bg, true, MusicType.music);
            gm.data = musicstr;
            bgMusic = gm;
            Allmusic.Add(musicstr, gm);
        }

        string nn = bg;
        nn = bg.Split('.')[0];
        string path = "Audio/" + nn;
        AudioSource au = gm.au;
        au.loop = true;
        AudioClip clip = getClip(bg);
        au.clip = clip;
        au.volume = _musicValue;

        au.Play();
        return gm;
    }

    #endregion

    #region 电脑声音处理

    AudioClip getClip(string path)
    {
        AudioClip clip = null;
        if (AllClip.ContainsKey(path) == true && AllClip[path] != null)
        {
            clip = AllClip[path];
        }
        else
        {
            if (music.ContainsKey(path) == true)
            {
                clip = ResLoad.self.LoadAudio(music[path]);// 

            }
            else
            {
                clip = Resources.Load<AudioClip>("Audio/" + path);
            }
            AllClip.Add(path, clip);
            List<string> kk = new List<string>();
            foreach (string key in AllClip.Keys)
            {
                if (AllClip[key] == null)
                {
                    kk.Add(key);
                }
            }
            for (int i = 0; i < kk.Count; i++)
            {
                AllClip.Remove(kk[i]);
            }
        }
        return clip;
    }

    /// <summary>
    /// 获取音效对象
    /// </summary>
    /// <returns></returns>
    GameObject getMusicObj()
    {
        GameObject g = null;
        clearAllHuan();
        if (g == null)
        {
            g = new GameObject();
            g.transform.parent = transform;
            if (g.GetComponent<AudioSource>() == null)
            {
                AudioSource au = g.AddComponent<AudioSource>();
            }
        }

        return g;
    }

    /// <summary>
    /// 清理缓存引用
    /// </summary>
    public void clearAllHuan()
    {
        List<string> re = new List<string>();
        foreach (string key in Allmusic.Keys)
        {
            if (Allmusic[key].g == null)
            {
                re.Add(key);
            }
        }
        for (int i = 0; i < re.Count; i++)
        {
            Allmusic.Remove(re[i]);
        }
        //Debug.Log("声音对象数目::::::" + Allmusic.Count);
    }

    /// <summary>
    /// 清理所有的音效
    /// </summary>
    public void clearAllAudio()
    {
        List<GameObject> gg = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
            bool isAdd = true;
            if (bgMusic != null && g == bgMusic.g)
            {
                isAdd = false;
            }
            if (isAdd == true)
            {
                gg.Add(g);
            }
        }
        for (int i = 0; i < gg.Count; i++)
        {
            GameObject.Destroy(gg[i]);
        }
        clearAllHuan();
    }
    /// <summary>
    /// 移除声音
    /// </summary>
    /// <param name="item"></param>
    public void removeAudio(AudioItem item)
    {
        if (item != null)
        {
            item.clear();
            item = null;
        }
        MusicManage.Instance.clearAllHuan();
    }
    #endregion
}

public class AudioItem
{
    string _data = "";
    public string data
    {
        get
        {
            return _data;
        }
        set
        {
            g.name = value;
            _data = value;
        }
    }
    public GameObject g;
    public AudioSource au;
    public string url;
    public bool isLoop;
    public MusicType auty;
    public bool isDes = false;
    public bool isLost = false;
    public bool isOkCanStop = false;
    bool isLoopStop = false;
    public float oriValue = 0;

    public AudioItem(GameObject _g, string _url, bool _isloop, MusicType _auty = MusicType.audio, bool _isOkCanStop = false)
    {
        auty = _auty;
        url = _url;
        isLoop = _isloop;
        g = _g;
        isOkCanStop = _isOkCanStop;
        if (_g != null)
        {
            au = _g.GetComponent<AudioSource>();
        }
        if (au != null)
        {
            _value = au.volume;
            oriValue = _value;
        }
    }

    public int qqAudioId = -1;
    public AudioItem(string _url, bool _isloop, bool _isPlayDec)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("audio", _url);
        dic.Add("audioFun", "create");
        dic.Add("loop", _isloop);
        int isD = 0;
        if (_isPlayDec)
        {
            isD = 1;
        }
        dic.Add("isDec", isD);
        WebSdk.self.send_ssSendAll(JsonConvert.SerializeObject(dic), (uid, data) =>
        {
            Debug.Log("声音id：：：：：：：" + uid + " :" + data);
            qqAudioId = int.Parse(uid);
        });
    }

    void Uloop(float t)
    {
        IEUloop = IELaterFun(() =>
        {
            if (isLoopStop == false && au != null)
            {
                au.Play();
                Uloop(t);
            }
        }, t);
        Tools.Instance.mono.StartCoroutine(IEUloop);
    }
    IEnumerator IEUloop = null;
    IEnumerator IELaterFun(Action fun, float t)
    {
        yield return new WaitForSeconds(t);
        fun();
    }


    public void play(bool isOriValue = false, bool isYesPlay = false)
    {
        if (url != "")
        {
            if (IEstop != null)
            {
                Tools.Instance.mono.StopCoroutine(IEstop);
            }
            bool isQQ = false;
#if PT_qq
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                isQQ = true;
            }
#endif
            if (isQQ == false)
            {
                bool isP = true;
                if (isYesPlay == false)
                {
                    if(this != MusicManage.Instance.bgMusic)
                    {
                        if(MusicManage.Instance.audioValue <= 0)
                        {
                            isP = false;
                        }
                    }
                    else
                    {
                        if (MusicManage.Instance.musicValue <= 0)
                        {
                            isP = false;
                        }
                    }
                }
                if (isP == true)
                {
                    if (au != null)
                    {
                        if (isOriValue == true)
                        {
                            value = oriValue;
                        }
                        if (isOkCanStop == false)
                        {
                            if (_value > 0)
                            {
                                au.Play();
                            }
                        }
                        else
                        {
                            if (_value > 0)
                            {
                                isLoopStop = false;
                                if (IEUloop != null)
                                {
                                    Tools.Instance.mono.StopCoroutine(IEUloop);
                                }
                                au.Play();
                                Uloop(au.clip.length);
                            }
                        }
                    }
                    else
                    {
                        //Debug.Log("声音已删除>>>>>>>" + url);
                        isLost = true;
                    }
                }
            }
            else
            {
                if (MusicManage.Instance.audioValue > 0)
                {
                    isYesPlay = true;
                }
                if (isYesPlay == true)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("audio", qqAudioId);
                    dic.Add("audioFun", "play");
                    WebSdk.self.send_ssSendAll(JsonConvert.SerializeObject(dic));
                }

            }
        }

    }
    public void playLerp(float t=0.1f,float valueOri=-1)
    {
        if (valueOri >= 0)
        {
            value = valueOri;
        }
        play();
        if (IEstop != null)
        {
            Tools.Instance.mono.StopCoroutine(IEstop);
            IEstop = null;
        }
        if (IEPlay != null)
        {
            Tools.Instance.mono.StopCoroutine(IEPlay);
            IEPlay = null;
        }
        IEPlay = IEPlayFun(t);
        Tools.Instance.mono.StartCoroutine(IEPlay);
    }
    IEnumerator IEPlay = null;
    IEnumerator IEPlayFun(float t)
    {
        while (value < 1)
        {
            if (this != null)
            {
                value += t;
                if (value >= 1)
                {
                    break;
                }
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    public void stop()
    {
        bool isQQ = false;
#if PT_qq
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            isQQ = true;
        }
#endif
        if (isQQ == false)
        {
            if (url != "")
            {
                if (au != null)
                {
                    if (isOkCanStop == false)
                    {
                        au.Stop();
                    }
                    else
                    {
                        isLoopStop = true;
                        if (IEUloop != null)
                        {
                            Tools.Instance.mono.StopCoroutine(IEUloop);
                        }
                    }
                }
            }
        }
        else
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("audio", qqAudioId);
            dic.Add("audioFun", "stop");
            WebSdk.self.send_ssSendAll(JsonConvert.SerializeObject(dic));
        }
    }
    public void clear()
    {
        if (g != null)
        {
            GameObject.Destroy(g);
        }
#if PT_qq
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("audio", qqAudioId);
            dic.Add("audioFun", "destroy");
            WebSdk.self.send_ssSendAll(JsonConvert.SerializeObject(dic));
        }
#endif
    }
    public void stopLerp(float t = 0.1f)
    {
        bool isQQ = false;
#if PT_qq
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            isQQ = true;
        }
#endif
        if (isQQ == false)
        {
            if (IEstop != null)
            {
                Tools.Instance.mono.StopCoroutine(IEstop);
                IEstop = null;
            }
            if (IEPlay != null)
            {
                Tools.Instance.mono.StopCoroutine(IEPlay);
                IEPlay = null;
            }
            IEstop = IEstopFun(t);
            Tools.Instance.mono.StartCoroutine(IEstop);
        }
        else
        {
            stop();
        }

    }
    IEnumerator IEstop = null;
    public Action stopLerp_okFun;
    IEnumerator IEstopFun(float t)
    {
        while (value > 0)
        {
            if (this!=null)
            {
                value -= t;
                if (value <= 0)
                {
                    stop();
                    Debug.Log("停止声音》》》》" + value + "   :" + stopLerp_okFun);
                    stopLerp_okFun?.Invoke();
                }
            }
            else
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    float _value = 1;
    public float value
    {
        get
        {
            return _value;
        }
        set
        {
            if (value >= 1)
            {
                value = 1;
                play();
            }
            if (value <= 0)
            {
                value = 0;
                stop();
            }
            if (au != null)
            {
                au.volume = value;
            }
            _value = value;
        }
    }
    public float au_vlaue
    {
        get
        {
            return _value;
        }
        set
        {
            if(MusicManage.Instance.audioValue<=0)
            {
                value = 0;
            }
            if (value >= 1)
            {
                value = 1;
            }else if (value <= 0)
            {
                value = 0;
            }
            if (au != null)
            {
                au.volume = value;
                _value = value;
            }
        }
    }
}

public enum MusicType
{
    path = 0,
    music = 1,
    audio = 2,
}
