using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PENet;
using PEProtocal;


/// <summary>
/// 网络服务
/// </summary>
public class NetService : MonoBehaviour
{
    public static NetService instance;

    // 创建客户端
    PESocket<ClientSession, GameMsg> client;

    private Queue<GameMsg> msgQue = new Queue<GameMsg>();

    private static readonly string obj = "lock";
    // 初始化客户端，连接服务器
    public void InitService(){
        instance = this;

        client = new PESocket<ClientSession, GameMsg>();

        client.SetLog(true, (string msg, int level) =>{
            switch (level)
            {
                case 0:
                    msg = "Log:" + msg;
                    Debug.Log(msg);
                    break;
                case 1:
                    msg =  "Warn:" + msg;
                    Debug.LogWarning(msg);
                    break;
                case 2:
                    msg =  "Error:" + msg;
                    Debug.LogError(msg);
                    break;
                case 3:
                    msg =  "Info:" + msg;
                    Debug.Log(msg);
                    break;
                default:
                    break;
            }
        });

        client.StartAsClient(ServerCfg.IP, ServerCfg.port);
    }

    // 发送消息
    public void SendMsg(GameMsg msg){
        if(client.session != null){
            client.session.SendMsg(msg);
        }else{
            GameRoot.AddTips("服务器未连接");
            // 重新进行一次连接
            InitService();
        }
    }

    public void AddNetPkg(GameMsg msg){
        lock(obj){
            msgQue.Enqueue(msg);
        }
    }

    private void Update() {
        if(msgQue.Count > 0){
            lock(obj){
                GameMsg msg = msgQue.Dequeue();
                ProcessMsg(msg);
            }
        }
    }

    // 消息分发
    private void ProcessMsg(GameMsg msg){
        if(msg.err != (int)ErrorCode.none){
            switch ((ErrorCode)msg.err)
            {
                case ErrorCode.serverDataError:
                    PECommon.Log("数据库数据异常", LogMethod.Error);
                    GameRoot.AddTips("客户端数据异常");
                    break;
                case ErrorCode.acctIsOnLine:
                    GameRoot.AddTips("当前帐号已经上线");
                    break;
                case ErrorCode.wrongPass:
                    GameRoot.AddTips("密码错误");
                    break;
                case ErrorCode.updateDBError:
                    PECommon.Log("数据库更新异常", LogMethod.Error);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.lackCoin:
                    GameRoot.AddTips("金币数量不够");
                    break;
                case ErrorCode.lackCrystal:
                    GameRoot.AddTips("水晶数量不够");
                    break;
                case ErrorCode.lackLevel:
                    GameRoot.AddTips("角色级别不够");
                    break;
                case ErrorCode.lackDiamond:
                    GameRoot.AddTips("钻石数量不够");
                    break;
                case ErrorCode.ClientDataError:
                    PECommon.Log("客户端数据异常", LogMethod.Error);
                    break;
                case ErrorCode.lackPower:
                    GameRoot.AddTips("角色体力不够");
                    break;
                default:
                    break;
            }
            return;
        }
        switch ((Command)msg.cmd)
        {
            case Command.responseLogin:
                LoginSys.instance.RespondLogin(msg);
                break;
            case Command.rspRename:
                LoginSys.instance.RspRename(msg);
                break;
            case Command.rspGuide:
                MainCitySys.instance.RspGuide(msg);
                break;
            case Command.rspStrong:
                MainCitySys.instance.RspStrong(msg);
                break;
            case Command.pushChat:
                MainCitySys.instance.PushChat(msg);
                break;
            case Command.rspBuy:
                MainCitySys.instance.RspBuy(msg);
                break;
            case Command.pushPower:
                MainCitySys.instance.PushPower(msg);
                break;
            case Command.rspTakeRewardTask:
                MainCitySys.instance.RspTakeTaskReward(msg);
                break;
            case Command.pushTaskArr:
                MainCitySys.instance.PushTaskPrgs(msg);
                break;
            case Command.rspFBFight:
                FuBenSys.instance.RspFBFight(msg);
                break;
            case Command.rspFBFightEnd:
                BattleSys.instance.RspFBFightEnd(msg);
                break;
            case Command.rspBuyItem:
                InventorySys.instance.RspBuyItem(msg);
                break;
            case Command.rspSellItem:
                InventorySys.instance.RspSellItem(msg);
                break;
            case Command.rspRank:
                MainCitySys.instance.RspRank(msg);
                break;
            default:
                break;
        }
    }
}
