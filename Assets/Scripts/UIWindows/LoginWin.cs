using System.ComponentModel.Design;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocal;

public class LoginWin : WinRoot
{
    public InputField inputAccount;
    public InputField inputPass;
    public Button enterButton;

    protected override void InitWin(){
        base.InitWin();

        // 获取本地存储的账号密码，如果存在，则直接输入
        if(PlayerPrefs.HasKey("Acct")&&PlayerPrefs.HasKey("Pass")){
            inputAccount.text = PlayerPrefs.GetString("Acct");
            inputPass.text = PlayerPrefs.GetString("Pass");
        }else{
            // 如果不存在则为空
            inputAccount.text = "";
            inputPass.text = "";
        }

    }

    /// <summary>
    /// 点击进入游戏按钮
    /// </summary>
    public void ClickEnterButton(){
        // 播放点击登录游戏按钮音效
        audioServer.PlayUIAudio(Constants.UILoginButtonClick);

        string _acct = inputAccount.text;
        string _pass = inputPass.text;
        if(_acct != "" && _pass != ""){
            // 更新本地存储的账号密码
            PlayerPrefs.SetString("Acct", _acct);
            PlayerPrefs.SetString("Pass", _pass);
                
            // 发送网络消息，请求登录
            GameMsg msg = new GameMsg{
                cmd = (int) Command.requestLogin,
                requestLogin = new RequestLogin{
                    acct = _acct,
                    pass = _pass
                }
            };
            netService.SendMsg(msg);
            
        }else{
            // 没有输入的时候
            GameRoot.AddTips("账号或密码为空");
        }
    }

    /// <summary>
    /// 点击公告按钮
    /// </summary>
    public void ClickNoticeButton(){
        // 播放UI点击按钮常规音效
        audioServer.PlayUIAudio(Constants.UIClickButton);
        // 打开公告界面
        LoginSys.instance.SetGongGaoWinState();
    }
}
