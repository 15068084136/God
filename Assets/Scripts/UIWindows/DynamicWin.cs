using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DynamicWin : WinRoot
{
    public Animation anim;
    public Text tips;
    // 是否正在播放
    private bool isTips;
    public Transform hpItemRoot;
    public Animation selfDodge;

    // 怪兽的名字是key
    private Dictionary<string, HpItem> itemDic = new Dictionary<string, HpItem>();

    // 提示队列
    private Queue<string> tipsQue = new Queue<string>();

    protected override void InitWin()
    {
        base.InitWin();

        SetActive(tips, false);
    }

    /// <summary>
    /// 将提示加入队列中
    /// </summary>
    /// <param name="tips"></param>
    public void AddTips(string tips){
        // 可能在多个线程里，所以加了个锁
        lock(tipsQue){
            tipsQue.Enqueue(tips);
        }
    }

    private void Update(){
        if(tipsQue.Count > 0 && !isTips){
            lock(tipsQue){
                string tips = tipsQue.Dequeue();
                isTips = true;
                SetTips(tips);
            }
        }
    }

    private void SetTips(string content){
        SetActive(tips);
        SetText(tips, content);

        AnimationClip clip = anim.GetClip("TxtTipsIdle");
        anim.Play();
        // 延时关闭激活状态
        StartCoroutine(AniPlayDone(clip.length, () => {
            SetActive(tips, false);
            isTips = false;
        }));

    }

    private IEnumerator AniPlayDone(float second, Action cb){
        yield return new WaitForSeconds(second);
        if(cb != null){
            cb();
        }
    }

    // 增加血条UI提示
    public void AddHPItemInfo(string mName, Transform transform, int hp){
        HpItem hpItem = null;
        if(itemDic.TryGetValue(mName, out hpItem)){
            return;
        }else{
            GameObject go = resService.LoadPrefeb(Constants.hpItemPath, true);
            go.transform.SetParent(hpItemRoot);
            go.transform.localPosition = new Vector3(-1000, 0, 0);
            HpItem item = go.GetComponent<HpItem>();
            // 设置血量信息
            item.InitItemInfo(transform, hp);
            itemDic.Add(mName, item);
        }
    }
    public void RemoveHPItemInfo(string name){
        HpItem hpItem = null;
        if(itemDic.TryGetValue(name, out hpItem)){
            Destroy(hpItem.gameObject);
            itemDic.Remove(name);
        }
    }

    // 移除所有血条
    public void RemoveAllHPItemInfo(){
        foreach (var item in itemDic)
        {
            Destroy(item.Value.gameObject);
        }
        itemDic.Clear();
    }

    public void SetDodge(string key){
        HpItem item = null;
        if(itemDic.TryGetValue(key, out item)){
            item.SetDodge();
        }
    }
    public void SetCritical(string key, int critical){
        HpItem item = null;
        if(itemDic.TryGetValue(key, out item)){
            item.SetCritical(critical);
        }
    }
    public void SetHurt(string key, int hurt){
        HpItem item = null;
        if(itemDic.TryGetValue(key, out item)){
            item.SetHurt(hurt);
        }
    }
    public void SetHpVal(string key, int oldVal, int newVal){
        HpItem item = null;
        if(itemDic.TryGetValue(key, out item)){
            item.SetHpVal(oldVal, newVal);
        }
    }

    // 设置玩家的闪避信息
    public void SetSelDodge(){
        selfDodge.Stop();
        selfDodge.Play();
    }
}
