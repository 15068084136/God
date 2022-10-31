using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocal;

/// <summary>
/// 购买界面
/// </summary>
public class BuyWin : WinRoot
{
    public Text infoText;
    public Button sureBtn;
    private int buyType;// 0：体力 1：金币

    protected override void InitWin()
    {
        base.InitWin();
        sureBtn.interactable = true;
        RefreshUI();
    }
    public void SetBuyType(int buyType){
        this.buyType = buyType;
    }
    private void RefreshUI(){
        switch (buyType)
        {
            case 0:
                // 体力
                infoText.text = "是否花费" + Constants.ColorText("10钻石", TxtColor.red) + "购买" + Constants.ColorText("100体力", TxtColor.green) + "?";
                break;
            case 1:
                // 金币
                infoText.text = "是否花费" + Constants.ColorText("10钻石", TxtColor.red) + "购买" + Constants.ColorText("1000金币", TxtColor.green) + "?";
                break;
        }
    }
    public void ClickSureBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        // 发送网络消息
        GameMsg msg = new GameMsg{
            cmd = (int)Command.reqBuy,
            reqBuy = new ReqBuy{
                type = buyType,
                cost = 10
            }
        };
        netService.SendMsg(msg);
        // 关闭按钮交互
        sureBtn.interactable = false;
    }
    public void ClickCloseBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        SetWinState(false);
    }
}
