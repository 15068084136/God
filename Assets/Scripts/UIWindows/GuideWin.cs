using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocal;

/// <summary>
/// 引导对话界面
/// </summary>
public class GuideWin : WinRoot
{
    public Text nameTxt;
    public Text talkTxt;
    public Image iconImage;

    private PlayerData pd;
    private AutoGuideCfg currentTaskData;
    // 对话数组
    private string[] dialogArr;
    // dialog索引号
    private int index;

    protected override void InitWin()
    {
        base.InitWin();
        pd = GameRoot.instance.PlayerData;
        currentTaskData = MainCitySys.instance.GetAutoGuideCfg();
        // 用井号键进行切割
        dialogArr = currentTaskData.dialogArr.Split("#");
        // 初始索引号为1
        index = 1;

        SetTalk();
    }

    private void SetTalk()
    {
        // 根据|来切割，得到序号和话
        string[] talkArr = dialogArr[index].Split("|");
        if(talkArr[0] == "0"){
            // 图标是自己
            SetSprite(iconImage, Constants.selfIconPath);
            SetText(nameTxt, pd.name);
        }else{
            // 图标是对话的NPC
            switch(currentTaskData.npcID){
                case Constants.wiseManNpc:
                    SetSprite(iconImage, Constants.wiseManIconPath);
                    SetText(nameTxt, "智者");
                    break;
                case Constants.generalNpc:
                    SetSprite(iconImage, Constants.generalIconPath);
                    SetText(nameTxt, "将军");
                    break;
                case Constants.artisanNpc:
                    SetSprite(iconImage, Constants.artisanIconPath);
                    SetText(nameTxt, "工匠");
                    break;
                case Constants.traderNpc:
                    SetSprite(iconImage, Constants.traderIconPath);
                    SetText(nameTxt, "商人");
                    break;
                default:
                    SetSprite(iconImage, Constants.guideIconPath);
                    SetText(nameTxt, "小精灵");
                    break;
            }
        }
        // 将图标设置为原生大小
        iconImage.SetNativeSize();
        // 设置对话内容，并改变对应的名称
        SetText(talkTxt, talkArr[1].Replace("$name", pd.name));
    }

    // 点击下一段对话按钮
    public void ClickNextBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);

        index += 1;
        if(index == dialogArr.Length){
            // 发送任务引导完成信息
            GameMsg msg = new GameMsg{
                cmd = (int)Command.reqGuide,
                reqGuide = new ReqGuide{
                    guideId = currentTaskData.ID
                }
            };
            netService.SendMsg(msg);
            SetWinState(false);
        }else{
            SetTalk();
        }
    }
}
