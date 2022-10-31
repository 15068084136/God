using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocal;

/// <summary>
/// 副本选择界面
/// </summary>
public class FuBenWin : WinRoot{
    #region UI Definer
    public Button[] fuBenBtnArr;
    public Transform tipsTrans;
    #endregion
    PlayerData pd;

    protected override void InitWin()
    {
        base.InitWin();
        pd = GameRoot.instance.PlayerData;
        RefreshUI();
    }
    public void RefreshUI(){
        int fbId = pd.fuBen;
        for (int i = 0; i < fuBenBtnArr.Length; i++)
        {
            if(i < fbId % 10000){
                SetActive(fuBenBtnArr[i].gameObject);
                if(i == fbId % 10000 - 1){
                    // 设置 当前任务 图标
                    tipsTrans.SetParent(fuBenBtnArr[i].transform);
                    tipsTrans.localPosition = new Vector3(0, 100, 0);
                }
            }else{
                SetActive(fuBenBtnArr[i].gameObject, false);
            }
        }
    }
    public void ClickCloseBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        SetWinState(false);
    }

    // 点击进入副本按钮
    public void ClickTaskBtn(int fbId){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        // 检查体力是否充足
        int power = resService.GetMapCfg(fbId).power;
        if(power > pd.power){
            GameRoot.AddTips("体力值不足");
        }else{
            // 发送网路消息
            netService.SendMsg(new GameMsg{
                cmd = (int)Command.reqFBFight,
                reqFBFight = new ReqFBFight{
                    fbId = fbId
                }
            });
        }
    }
}
