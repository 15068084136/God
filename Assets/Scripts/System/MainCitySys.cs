using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PEProtocal;

/// <summary>
/// 主城业务系统
/// </summary>
public class MainCitySys : SystemRoot
{
    public static MainCitySys instance;

    #region Win
    public TaskWin taskWin;
    public BuyWin buyWin;
    public ChatWin chatWin;
    public MainCityWin mainCityWin;
    public StrongWin strongWin;
    public GuideWin guideWin;
    public InfoWin infoWin;
    #endregion
    public Transform charCamTrans;
    PlayerController playerController;
    private AutoGuideCfg currentTaskData;
    private Transform[] npcPosTrans;
    private NavMeshAgent nav;
    private CharacterController characterController;

    public override void InitSys(){
        base.InitSys();
        instance = this;
    }

    // 进入主城
    public void EnterMainCity(){
        // 根据mapID得到主城mapdata
        MapCfg mapData = resService.GetMapCfg(Constants.MainCityMapID);
        resService.AsyncLoadScene(mapData.sceneName, ()=>{
            PECommon.Log("Enter MainCity...");

            // 加载游戏主角
            LoadPlayer(mapData);
            // 打开主城场景UI
            mainCityWin.SetWinState();
            // 播放主城背景音乐
            audioServer.PlayBGMAudio(Constants.MainCityBGM);

            // 关闭GameRoot上的AudioListener
            GameRoot.instance.GetComponent<AudioListener>().enabled = false;

            // 获得NPC坐标
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            MainCityMap mcm = map.GetComponent<MainCityMap>();
            npcPosTrans = mcm.npcPosTrans;

            // 人物展示相机
            if(charCamTrans != null){
                charCamTrans.gameObject.SetActive(false);
            }
        });
    }

    private void LoadPlayer(MapCfg mapData){
        GameObject player =  resService.LoadPrefeb(Constants.playerPrefebPath, true);
        // 初始化角色位置
        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRote;
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        // 初始化相机位置
        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        playerController = player.GetComponent<PlayerController>();
        playerController.Init();
        nav = playerController.GetComponent<NavMeshAgent>();
        characterController = playerController.GetComponent<CharacterController>();
    }

    public void SetMoveDir(Vector2 dir){
        // 如果在导航，则关闭导航
        StopNavTask();

        if(dir == Vector2.zero){
            playerController.SetBlend(0);
        }else{
            playerController.SetBlend(1);
        }
        playerController.Dir = dir;
    }

    #region BuyWin
    public void OpenBuyWin(int buyType){
        buyWin.SetBuyType(buyType);
        buyWin.SetWinState();
    }
    public void RspBuy(GameMsg msg){
        RspBuy data = msg.rspBuy;
        GameRoot.instance.SetPlayerDataByBuy(data);
        GameRoot.AddTips("购买成功");
        mainCityWin.RefreshUI();
        buyWin.SetWinState(false);
    }
    #endregion

    #region EnterFuBenSys
    // 中转
    public void EnterFuBen(){
        // 打开副本界面时，停下寻路
        StopNavTask();
        FuBenSys.instance.EnterFuBen();
    }
    #endregion

    #region TaskWin
    public void OpenTaskWin(){
        StopNavTask();
        taskWin.SetWinState();
    }
    public void RspTakeTaskReward(GameMsg msg){
        RspTakeTaskReward data = msg.rspTakeTaskReward;
        // 改变GameRoot中的数据
        GameRoot.instance.SetPlayerDataByTaskReward(data);

        taskWin.RefreshUI();
        mainCityWin.RefreshUI();
    }
    public void PushTaskPrgs(GameMsg msg){
        PushTaskArr data = msg.pushTaskArr;
        GameRoot.instance.SetPlayerDataByTaskPrgs(data);
    }
    #endregion

    #region 体力恢复相关
    public void PushPower(GameMsg msg){
        PushPower data = msg.pushPower;
        GameRoot.instance.SetPlayerDataByPower(data);
        if(mainCityWin.gameObject.activeSelf){
            mainCityWin.RefreshUI();
        }
    }
    #endregion

    #region ChatWin
    public void OpenChatWin(){
        StopNavTask();
        chatWin.SetWinState();
    }
    public void PushChat(GameMsg msg){
        chatWin.AddChatMsg(msg.pushChat.name, msg.pushChat.chat);
    }
    #endregion

