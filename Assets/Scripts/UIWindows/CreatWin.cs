using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocal;

/// <summary>
/// 角色创建界面
/// </summary>
public class CreatWin : WinRoot
{
    public InputField inputName;

    protected override void InitWin()
    {
        base.InitWin();

        // 随机名字，且角色是女性
        inputName.text = resService.GetRandomName(false);
    }

    // 色子按钮
    public void ClickRandomNameButton(){
        audioServer.PlayUIAudio(Constants.UIClickButton);

        string name = resService.GetRandomName(false);
        inputName.text = name;
    }

    // 进入游戏按钮
    public void ClickEnterGameButton(){
        audioServer.PlayUIAudio(Constants.UIClickButton);

        if(inputName.text != ""){
            // 发送网络消息，发送名字数据给服务器，登录主城
            GameMsg msg = new GameMsg{
                cmd = (int)Command.reqRename,
                reqRename = new ReqRename{
                    name = inputName.text
                }
            };
            netService.SendMsg(msg);
        }
        else{
            GameRoot.AddTips("当前输入不合规范");
        }
    }
}
