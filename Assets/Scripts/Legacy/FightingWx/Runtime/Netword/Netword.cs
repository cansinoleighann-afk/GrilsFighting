using UnityEngine;

using System.Collections;
using System;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography;

public class Netword
{
    private static Netword _self;
    public static Netword self
    {
        get { if (_self == null) _self = new Netword(); return _self; }
    }

    //public string webUrl = "https://test.grid2048.com/";
    //public string webUrl = "http://192.168.50.164:8080/";
    public string webUrl = "";

    Dictionary<string, LaterNetword> laterNet = new Dictionary<string, LaterNetword>();

    public string sign = "";

    /// <summary>
    /// 签名头获取和发送的数据
    /// </summary>
    List<string> send_head_keys = new List<string>() {
        "Token"
    };
    Dictionary<string, string> send_head = new Dictionary<string, string>();
    /// <summary>
    /// 设置请求头
    /// </summary>
    /// <param name="req"></param>
    void get_head(UnityWebRequest req)
    {
        List<string> re = new List<string>();
        for (int i = 0; i < send_head_keys.Count; i++)
        {
            string ss = req.GetResponseHeader(send_head_keys[i]);
            Debug.Log("请求头数据：：：" + send_head_keys[i] + "  :" + ss);
            if (ss != "")
            {
                send_head.Add(send_head_keys[i], ss);
                re.Add(send_head_keys[i]);
            }
        }
        for (int i = 0; i < re.Count; i++)
        {
            send_head_keys.Remove(re[i]);
        }
        //"http://192.168.50.164:8081/login?code=grid_test_code" + Token + "grid"
    }

    void set_head(UnityWebRequest req, string url)
    {
        foreach (var headK in send_head.Keys)
        {
            req.SetRequestHeader(headK, send_head[headK]);
            Debug.Log("请求头设置：" + headK + "  ：" + send_head[headK]);
        }
        if (send_head.Count > 0)
        {
            string Token = send_head["Token"];
            string sss = Token + DateTime.Now.ToString() + "grid";
            string sign = CalculateMD5Hash(sss);
            sign = sign.ToLower();
            req.SetRequestHeader("Sign", sign);
            Debug.Log("请求头签名：" + sign + "  ：" + sss);
            req.SetRequestHeader("Time", DateTime.Now.ToString());
        }
    }