    #region StrongWin
    public void OpenStrongWin(){
        StopNavTask();
        strongWin.SetWinState();
    }
    public void RspStrong(GameMsg msg){
        // 强化之前的战力
        int preFight = PECommon.GetFight(GameRoot.instance.PlayerData);
        // 更新GameRoot里的PlayerData
        GameRoot.instance.SetPlayerDataByStrong(msg.rspStrong);
        // 计算当前战力
        int nowFight = PECommon.GetFight(GameRoot.instance.PlayerData);
        // 计算战力增加数值
        GameRoot.AddTips(Constants.ColorText("战力提升" + (nowFight - preFight), TxtColor.blue));
        // 刷新UI
        strongWin.UpdateStrongDetailWin();
        // 刷新主城UI
        mainCityWin.RefreshUI();
    }
    #endregion

    #region InfoWin
    public void OpenInfoWin(){
        // 关闭导航
        StopNavTask();

        if(charCamTrans == null){
            charCamTrans = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }

        // 设置人物展示相机相对位置
        charCamTrans.localPosition = playerController.transform.position + playerController.transform.forward * 6.5f + new Vector3(0, 1.4f, 0);
        charCamTrans.localEulerAngles = new Vector3(0, 180 + playerController.transform.localEulerAngles.y, 0);
        charCamTrans.localScale = Vector3.one;
        charCamTrans.gameObject.SetActive(true);
        infoWin.SetWinState();
    }

    public void CloseInfoWin(){
        if(charCamTrans != null){
            charCamTrans.gameObject.SetActive(false);
            infoWin.SetWinState(false);
        }
    }

    // 展示角色的初始旋转
    private float startRoate;
    public void SetPlayerStartRoate(){
        startRoate = playerController.transform.localEulerAngles.y;
    }

    public void SetPlayerRotate(float rotate){
        playerController.transform.localEulerAngles = new Vector3(0, startRoate + rotate, 0);
    }
    #endregion

    #region GuideWin
    private bool isNav;
    // 执行任务
    public void RunTask(AutoGuideCfg agc){
        if(agc != null){
            // 保存任务数据
            currentTaskData = agc;
        }
        // 解析任务数据
        nav.enabled = true;
        characterController.enabled = false;
        if(currentTaskData.npcID != -1){
            float dis = Vector3.Distance(playerController.transform.position, npcPosTrans[agc.npcID].position);
            // 如果主角生成的位置就是目标位置附近，则不导航
            if(dis < 0.5f){
                isNav = false;
                nav.isStopped = true;
                playerController.SetBlend(0);
                nav.enabled = false;
                characterController.enabled = true;
                OpenGuideWin();
            }else{
                isNav = true;
                nav.enabled = true;
                nav.speed = Constants.PlayerMoveSpeed;
                nav.SetDestination(npcPosTrans[agc.npcID].position);
                playerController.SetBlend(1);
            }
        }else{
            // 打开引导界面
            OpenGuideWin();
            characterController.enabled = true;
        }
    }

    private void Update() {
        if(isNav){
            IsArriveNavPos();
            // 在导航的时候更新摄像机位置
            playerController.SetCam();
        }
    }

    // 判断是否已经到达目的地
    private void IsArriveNavPos(){
        float dis = Vector3.Distance(playerController.transform.position, npcPosTrans[currentTaskData.npcID].position);
        if(dis < 0.5f){
            isNav = false;
            nav.isStopped = true;
            playerController.SetBlend(0);
            nav.enabled = false;
            characterController.enabled = true;
            OpenGuideWin();
        }
    }

    // 关闭导航
    private void StopNavTask(){
        if(isNav){
            isNav = false;
            nav.isStopped = true;
            playerController.SetBlend(0);
            nav.enabled = false;
            characterController.enabled = true;
        }
    }

    private void OpenGuideWin()
    {
        guideWin.SetWinState();
    }

    // 获得任务数据
    public AutoGuideCfg GetAutoGuideCfg(){
        return currentTaskData;
    }
    // 接收到服务器的引导信息
    public void RspGuide(GameMsg msg){
        RspGuide data = msg.rspGuide;

        GameRoot.AddTips(Constants.ColorText("任务奖励 金币+" + currentTaskData.coin + "经验+" + currentTaskData.exp, TxtColor.blue));

        switch(currentTaskData.actID){
            case 0:
                // 与智者对话
                break;
            case 1:
                // 进入副本
                EnterFuBen();
                break;
            case 2:
                // 进入强化界面
                OpenStrongWin();
                break;
            case 3:
                // 进入购买体力
                OpenBuyWin(0);
                break;
            case 4:
                // 进入金币铸造
                OpenBuyWin(1);
                break;
            case 5:
                // 进入世界聊天
                OpenChatWin();
                break;
        }
        // 更改GameRoot中的玩家数据
        GameRoot.instance.SetPlayerDataByGuide(data);
        // 更新主城UI界面数据
        mainCityWin.RefreshUI();
    }
    #endregion
}
