mergeInto(LibraryManager.library, {
    ssAdEvent: function (_v) {
        let v= UTF8ToString(_v)
        if(window.unity_ss_AdEvent!=null){
            window.unity_ss_AdEvent(v)
        }
    },
    ssWeb4399: function (_fun,_data) {
        var funStr = UTF8ToString(_fun);
        var dataStr = UTF8ToString(_data);
        if (dataStr != "") {
            window.h5api[funStr](dataStr);
        } else {
            window.h5api[funStr]();
        }
    },
    ssAdEventInitTT: function (_v) {
        let v = UTF8ToString(_v)          
        var uma = tt.uma     
        if(window.uma==null){
            uma.init({
              appKey:v,
              autoGetOpenid:true,
              debug:true,
              uploadUserInfo:true
            })
            window.uma=uma
        }
    },
    ssAdEventTT: function (_v) {
        let v = UTF8ToString(_v)           
        var vvv = JSON.parse(v)
        if(window.uma!=null)
        {
            var uma = tt.uma
            var evenID = vvv["evenID"]
            var ddd = {}
            for (var i in vvv) {
                if (i != "evenID" && vvv[i] != "") {
                    ddd[i] = vvv[i];
                }
            }
            var data = null
            if (Object.keys(ddd).length > 0) {  
                data = ddd
            }
            console.log("js>>::::请求成功::::抖音:::",evenID,data);
            uma.trackEvent(evenID, data)
        }
    },
    ssLogin: function (_v) {
        let v= UTF8ToString(_v)
        if(window.unity_ss_ssLogin!=null){
            window.unity_ss_ssLogin(v)
        }
    },
    ssVivoSendWeb: function (_v) {
        let v= UTF8ToString(_v)
        if(window.unity_ss_ssVivoSendWeb!=null){
            window.unity_ss_ssVivoSendWeb(v)
        }
    },
    ssQuality: function (_v) {
        if(window.unity_ss_Quality!=null){
            window.unity_ss_Quality(_v)
        }
    },
    ssSendAll: function (_v) {
        let v= UTF8ToString(_v)
        if(window.unity_ss_SendAll!=null){
            window.unity_ss_SendAll(v)
        }
    },
    InitComm: function (gameObjectName, methodName) {
        gameObjectName = UTF8ToString(gameObjectName);
        methodName = UTF8ToString(methodName);

        window["CommJS"] = {}
        window.CommJS.CommCS  = function (param, funcName)
        {
            if(window["unity_module"]==null){
                if(window.unityInstance!=null){
                    window["unity_module"]=window.unityInstance.Module
                }
            }
            if(window["unity_module"]==null){
                window["unity_module"]=window.Module
            }
            window["unity_module"].SendMessage(window.CommJS.GameObjectName, funcName, JSON.stringify(param));
        }
        window.CommJS.GameObjectName=gameObjectName;
    },
    ConnectJS: function (methodNamePath, data) {
        methodNamePath = UTF8ToString(methodNamePath);
        data = UTF8ToString(data);
        var dataObj = JSON.parse(data);
        var methodPathArr = methodNamePath.split(".");

        var returnStr = "";

        var tempMethod = window;
        for (var i = 0; i < methodPathArr.length; i++) {
            var methodName = methodPathArr[i];
            if (methodName.indexOf("()") != -1) methodName = methodName.split("()")[0]
            tempMethod = tempMethod[methodName]
            if (!tempMethod) {
                break;
            }
        }
        if (tempMethod) {
            var tempData;
            if (methodNamePath.indexOf("()") != -1) {
                tempData = tempMethod(dataObj);
            } else {
                tempData = tempMethod;
            }
            tempData = JSON.stringify(tempData)
            if (tempData) returnStr = tempData;
        } else {
            
        }

        returnStr = returnStr || "";
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;

    },
    JsCreateQuanBtn: function(left, top, width, height, unityScreenW, unityScreenH) {
        if (typeof wx === 'undefined') return;

        const sysInfo = wx.getSystemInfoSync();

        const scaleX = sysInfo.windowWidth / unityScreenW;
        const scaleY = sysInfo.windowHeight / unityScreenH;

        const style = {
            left: Math.round(left * scaleX),
            top: Math.round(top * scaleY),
            width: Math.round(width * scaleX),
            height: Math.round(height * scaleY)
        };

        if (window.myQuanBtn) {
            window.myQuanBtn.style.left = style.left;
            window.myQuanBtn.style.top = style.top;
            window.myQuanBtn.style.width = style.width;
            window.myQuanBtn.style.height = style.height;
            window.myQuanBtn.show();
            return;
        }

        window.myQuanBtn = wx.createGameClubButton({
            type: 'image',
            image: 'images/quan_btn.png',
            style: style
        });
    },

    JsHideQuanBtn: function() {
        if (window.myQuanBtn) {
            window.myQuanBtn.hide();
        }
    }
})
