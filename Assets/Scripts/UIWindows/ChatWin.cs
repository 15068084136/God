using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocal;

/// <summary>
/// 聊天界面
/// </summary>
public class ChatWin : WinRoot
{
    #region UI Define
    public InputField inputChat;
    public Text chatTxt;
    public Image worldImage;
    public Image teamImage;
    public Image friendImage;
    #endregion

    // 当前是什么聊天类型
    private int chatType;
    // 文本列表
    private List<string> chatList  = new List<string>();

    protected override void InitWin()
    {
        base.InitWin();

        chatType = 0;

        RefreshUI();
    }

    // 广播消息
    public void AddChatMsg(string name, string chat){
        chatList.Add(Constants.ColorText(name + ":", TxtColor.blue) + chat);
        if(chatList.Count > 12){
            chatList.RemoveAt(0);
        }
        RefreshUI();
    }

    public void RefreshUI(){
        if(chatType == 0){
            string chatMsg = "";
            for (int i = 0; i < chatList.Count; i++)
            {
                chatMsg += chatList[i] + "\n";
            }
            SetText(chatTxt, chatMsg);
            SetSprite(worldImage, Constants.chatTypeBtn1);
            SetSprite(teamImage, Constants.chatTypeBtn2);
            SetSprite(friendImage, Constants.chatTypeBtn2);
        }else if(chatType == 1){
            SetText(chatTxt, "尚未加入工会");
            SetSprite(worldImage, Constants.chatTypeBtn2);
            SetSprite(teamImage, Constants.chatTypeBtn1);
            SetSprite(friendImage, Constants.chatTypeBtn2);
        }else if(chatType == 2){
            SetText(chatTxt, "暂无好友信息");
            SetSprite(worldImage, Constants.chatTypeBtn2);
            SetSprite(teamImage, Constants.chatTypeBtn2);
            SetSprite(friendImage, Constants.chatTypeBtn1);
        }
    }
    #region ClickEvts
    public void ClickWorldBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        chatType = 0;
        RefreshUI();
    }
    public void ClickTeamBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        chatType = 1;
        RefreshUI();
    }
    public void ClickFriendBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        chatType = 2;
        RefreshUI();
    }
    bool canSend = true;
    public void ClickSendBtn(){
        if(!canSend){
            GameRoot.AddTips("聊天消息每5秒才能发送一条");
            return;
        }
        if(inputChat.text != null && inputChat.text != ""){
            if(inputChat.text.Length > 12){
                GameRoot.AddTips("输入信息不能超过12个字");
            }else{
                // 发送网络消息
                GameMsg msg = new GameMsg{
                    cmd = (int)Command.sendChat,
                    sendChat = new SendChat{
                        chat = inputChat.text
                    }
                };
                // 发送完消息后要清空
                inputChat.text = "";
                netService.SendMsg(msg);
                canSend = false;

                // 计时器，效果如同协程一致
                timerService.AddTimeTask((int tid)=>{
                    canSend = true;
                }, 5, PETimeUnit.Second);
            }
        }else{
            GameRoot.AddTips("尚未输入聊天信息");
        }
    }
    public void CloseBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        SetWinState(false);
        chatType = 0;
    }
    #endregion
}
