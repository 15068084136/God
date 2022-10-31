using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗结算界面
/// </summary>
public class BattleEndWin : WinRoot
{
    #region  UIDefine
    public Transform rewardTrans;
    public Button closeBtn;
    public Button ExitBtn;
    public Button SureBtn;
    public Text timeTxt;
    public Text restHPTxt;
    public Text rewardTxt;
    public Animation ani;
    #endregion

    private FBEndType endType = FBEndType.none;

    protected override void InitWin()
    {
        base.InitWin();

        RefreshUI();
    }

    private void RefreshUI(){
        switch (endType)
        {
            case FBEndType.pause:
                SetActive(rewardTrans, false);
                SetActive(ExitBtn.gameObject);
                SetActive(closeBtn.gameObject);
                break;
            case FBEndType.win:
                SetActive(rewardTrans, false);
                SetActive(ExitBtn.gameObject, false);
                SetActive(closeBtn.gameObject, false);

                MapCfg mapCfg = resService.GetMapCfg(fbId);
                int min = costTime / 60;
                int sec = costTime % 60;
                int coin = mapCfg.coin;
                int exp = mapCfg.exp;
                int crystal = mapCfg.crystal;
                SetText(timeTxt, "通关时间：" + min + ":" + sec);
                SetText(restHPTxt, "剩余血量：" + restHP);
                SetText(rewardTxt, "关卡奖励：" + Constants.ColorText(coin + "金币", TxtColor.green) + Constants.ColorText(exp + "经验", TxtColor.yellow) + Constants.ColorText(crystal + "水晶", TxtColor.blue));
                
                timerService.AddTimeTask((int tid)=>{
                    SetActive(rewardTrans);
                    ani.Play();
                    timerService.AddTimeTask((int tid1)=>{
                        audioServer.PlayUIAudio(Constants.fbItem);
                        timerService.AddTimeTask((int tid2)=>{
                            audioServer.PlayUIAudio(Constants.fbItem);
                            timerService.AddTimeTask((int tid3)=>{
                                audioServer.PlayUIAudio(Constants.fbItem);
                                timerService.AddTimeTask((int tid4)=>{
                                    audioServer.PlayUIAudio(Constants.FBLogoEnter);
                                }, 370);
                            }, 350);
                        }, 350);
                    }, 400);
                }, 1000);
                break;
            case FBEndType.lose:
                SetActive(rewardTrans, false);
                SetActive(ExitBtn.gameObject);
                SetActive(closeBtn.gameObject, false);
                audioServer.PlayUIAudio(Constants.FBLose);
                break;
        }
    }

    public void SetWinType(FBEndType endType){
        this.endType = endType;
    }

    private int fbId;
    private int costTime;
    private int restHP;
    public void SetWinData(int fbId, int costTime, int restHP){
        this.fbId = fbId;
        this.costTime = costTime;
        this.restHP = restHP;
    }

    public void ClickCloseBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        SetWinState(false);
        BattleSys.instance.battleManager.isPauseGame = false;
    }
    public void ClickExitBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        // 销毁当前战斗，进入主城
        MainCitySys.instance.EnterMainCity();
        BattleSys.instance.DestroyBattle();
        SetWinState(false);
    }
    public void ClickSureBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        // 销毁当前战斗，进入主城
        MainCitySys.instance.EnterMainCity();
        BattleSys.instance.DestroyBattle();
        // 打开副本
        FuBenSys.instance.EnterFuBen();
        SetWinState(false);
    }

}

public enum FBEndType{
    none,
    pause,
    win,
    lose
}
