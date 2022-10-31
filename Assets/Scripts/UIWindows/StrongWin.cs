using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PEProtocal;


/// <summary>
/// 强化界面
/// </summary>
public class StrongWin : WinRoot
{
    public Transform posBtnTrans;
    // Pos图片
    private Image[] images = new Image[6];
    // 当前点击的Pos图片序号
    public int currentIndex;
    private PlayerData pd;

    #region UIDefine
    // 当前选择的强化图片
    public Image currentImagePos;
    public Text startLvTxt;
    // 星星的父物体
    public Transform starTransGroup;
    // 具体属性
    public Text propHp1;
    public Text propHp2;
    public Text propHurt1;
    public Text propHurt2;
    public Text propDef1;
    public Text propDef2;
    public Image propArr1;
    public Image propArr2;
    public Image propArr3;
    // 所需级别、金币、水晶
    public Text needLvTxt;
    public Text costCoinTxt;
    public Text costCryTxt;
    // 当前所剩余的金币数量
    public Text coinTxt;
    // 所需物资的根物体
    public Transform costTransRoot;
    // 下一级别的强化数据
    StrongCfg nextSd;
    #endregion

    protected override void InitWin()
    {
        base.InitWin();
        pd = GameRoot.instance.PlayerData;
        RegClickEvts();

        // 默认是点击第一个强化图片
        ClickPosItem(0);
    }

    private void RegClickEvts()
    {
        for (int i = 0; i < posBtnTrans.childCount; i++)
        {
            Image image = posBtnTrans.GetChild(i).GetComponent<Image>();

            OnClick(image.gameObject, (object args) =>{
                audioServer.PlayUIAudio(Constants.UIClickButton);
                ClickPosItem((int)args);
            }, i);
            images[i] = image;
        }
    }

    private void ClickPosItem(int index){
        PECommon.Log("点击成功！是第" + index + "个按钮");

        currentIndex = index;
        for (int i = 0; i < images.Length; i++)
        {
            Transform transform = images[i].transform;
            if(i == currentIndex){
                // 箭头显示
                SetSprite(images[i], Constants.arrowBg);
            }else{
                // 平板显示
                SetSprite(images[i], Constants.platBg);
            }
        }
        RefreshItem();
    }

    public void ClickCloseBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        SetWinState(false);
    }

    // 点击某个强化图标后，会显示相应界面
    public void RefreshItem(){
        // 剩余金币
        SetText(coinTxt, pd.coin);
        switch (currentIndex)
        {
            case 0:
                SetSprite(currentImagePos, Constants.touKuiItem);
                break;
            case 1:
                SetSprite(currentImagePos, Constants.bodyItem);
                break;
            case 2:
                SetSprite(currentImagePos, Constants.yaobuItem);
                break;
            case 3:
                SetSprite(currentImagePos, Constants.handItem);
                break;
            case 4:
                SetSprite(currentImagePos, Constants.legItem);
                break;
            case 5:
                SetSprite(currentImagePos, Constants.footItem);
                break;
        }
        //  设置当前位置的星级
        SetText(startLvTxt, pd.strongArr[currentIndex] + "星级");

        // 设置小星星数量
        int currentStartLv = pd.strongArr[currentIndex];
        for (int i = 0; i < starTransGroup.childCount; i++)
        {
            Image image = starTransGroup.GetChild(i).GetComponent<Image>();
            if(i < currentStartLv){
                SetSprite(image, Constants.star);
            }else{
                SetSprite(image, Constants.grayStar);
            }
        }

        // 获取下一级别的增加属性
        int nextStartLv = currentStartLv + 1;
        // 设置累加的属性
        int sumAddHp = resService.GetStrongVal(currentIndex, nextStartLv, 1);
        int sumAddHurt = resService.GetStrongVal(currentIndex, nextStartLv, 2);
        int sumAddDef = resService.GetStrongVal(currentIndex, nextStartLv, 3);
        SetText(propHp1, "生命 +" + sumAddHp);
        SetText(propHurt1, "伤害 +" + sumAddHurt);
        SetText(propDef1, "防御 +" + sumAddDef);

        nextSd = resService.GetStrongData(currentIndex, nextStartLv);
        if(nextSd != null){
            SetActive(propHp2);
            SetActive(propHurt2);
            SetActive(propDef2);
            SetActive(costTransRoot);
            SetActive(propArr1);
            SetActive(propArr2);
            SetActive(propArr3);

            SetText(propHp2, "强化后 +" + nextSd.addHp);
            SetText(propHurt2, "强化后 +" + nextSd.addHurt);
            SetText(propDef2, "强化后 +" + nextSd.addDef);
            SetText(needLvTxt, "需要等级：" + nextSd.minLv);
            SetText(costCoinTxt, "需要消耗：      " + nextSd.coin);
            SetText(costCryTxt, nextSd.crystal + "/" + pd.crystal);
        }else{
            // 如果下一级别超过了最高级别，就无法获得强化数据了
            SetActive(propHp2, false);
            SetActive(propHurt2, false);
            SetActive(propDef2, false);
            SetActive(costTransRoot, false);
            SetActive(propArr1, false);
            SetActive(propArr2, false);
            SetActive(propArr3, false);
        }
    }

    // 点击强化按钮
    public void ClickStrongBtn(){
        audioServer.PlayUIAudio(Constants.UIClickButton);
        // 给服务器发送数据之前，先在客户端进行一次数据过滤
        // 如果当前装备强化等级小于最高等级才能进行发送网络消息
        if(pd.strongArr[currentIndex] < 10){
            if(pd.level < nextSd.minLv){
                GameRoot.AddTips("角色等级不够");
                return;
            }
            if(pd.coin < nextSd.coin){
                GameRoot.AddTips("金币数量不够");
                return;
            }
            if(pd.crystal < nextSd.crystal){
                GameRoot.AddTips("水晶不够");
                return;
            }
            netService.SendMsg(new GameMsg{
                cmd = (int)Command.reqStrong,
                reqStrong = new ReqStrong{
                    pos = currentIndex
                }
            });
        }else{
            GameRoot.AddTips("星级已经满级");
        }
    }

    // 更新strong强化detailUI界面
    public void UpdateStrongDetailWin(){
        audioServer.PlayUIAudio(Constants.fbItem);
        // 相当于重新点击了一次当前的强化图标
        ClickPosItem(currentIndex);
    }
}
