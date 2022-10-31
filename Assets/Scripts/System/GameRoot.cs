using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;

public class GameRoot : MonoBehaviour
{
    public static GameRoot instance;

    // 公用业务系统API
    public LoadingWin loadingWin;
    public DynamicWin dynamicWin;

    private void Start() {
        instance = this;
        DontDestroyOnLoad(this);

        ClearUIWin();

        Init();
    }

    /// <summary>
    /// 使得除了DynamicWin外的所有win关闭
    /// </summary>
    private void ClearUIWin(){
        // 找到子物体Canvas
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }

    }

    void Init(){
    // 服务模块初始化
    LuaService luaService = GetComponent<LuaService>();
    luaService.InitService();
    NetService netService = GetComponent<NetService>();
    netService.InitService();
    ResService resService = GetComponent<ResService>();
    resService.InitService();
    AudioServer audioServer = GetComponent<AudioServer>();
    audioServer.InitService();
    TimerService timerService = GetComponent<TimerService>();
    timerService.InitService();
    // 业务系统初始化
    LoginSys loginSys = GetComponent<LoginSys>();
    loginSys.InitSys();
    MainCitySys mainCitySys = GetComponent<MainCitySys>();
    mainCitySys.InitSys();
    FuBenSys fuBenSys = GetComponent<FuBenSys>();
    fuBenSys.InitSys();
    BattleSys battleSys = GetComponent<BattleSys>();
    battleSys.InitSys();

    dynamicWin.SetWinState();
    // 进入登入场景并加载相应UI
    loginSys.EnterLogin();
    }

    public static void AddTips(string content){
        instance.dynamicWin.AddTips(content);
    }

    private PlayerData playerData;
    public PlayerData PlayerData{
        get{
            return playerData;
        }
    }

    #region 设置玩家数据
    public void SetPlayerData(RepLogin data){
        playerData = data.playerData;
    }

    public void SetPlayerName(string name){
        PlayerData.name = name;
    }

    // 通过引导任务改变玩家数据
    public void SetPlayerDataByGuide(RspGuide rspGuide){
        playerData.coin = rspGuide.coin;
        playerData.level = rspGuide.level;
        playerData.exp = rspGuide.exp;
        playerData.guideId = rspGuide.guideId;
    }
    // 通过强化改变玩家数据
    public void SetPlayerDataByStrong(RspStrong data){
        playerData.coin = data.coin;
        playerData.crystal = data.crystal;
        playerData.hp = data.hp;
        playerData.ad = data.ad;
        playerData.ap = data.ap;
        playerData.addef = data.addef;
        playerData.apdef = data.apdef;
        playerData.strongArr = data.strongArr;
    }

    public void SetPlayerDataByBuy(RspBuy data){
        playerData.coin = data.coin;
        playerData.diamond = data.diamond;
        playerData.power = data.power;
    }

    public void SetPlayerDataByPower(PushPower data){
        playerData.power = data.power;
    }

    public void SetPlayerDataByTaskReward(RspTakeTaskReward data){
        playerData.coin = data.coin;
        playerData.level = data.level;
        playerData.exp = data.exp;
        playerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByTaskPrgs(PushTaskArr data){
        playerData.taskArr = data.taskArr;
    }

    public void SetPlayerDataByFBFightStart(RspFBFight data){
        playerData.power = data.power;
    }

    public void SetPlayerDataByFBFightEnd(RspFBFightEnd data){
        playerData.coin = data.coin;
        playerData.level = data.level;
        playerData.exp = data.exp;
        playerData.critical = data.crystal;
        playerData.fuBen = data.fuben;
    }
    #endregion
}
