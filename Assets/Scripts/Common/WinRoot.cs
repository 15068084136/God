using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WinRoot : MonoBehaviour
{
    protected ResService resService = null;
    protected AudioServer audioServer = null;
    protected NetService netService = null;
    protected TimerService timerService = null;

    // 设置界面的开与关
    public void SetWinState(bool active = true){
        if(gameObject.activeSelf != active){
            SetActive(gameObject, active);
        }
        if(active){
            InitWin();
        }
        else{
            ClearWin();
        }
    }

    protected virtual void InitWin(){
        resService = ResService.instance;
        audioServer = AudioServer.instance;
        netService = NetService.instance;
        timerService = TimerService.Instance;
    }

    protected virtual void ClearWin(){
        resService = null;
        audioServer = null;
        netService = null;
        timerService = null;
    }

    #region Tool Functions
    protected void SetText(Text txt, string content){
        txt.text = content;
    }
    protected void SetText(Transform transform, int num){
        SetText(transform.GetComponent<Text>(), num);
    }
    protected void SetText(Transform transform, string content){
        SetText(transform.GetComponent<Text>(), content);
    }
    protected void SetText(Text txt, int num){
        SetText(txt, num.ToString());
    }

    protected void SetActive(GameObject obj, bool active = true){
        obj.SetActive(active);
    }
    protected void SetActive(Transform transform, bool active = true){
        transform.gameObject.SetActive(active);
    }
    protected void SetActive(RectTransform rectTransform, bool active = true){
        rectTransform.gameObject.SetActive(active);
    }
    protected void SetActive(Image image, bool active = true){
        image.gameObject.SetActive(active);
    }
    protected void SetActive(Text text, bool active = true){
        text.gameObject.SetActive(active);
    }
    // 添加组件
    protected T IsAddComponent<T>(GameObject go) where T : Component{
        T t = go.GetComponent<T>();
        if(t == null){
            t = go.AddComponent<T>();
        }
        return t;
    }
    // 改变图片
    protected void SetSprite(Image image, string path){
        Sprite sp = resService.LoadSprite(path, true);
        image.sprite = sp;
    }
    // 获得子物体
    protected Transform GetTrans(Transform trans, string name){
        if(trans!=null){
            return trans.Find(name);
        }else{
            return transform.Find(name);
        }
    }
    #endregion

    #region 点击事件
    protected void OnClick(GameObject go, Action<object> cb, object args){
        PEListener listener = IsAddComponent<PEListener>(go);
        listener.onClick = cb;
        listener.args = args;
    }
    protected void OnClickDown(GameObject go, Action<PointerEventData> cb){
        PEListener listener = IsAddComponent<PEListener>(go);
        listener.onClickDown = cb;
    }
    protected void OnClickUp(GameObject go, Action<PointerEventData> cb){
        PEListener listener = IsAddComponent<PEListener>(go);
        listener.onClickUp = cb;
    }
    protected void OnDrag(GameObject go, Action<PointerEventData> cb){
        PEListener listener = IsAddComponent<PEListener>(go);
        listener.onDrag = cb;
    }
    #endregion
}
