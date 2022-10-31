using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PEProtocal;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 主城UI界面
/// </summary>
public class MainCityWin : WinRoot
{
    public Image dirTouchBg;
    public Image dirImage;
    public Image dirPointImage;
    public Animation XMenuAni;
    public Button XMenuBtn;
    public Text fightTxt;
    public Text powerTxt;
    public Image powerPrg;
    public Text levelTxt;
    public Text nameTxt;
    public Text expTxt;
    public Transform expPrgTrans;
    public AutoGuideCfg currentTaskData;
    public Button guideBtn;

    protected override void InitWin()
    {
        base.InitWin();

        defaultPos = dirTouchBg.transform.position;
        // 初始状态下小点是不显示的
        SetActive(dirPointImage, false);
        RegisterTouchEvent();

        RefreshUI();
    }

    // 刷新UI控件
    public void RefreshUI(){
        PlayerData pd = GameRoot.instance.PlayerData;
        // 刷新战斗力UI
        SetText(fightTxt, PECommon.GetFight(pd));
        // 刷新体力值UI
        SetText(powerTxt, "体力:"+ pd.power + "/" + PECommon.GetPowerLimit(pd.level));
        // 刷新体力进度UI
        powerPrg.fillAmount = pd.power * 1.0f/PECommon.GetPowerLimit(pd.level);
        SetText(levelTxt, pd.level);
        SetText(nameTxt, pd.name);
        // 刷新经验值
        int expprgValue = (int)((pd.exp * 1.0f/PECommon.GetExpLevelUp(pd.level) * 100));
        SetText(expTxt, expprgValue + "%");
        // 刷新经验值进度条
        int index = expprgValue / 10;
        for (int i = 0; i < expPrgTrans.childCount; i++)
        {
            Image image = expPrgTrans.GetChild(i).GetComponent<Image>();
            if(i < index){
                image.fillAmount = 1;
            }else if(i == index){
                image.fillAmount = expprgValue % 10 * 1.0f/ 10;
            }else
            {
                image.fillAmount = 0;
            }
        } 

        // 设置自动任务图标
        currentTaskData = resService.GetGuildCfg(pd.guideId);
        if(currentTaskData != null){
            SetGuideBtnIcon(currentTaskData.npcID);
        }else{
            SetGuideBtnIcon(-1);
        }
    }

    private void SetGuideBtnIcon(int npcID){
        string spPath = "";
        Image image = guideBtn.GetComponent<Image>();
        switch(npcID){
            case Constants.wiseManNpc:
                spPath = Constants.wiseManHead;
                break;
            case Constants.generalNpc:
                spPath = Constants.generalHead;
                break;
            case Constants.artisanNpc:
                spPath = Constants.artisanHead;
                break;
            case Constants.traderNpc:
                spPath = Constants.traderHead;
                break;
            default:
                spPath = Constants.taskHead;
                break;
        }

        // 加载图片
        SetSprite(image, spPath);
    }

    #region 点击事件
    private bool XMenuState = true;
    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos;
    #region OpenWin
    public void ClickTaskBtn(){
        audioServer.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.instance.OpenTaskWin();
    }
    public void ClickBuyPowerBtn(){
        audioServer.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.instance.OpenBuyWin(0);
    }
    public void ClickBuyCoinBtn(){
        audioServer.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.instance.OpenBuyWin(1);
    } 
    public void ClickChatBtn(){
        audioServer.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.instance.OpenChatWin();
    }
    public void ClickStrongBtn(){
        audioServer.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.instance.OpenStrongWin();
    }
    public void ClickHeadBtn(){
        audioServer.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.instance.OpenInfoWin();
    }
    #endregion
    
    // 点击自动任务按钮
    public void ClickGuideBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        if(currentTaskData != null){
            MainCitySys.instance.RunTask(currentTaskData);
        }else{
            GameRoot.AddTips("更多引导任务，正在开发中...");
        }
    }
    public void ClickXMenuBtn(){
        audioServer.PlayUIAudio(Constants.UIExtenBtn);
        // 改变状态
        XMenuState = !XMenuState;
        AnimationClip clip;
        if(XMenuState){
            clip = XMenuAni.GetClip("OpenXMenu");
        }else{
            clip = XMenuAni.GetClip("CloseXMenu");
        }
        XMenuAni.Play(clip.name);
    }
    public void RegisterTouchEvent(){
        // 为触摸范围 添加PEListener组件
        OnClickDown(dirTouchBg.gameObject, (PointerEventData evt) =>{
            startPos = evt.position;
            // 激活关闭的小点
            SetActive(dirPointImage);
            dirImage.transform.position = evt.position;
        });
        OnClickUp(dirTouchBg.gameObject, (PointerEventData evt) =>{
            // 抬起时是在默认位置
            dirImage.transform.position = defaultPos;
            // 小点位置归零
            dirPointImage.transform.localPosition = Vector2.zero;
            // 并关闭小点
            SetActive(dirPointImage);
            MainCitySys.instance.SetMoveDir(Vector2.zero);
        });
        OnDrag(dirTouchBg.gameObject, (PointerEventData evt) =>{
            // 获得方向向量
            Vector2 dir = evt.position - startPos;
            // 获得向量模
            float length = dir.magnitude;
            if(length > Constants.ScreenDragLength){
                // 获得一个限制距离的向量
                Vector2 clampDir = Vector2.ClampMagnitude(dir, Constants.ScreenDragLength);
                dirPointImage.transform.position = startPos + clampDir;
            }else{
                // 在未超出范围时
                dirPointImage.transform.position = evt.position;
            }
            MainCitySys.instance.SetMoveDir(dir.normalized);
        });
    }

    #region Enter FuBenSys
    // 注意要中转副本系统
    public void ClickFuBenBtn(){
        audioServer.PlayUIAudio(Constants.UIOpenPage);
        MainCitySys.instance.EnterFuBen();
    }
    #endregion
    #endregion
}