    /// <summary>
    /// 本方法将以Json格式向后端发送请求
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="url"></param>
    /// <param name="fun"></param>
    /// <param name="keys"></param>
    /// <param name="type"></param>
    /// <param name="originJson"></param>
    /// <param name="isNowSet">0立即发送  大于0延迟发送</param>
    /// <returns></returns>
    public IEnumerator SendRequest<T>(string url, Action<T> fun, Dictionary<string, string> keys = null, string type = "GET", Dictionary<string, string> originJson = null, float isNowSet = -1)
    {
        string ss = "";
        bool isOne = false;
        string key_yes = url;
        if (keys != null)
        {
            foreach (string key in keys.Keys)
            {
                if (isOne == false)
                {
                    isOne = true;
                    ss = key + "=" + keys[key];
                }
                else
                {
                    ss += "&" + key + "=" + keys[key];
                }
                key_yes += ";" + key;
            }
        }
        string sendUrl = webUrl + url + ss;
        if (laterNet.ContainsKey(key_yes) == false)
        {
            laterNet.Add(key_yes, new LaterNetword(sendUrl, originJson));
        }
        else
        {
            if (laterNet[key_yes].jsonData != originJson)
            {
                laterNet[key_yes].jsonData = originJson;
            }
            if (laterNet[key_yes].url != sendUrl)
            {
                laterNet[key_yes].url = sendUrl;
            }
            fun(default(T));
            yield break;
        }
        if (isNowSet > 0)
        {
            float ttt = isNowSet;
            yield return new WaitForSeconds(ttt);
        }

        // 将字符串使用UTF-8编码成字节流

        // 创建UnityWebRequest对象，用以发送请求。使用type指定请求的类型。
        string yes_send = laterNet[key_yes].url + sign;
        Dictionary<string, string> yes_data = laterNet[key_yes].jsonData;
        string jsonStr = laterNet[key_yes].json;
        byte[] postBytes = Encoding.GetEncoding("UTF-8").GetBytes(jsonStr);

        laterNet.Remove(key_yes);
        if (type == "GET")
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(new Uri(yes_send), type))
            {
                // 设置要上传的数据
                webRequest.uploadHandler = new UploadHandlerRaw(postBytes);
                // 创建后端返回数据的接收端
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                // 添加请求头。token是我们项目特有的
                //webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                // 必须要添加Content-Type请求头，明确指定以json形式传输
               webRequest.SetRequestHeader("Content-Type", "application/json");
                set_head(webRequest, yes_send);

                // 发送请求，并等待后端返回后继续调用。
                yield return webRequest.SendWebRequest();
                if (webRequest.isDone)
                {
                    string receiveContent = webRequest.downloadHandler.text;
                    get_head(webRequest);
                    //ViewManage.Instance.hideQuan();
                    if (fun != null)
                    {
                        Debug.Log("请求>>>>>>>>>>>>>" + sendUrl + "  类型:" + type + "   json:" + originJson + "     返回数据>>>>>>>" + receiveContent);
                        T info = JsonConvert.DeserializeObject<T>(receiveContent);
                        fun(info);
                    }
                }
                else
                {
                    if (fun != null)
                    {
                        fun(default(T));
                    }
                }
                //ViewManage.Instance.hideQuan();
                webRequest.Dispose();
            }
        }
        else
        {
            WWWForm form = new WWWForm();
            if(yes_data!=null && yes_data.Count > 0)
            {
                foreach (string item in yes_data.Keys)
                {
                    form.AddField(item, yes_data[item]);
                }
            }
            
            using (UnityWebRequest webRequest = UnityWebRequest.Post(new Uri(yes_send), form))
            {

                // 创建后端返回数据的接收端
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                // 添加请求头。token是我们项目特有的
                //webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                webRequest.SetRequestHeader("Content-Type", "application/json");
                //set_head(webRequest, yes_send);

                //webRequest.SetRequestHeader("Sign", "application/x-www-form-urlencoded");
                //webRequest.SetRequestHeader("t", "application/x-www-form-urlencoded");
                // 必须要添加Content-Type请求头，明确指定以json形式传输
                // 发送请求，并等待后端返回后继续调用。
                yield return webRequest.SendWebRequest();
                if (webRequest.isDone)
                {
                    string receiveContent = webRequest.downloadHandler.text;
                    get_head(webRequest);
                    Debug.Log("返回数据>>>>>>>" + receiveContent + "     请求>>>>>>>>>>>>>" + sendUrl + "  类型:" + type + "   data数据:" + jsonStr);
                    if (fun != null)
                    {
                        T info = JsonConvert.DeserializeObject<T>(receiveContent);
                        fun(info);
                    }
                }
                else
                {
                    Debug.LogError("返回数据>>>错误>>>>" + webRequest.downloadHandler.text + "     请求>>>>>>>>>>>>>" + sendUrl + "  类型:" + type + "   data数据:" + jsonStr);
                    if (fun != null)
                    {
                        fun(default(T));
                    }
                }
                //ViewManage.Instance.hideQuan();
                webRequest.Dispose();
            }
        }
    }
    public void SendRequestGetStr(string url, Action<string> fun, Dictionary<string, string> head, Dictionary<string, string> keys = null)
    {
        string ss = "";
        bool isOne = false;
        if (keys != null)
        {
            foreach (string key in keys.Keys)
            {
                if (isOne == false)
                {
                    isOne = true;
                    ss = key + "=" + keys[key];
                }
                else
                {
                    ss += "&" + key + "=" + keys[key];
                }
            }
            url += ss;
        }
        IEnumerator ie = _SendRequestGetStr(url, fun, head);
        Tools.Instance.mono.StartCoroutine(ie);
    }

    IEnumerator _SendRequestGetStr(string url, Action<string> fun, Dictionary<string, string> head)
    {
        UnityWebRequest www = UnityWebRequest.Get(new Uri(url));
        if (head != null)
        {
            foreach (var key in head.Keys)
            {
                Debug.Log("设置请求头::::::::" + url + "  :" + key + "  :" + head[key]);
                www.SetRequestHeader(key, head[key]);
            }
        }
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("请求url：" + url + "   返回:" + www.downloadHandler.text);
            fun(www.downloadHandler.text);
        }
        else
        {
            // 打印错误
            Debug.LogError(www.error);
        }
    }


    public void SendRequestPostStr(string url, Action<string> fun, string json, Dictionary<string, string> header = null)
    {
        IEnumerator ie = _SendRequestPostStr(url, fun, json, header);
        Tools.Instance.mono.StartCoroutine(ie);
    }
    public IEnumerator _SendRequestPostStr(string url, Action<string> fun, string json, Dictionary<string, string> header)
    {
        // 使用UnityWebRequest发送POST请求
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(new Uri(url), ""))
        {
            if (json != "")
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            }
            www.SetRequestHeader("Content-Type", "application/json");
            if (header != null)
            {
                foreach (string kk in header.Keys)
                {
                    www.SetRequestHeader(kk, header[kk]);
                }
            }
            // 发送请求并等待完成
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("请求返回：" + url + "   :" + www.downloadHandler.text + "  数据:" + json);
                fun?.Invoke(www.downloadHandler.text);
            }
            else
            {
                fun?.Invoke("");
                // 打印错误
                Debug.LogError("请求：" + url + " 请求的json:" + json + " 错误：" + www.error);
            }
        }
    }

    /// <summary>
    /// 转成md5
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public string CalculateMD5Hash(string input)
    {
        //Debug.Log("字符串:::::::" + input);
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = md5.ComputeHash(inputBytes);
        // step 2, convert byte array to hex string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        //Debug.Log("字符串:::md5::::" + sb);
        return sb.ToString();
    }

    public string ToBase64Str(string Str)
    {
        //Debug.Log("字符串:::::::" + Str);
        byte[] b = System.Text.Encoding.Default.GetBytes(Str);
        string bb = Convert.ToBase64String(b);
        //Debug.Log("字符串::base64:::::" + bb);
        return bb;

    }

    string wxId = "wx613212e3d497eed9";
    public void setSign(string uid, string time)
    {
        sign = "";

        if (uid != null && time != null)
        {
            string ssss = uid + wxId + time;
            string sign1 = ToBase64Str(ssss);
            string sign2 = time;
            ssss = time + uid + wxId;
            string sign3 = CalculateMD5Hash(ssss);
            sign = "&sign1=" + sign1 + "&sign2=" + sign2 + "&sign3=" + sign3 + "&gezi";
        }
    }

    public float sendT = 0.1f;
    public void update()
    {
        foreach (string key in laterNet.Keys)
        {
            LaterNetword info = laterNet[key];
            info.sendT -= 0.1f;
            if (info.sendT < 0.1f)
            {
                info.sendT = 0.1f;
            }
        }
    }
}

[Serializable]
public class NetWordInfo
{
    public int code = -1;
    public string msg;
    public Dictionary<string, string> data = new Dictionary<string, string>();

    /// <summary>
    /// 获取返回来的数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string get_key(string key)
    {
        string v = "";
        if (data != null && data.ContainsKey(key) == true)
        {
            v = data[key];
        }
        return v;
    }
}

public class LaterNetword
{
    public string url;
    public string _json = "";
    public string json
    {
        get
        {
            if (jsonData != null && jsonData.Count > 0)
            {
                return JsonConvert.SerializeObject(jsonData);
            }
            else
            {
                return _json;
            }
        }
    }
    public Dictionary<string, string> jsonData;
    public float sendT = 0;
    public LaterNetword(string _url, Dictionary<string, string> _json)
    {
        url = _url;
        jsonData = _json;
    }

    public LaterNetword(string _url,string str_json)
    {
        url = _url;
        _json = str_json;
    }
}

