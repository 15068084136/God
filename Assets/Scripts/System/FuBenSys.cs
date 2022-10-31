using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;

/// <summary>
///  副本业务系统
/// </summary>
public class FuBenSys : SystemRoot
{
    public static FuBenSys instance;

    public FuBenWin fuBenWin;

    public override void InitSys(){
        base.InitSys();
        instance = this;
    }

    //  由MainCitySys中转至此
    public void EnterFuBen(){
        SetFuBenWinState();
    }

    #region FuBenWin
    public void SetFuBenWinState(bool isActive = true){
        fuBenWin.SetWinState(isActive);
    }
    #endregion

    public void RspFBFight(GameMsg msg){
        GameRoot.instance.SetPlayerDataByFBFightStart(msg.rspFBFight);
        // 关闭主城场景
        MainCitySys.instance.mainCityWin.SetWinState(false);
        // 关闭副本选择场景
        SetFuBenWinState(false);
        // 中转到BattleSys
        BattleSys.instance.StartBattle(msg.rspFBFight.fbId);
    }
}
