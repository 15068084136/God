using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;

/// <summary>
/// 战斗业务系统
/// </summary>
public class BattleSys : SystemRoot
{
    public static BattleSys instance;
    public PlayerContrlWin playerContrlWin;
    public BattleManager battleManager;
    public BattleEndWin battleEndWin;

    private int fbid;
    private double startTime;

    public override void InitSys(){
        base.InitSys();
        instance = this;
    }
    // 开启一场战斗
    public void StartBattle(int mapId){
        fbid = mapId;

        GameObject go = new GameObject{
            name = "BattleRoot"
        };
        go.transform.SetParent(GameRoot.instance.transform);

        battleManager = go.AddComponent<BattleManager>();
        battleManager.Init(mapId, ()=>{
            startTime = timerService.GetNowTime();
        });

        SetPlayerContrlWin();
    }
    // 结束一场战斗
    public void EndBattle(bool isWin, int restHP){
        playerContrlWin.SetWinState(false);
        GameRoot.instance.dynamicWin.RemoveAllHPItemInfo();

        if(isWin){
            double endTime = timerService.GetNowTime();
            // 发送结算战斗请求
            GameMsg msg = new GameMsg{
                cmd = (int)Command.reqFBFightEnd,
                reqFBFightEnd = new ReqFBFightEnd{
                    win = isWin,
                    fbId = fbid,
                    restHp = restHP,
                    costTime = (int)(endTime - startTime),
                }
            };
            netService.SendMsg(msg);
        }else{
            SetBattleEndWinState(FBEndType.lose);
        }
    }
    // 销毁战斗场景
    public void DestroyBattle(){
        playerContrlWin.SetWinState(false);
        GameRoot.instance.dynamicWin.RemoveAllHPItemInfo();
        Destroy(battleManager.gameObject);
    }

    #region BattleEndWin
    // 打开战斗结算界面
    public void SetBattleEndWinState(FBEndType endType, bool isActive = true){
        battleEndWin.SetWinType(endType);
        battleEndWin.SetWinState(isActive);
    }

    public void RspFBFightEnd(GameMsg msg){
        RspFBFightEnd data = msg.rspFBFightEnd;
        GameRoot.instance.SetPlayerDataByFBFightEnd(data);

        // 设置结算界面的数据
        battleEndWin.SetWinData(data.fbId, data.costTime, data.restHp);
        SetBattleEndWinState(FBEndType.win);
    }
    #endregion

    #region PlayerControlWin
    public void SetPlayerContrlWin(bool isActive = true){
        playerContrlWin.SetWinState(isActive);
    }

    // 由playerControlWin中转至BattleSys,由BattleSys中转至Battlemgr
    public void ReqReleaseSkill(int index){
        battleManager.ReqReleaseSkill(index);
    }

    public void SetPlayerMoveDir(Vector2 dir){
        // 设置玩家的移动
        battleManager.SetPlayerMoveDir(dir);
    }

    public Vector2 GetCurrnetDir(){
        return playerContrlWin.currentDir;
    }
    #endregion
}
