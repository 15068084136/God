using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 血条UI
/// </summary>
public class HpItem : MonoBehaviour
{
    #region UIDefine
    public Image hpGrayImage;
    public Image hpRedImage;

    public Animation criticalAni;
    public Text criticalTxt;

    public Animation dodgeAni;
    public Text dodgeTxt;

    public Animation hpAni;
    public Text hpTxt;
    #endregion

    private int hpVal;
    private RectTransform rect;// 血条的组件
    private Transform rootTrans;// 怪物的坐标

    private void Update() {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(rootTrans.position);
        rect.position = screenPos;

        UpdateMixBlend();
        hpGrayImage.fillAmount = currentPrg;
    }

    private void UpdateMixBlend(){
        if(Mathf.Abs(currentPrg - targetPrg) < Constants.AcceleHpSpeed * Time.deltaTime){
            currentPrg = targetPrg;
        }else if(currentPrg > targetPrg){
            currentPrg -= Constants.AcceleHpSpeed * Time.deltaTime;
        }else{
            currentPrg += Constants.AcceleHpSpeed * Time.deltaTime;
        }
    }

    public void InitItemInfo(Transform trans , int hp){
        rect = transform.GetComponent<RectTransform>();
        rootTrans = trans;
        hpVal = hp;
        hpGrayImage.fillAmount = 1;
        hpRedImage.fillAmount = 1;
    }
    public void SetCritical(int critical){
        criticalAni.Stop();
        criticalTxt.text = "暴击 " + critical;
        criticalAni.Play();
    }
    public void SetDodge(){
        dodgeAni.Stop();
        dodgeTxt.text = "闪避";
        dodgeAni.Play();
    }
    public void SetHurt(int hit){
        hpAni.Stop();
        hpTxt.text = "-" + hit;
        hpAni.Play();
    }
    private float currentPrg;
    private float targetPrg;
    public void SetHpVal(int oldVal, int newVal){
        currentPrg = oldVal * 1.0f/hpVal;
        targetPrg = newVal * 1.0f/hpVal;
        hpRedImage.fillAmount = targetPrg;
    }
}
