using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

/// <summary>
/// 公告UI界面
/// </summary>
[Hotfix]
public class GongGaoWin : WinRoot
{
    List<string> noticeList = new List<string>();

    string notice1 = "公告:";
    string notice2 = "1--";
    string notice3 = "2--";
    string notice4 = "3--";

    public Text noticeTxt;

    protected override void InitWin()
    {
        base.InitWin();

        RefreshUI();
    }
    [LuaCallCSharp]
    private void RefreshUI(){
        if(noticeTxt.text != ""){
            noticeTxt.text = "";
            noticeList.Clear();
        }
        notice1 = "公告:";
        notice2 = "1--";
        notice3 = "2--";
        notice4 = "3--";
        noticeList.Add(notice1);
        noticeList.Add(notice2);
        noticeList.Add(notice3);
        noticeList.Add(notice4);
        for (int i = 0; i < noticeList.Count; i++)
        {
            noticeTxt.text += noticeList[i] + "\n";
        }
    }

    public void ClickCloseBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);

        LoginSys.instance.SetGongGaoWinState(false);
    }
    
}
