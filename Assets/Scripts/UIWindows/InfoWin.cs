using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocal;
using UnityEngine.EventSystems;

public class InfoWin : WinRoot
{
    #region UI组件
    // 角色信息
    public Text infoTxt;
    public Text expTxt;
    public Image expPrgImage;
    public Text powerTxt;
    public Image powerPrgImage;
    public Text jobTxt;
    public Text fightTxt;
    public Text hpTxt;
    public Text hurtTxt;
    public Text defTxt;
    public RawImage charImage;

    public Button closeBtn;
    public Button closeDetailBtn;
    public Button detailBtn;
    public Transform detailTrans;

    public Text dthp;
    public Text dtad;
    public Text dtap;
    public Text dtaddef;
    public Text dtapdef;
    public Text dtdodge;
    public Text dtpierce;
    public Text dtcritical;
    #endregion

    protected override void InitWin()
    {
        base.InitWin();
        RegisterTouchEvts();
        // 默认将细节面板关闭
        SetActive(detailTrans, false);
        RefreshUI();
    }

    private Vector2 startPos;
    private void RegisterTouchEvts(){
        OnClickDown(charImage.gameObject, (PointerEventData evt) =>{
            startPos = evt.position;
            // 获得初始旋转角度
            MainCitySys.instance.SetPlayerStartRoate();
        });
        OnDrag(charImage.gameObject, (PointerEventData evt) =>{
            float rotate = -(evt.position.x - startPos.x) * 0.4f;
            // 累加旋转距离
            MainCitySys.instance.SetPlayerRotate(rotate);
        });
    }

    private void RefreshUI(){
        PlayerData pd = GameRoot.instance.PlayerData;
        SetText(infoTxt, pd.name);
        SetText(expTxt, pd.exp + "/" + PECommon.GetExpLevelUp(pd.level));
        expPrgImage.fillAmount = pd.exp * 1.0f/PECommon.GetExpLevelUp(pd.level);
        SetText(powerTxt, pd.power + "/" + PECommon.GetPowerLimit(pd.level));
        powerPrgImage.fillAmount = pd.power * 1.0f/PECommon.GetPowerLimit(pd.level);
        SetText(jobTxt, "职业   暗夜刺客");
        SetText(fightTxt, "战力   " + PECommon.GetFight(pd));
        SetText(hpTxt, "血量   " + pd.hp);
        SetText(hurtTxt, "伤害   " + (pd.ad + pd.ap));
        SetText(defTxt, "防御   " + (pd.addef + pd.apdef));

        // 细节面板数据
        SetText(dthp, pd.hp);
        SetText(dtad, pd.ad);
        SetText(dtap, pd.ap);
        SetText(dtaddef, pd.addef);
        SetText(dtapdef, pd.apdef);
        SetText(dtdodge, pd.dodge + "%");
        SetText(dtpierce, pd.pierce + "%");
        SetText(dtcritical, pd.critical + "%");
    }

    // 关闭信息窗口
    public void ClickCloseBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        MainCitySys.instance.CloseInfoWin();
    }

    // 打开细节面板
    public void ClickOpenDetailBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        SetActive(detailTrans);
    }

    // 关闭细节面板
    public void ClickCloseDetailBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        SetActive(detailTrans, false);
    }
}
