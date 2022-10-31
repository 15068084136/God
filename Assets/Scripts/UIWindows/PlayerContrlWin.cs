using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using PEProtocal;

/// <summary>
/// 玩家控制界面
/// </summary>
public class PlayerContrlWin : WinRoot
{
    public Image dirTouchBg;
    public Image dirImage;
    public Image dirPointImage;
    
    public Text levelTxt;
    public Text nameTxt;
    public Text expTxt;
    public Transform expPrgTrans;
    public Text hpTxt;
    public Image hpPrgs;

    private Vector2 startPos = Vector2.zero;
    private Vector2 defaultPos;

    public Vector2 currentDir;// 当前向量

    private int HPSum;// 初始血量
    protected override void InitWin()
    {
        base.InitWin();

        defaultPos = dirTouchBg.transform.position;
        // 初始状态下小点是不显示的
        SetActive(dirPointImage, false);

        HPSum = GameRoot.instance.PlayerData.hp;
        SetText(hpTxt, HPSum + "/" + HPSum);
        
        hpPrgs.fillAmount = 1;

        RegisterTouchEvent();

        skill_1CDTime = resService.GetSkillCfg(101).cdTime / 1000.0f;
        skill_2CDTime = resService.GetSkillCfg(102).cdTime / 1000.0f;
        skill_3CDTime = resService.GetSkillCfg(103).cdTime / 1000.0f;


        RefreshUI();

        // 关闭Boss血条UI
        SetBossHPBarState(false);
    }

    private void Update() {
        float delta = Time.deltaTime;
        #region 技能CD
        if(isSkill_1CD){
            skill_1FillCount += delta;
            if(skill_1FillCount >= skill_1CDTime){
                isSkill_1CD = false;
                SetActive(skill_1CDImage, false);
                skill_1FillCount = 0;
            }else{
                // 如果没到CD时间
                skill_1CDImage.fillAmount = 1 - skill_1FillCount / skill_1CDTime;
            }
            skill_1NumCount += delta;
            if(skill_1NumCount >= 1){
                skill_1NumCount -= 1;
                skill_1Num -= 1;
                SetText(skill_1CDTxt, skill_1Num);
            }
        }
        if(isSkill_2CD){
            skill_2FillCount += delta;
            if(skill_2FillCount >= skill_2CDTime){
                isSkill_2CD = false;
                SetActive(skill_2CDImage, false);
                skill_2FillCount = 0;
            }else{
                // 如果没到CD时间
                skill_2CDImage.fillAmount = 1 - skill_2FillCount / skill_2CDTime;
            }
            skill_2NumCount += delta;
            if(skill_2NumCount >= 1){
                skill_2NumCount -= 1;
                skill_2Num -= 1;
                SetText(skill_2CDTxt, skill_2Num);
            }
        }
        if(isSkill_3CD){
            skill_3FillCount += delta;
            if(skill_3FillCount >= skill_3CDTime){
                isSkill_3CD = false;
                SetActive(skill_3CDImage, false);
                skill_3FillCount = 0;
            }else{
                // 如果没到CD时间
                skill_3CDImage.fillAmount = 1 - skill_3FillCount / skill_3CDTime;
            }
            skill_3NumCount += delta;
            if(skill_3NumCount >= 1){
                skill_3NumCount -= 1;
                skill_3Num -= 1;
                SetText(skill_3CDTxt, skill_3Num);
            }
        }
        #endregion

        if(BossHPRootTrans.gameObject.activeSelf){
            BlendBossHP();
            yellowImage.fillAmount = currentPrg;
        }
    }

