using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;
using UnityEngine.UI;

public class RankWin : WinRoot
{
    #region UIDefine
    public Transform scrollTrans;
    #endregion

    PlayerData pd;
    
    public List<RankData> rankDataList;

    protected override void InitWin()
    {
        base.InitWin();
        pd = GameRoot.instance.PlayerData;
        RefreshUI();
    }

    public void RefreshUI(){
        // 在实例化前先销毁之前生成的物体
        for (int i = 0; i < scrollTrans.childCount; i++)
        {
            Destroy(scrollTrans.GetChild(i).gameObject);
        }
        
        // 生成RankBg
        for (int i = 0; i < rankDataList.Count; i++)
        {
            GameObject go = resService.LoadPrefeb(Constants.rankItemPrefeb, scrollTrans);
            go.name = "Rank_" + i;
            // 设置战斗力
            SetText(GetTrans(go.transform, "Fight"), rankDataList[i].fight);
            // 设置名字
            SetText(GetTrans(go.transform, "Name"), rankDataList[i].name);
            SetText(GetTrans(go.transform, "RankCount"), i + 1);
        }
        
    }

    public void ClickCloseBtn(){
        SetWinState(false);
    }
}
