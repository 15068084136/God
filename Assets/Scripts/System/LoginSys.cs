using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;

/// <summary>
/// 登录系统
/// </summary>
public class LoginSys : SystemRoot
{
    public static LoginSys instance;
    public LoginWin loginWin;
    public CreatWin creatWin;
    public GongGaoWin gongGaoWin;

    public override void InitSys(){
        base.InitSys();
        instance = this;
    }

    /// <summary>
    /// 进入加载场景
    /// </summary>
    public void EnterLogin(){
        // 异步加载 加载场景
        resService.AsyncLoadScene(Constants.SceneLogin, () => {
        // 进入登录界面
        loginWin.SetWinState();
        // 登陆界面音效
        audioServer.PlayBGMAudio(Constants.LoginBGM);
        });
    }

    /// <summary>
    /// 响应登录操作
    /// </summary>
    public void RespondLogin(GameMsg msg){
        GameRoot.AddTips("登录成功");
        GameRoot.instance.SetPlayerData(msg.repLogin);

        if(msg.repLogin.playerData.name == ""){
            // 打开角色创建面板
            creatWin.SetWinState();
        }else{
            // 进入到主城
            MainCitySys.instance.EnterMainCity();
        }

        // 关闭登录界面
        loginWin.SetWinState(false);

        
    }

    public void RspRename(GameMsg msg){
        GameRoot.instance.SetPlayerName(msg.rspRename.name);

        MainCitySys.instance.EnterMainCity();
        // 关闭创建界面
        creatWin.SetWinState(false);
    }

    #region GongGaoWin
    public void SetGongGaoWinState(bool active = true){
        gongGaoWin.SetWinState(active);
    }

    #endregion
}