    // 刷新UI控件
    public void RefreshUI(){
        PlayerData pd = GameRoot.instance.PlayerData;
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
    }
    #region 点击事件
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
            // 先中转至battlesys，再转至BattleMgr
            currentDir = Vector2.zero;
            BattleSys.instance.SetPlayerMoveDir(currentDir);
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
            currentDir = dir.normalized; // 记录当前拖拽向量
            BattleSys.instance.SetPlayerMoveDir(currentDir);
        });
    }
    public void ClickNormalAtk(){
        BattleSys.instance.ReqReleaseSkill(0);
    }
    #region SK1
    public Image skill_1CDImage;
    public Text skill_1CDTxt;
    private bool isSkill_1CD;// 是否在CD时间内
    private float skill_1CDTime;
    private int skill_1Num;// 当前还剩多少秒CD
    private float skill_1FillCount;// CD进行了多少秒
    private float skill_1NumCount;// CD的文字跳动的频率，为1秒
    #endregion

    #region SK2
    public Image skill_2CDImage;
    public Text skill_2CDTxt;
    private bool isSkill_2CD;// 是否在CD时间内
    private float skill_2CDTime;
    private int skill_2Num;// 当前还剩多少秒CD
    private float skill_2FillCount;// CD进行了多少秒
    private float skill_2NumCount;// CD的文字跳动的频率，为1秒
    #endregion

    #region SK3
    public Image skill_3CDImage;
    public Text skill_3CDTxt;
    private bool isSkill_3CD;// 是否在CD时间内
    private float skill_3CDTime;
    private int skill_3Num;// 当前还剩多少秒CD
    private float skill_3FillCount;// CD进行了多少秒
    private float skill_3NumCount;// CD的文字跳动的频率，为1秒
    #endregion
    public void Clicskill_1Atk(){
        if(isSkill_1CD == false && GetCanRisSkill()){
            BattleSys.instance.ReqReleaseSkill(1);
            isSkill_1CD = true;
            SetActive(skill_1CDImage);
            skill_1CDImage.fillAmount = 1;
            skill_1Num = (int)skill_1CDTime;
            SetText(skill_1CDTxt, skill_1Num);
        }
    }
    public void Clickskill_2Atk(){
        if(isSkill_2CD == false && GetCanRisSkill()){
            BattleSys.instance.ReqReleaseSkill(2);
            isSkill_2CD = true;
            SetActive(skill_2CDImage);
            skill_2CDImage.fillAmount = 1;
            skill_2Num = (int)skill_2CDTime;
            SetText(skill_2CDTxt, skill_2Num);
        }
    }
    public void Clickskill_3Atk(){
        if(isSkill_3CD == false && GetCanRisSkill()){
            BattleSys.instance.ReqReleaseSkill(3);
            isSkill_3CD = true;
            SetActive(skill_3CDImage);
            skill_3CDImage.fillAmount = 1;
            skill_3Num = (int)skill_3CDTime;
            SetText(skill_3CDTxt, skill_3Num);
        }
    }
    // 点击头像按钮进入暂停界面
    public void ClickHeadBtn(){
        BattleSys.instance.battleManager.isPauseGame = true;
        BattleSys.instance.SetBattleEndWinState(FBEndType.pause);
    }
    #endregion
    // 更新血量数据
    public void SetHPVal(int value){
        
        SetText(hpTxt, value + "/" + HPSum);
        hpPrgs.fillAmount = value * 1.0f / HPSum;
    }
    public bool GetCanRisSkill(){
        return BattleSys.instance.battleManager.CanRlsSkill();
    }

    #region Boss血条
    public Transform BossHPRootTrans;
    public Image yellowImage;
    public Image redImage;
    private float currentPrg = 1f;
    private float targetPrg = 1f;
    public void SetBossHPBarState(bool state, float prg = 1){
        SetActive(BossHPRootTrans, state);
        redImage.fillAmount = prg;
        yellowImage.fillAmount = prg;
    }
    public void SetBossHPBarVal(int oldVal, int newVal, int sumVal){
        currentPrg = oldVal * 1.0f / sumVal;
        targetPrg = newVal * 1.0f / sumVal;
        redImage.fillAmount = targetPrg;
    }
    // Boss血条平滑混合
    private void BlendBossHP(){
        if(Mathf.Abs(currentPrg - targetPrg) < Constants.AcceleHpSpeed * Time.deltaTime){
            currentPrg = targetPrg;
        }else if(currentPrg > targetPrg){
            currentPrg -= Constants.AcceleHpSpeed * Time.deltaTime;
        }else{
            currentPrg += Constants.AcceleHpSpeed * Time.deltaTime;
        }
    }
    #endregion


}
