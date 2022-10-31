using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表现实体角色控制器
/// </summary>
public class PlayerController : Controller
{
    public GameObject skill_1Fx;// 技能1特效
    public GameObject skill_2Fx;// 技能1特效
    public GameObject skill_3Fx;// 技能1特效


    public GameObject Normal1Fx;// 普攻1特效
    public GameObject Normal2Fx;// 普攻2特效
    public GameObject Normal3Fx;// 普攻3特效
    public GameObject Normal4Fx;// 普攻4特效
    public GameObject Normal5Fx;// 普攻5特效



    Vector3 camOffset;

    
    float targetBlend;
    float currentBlend;

    public override void Init() {
        base.Init();
        camTrans = Camera.main.transform;
        // 摄像机偏移量
        camOffset = transform.position - camTrans.position;

        if(skill_1Fx != null){
            fxDic.Add(skill_1Fx.name, skill_1Fx);
        }
        if(skill_2Fx != null){
            fxDic.Add(skill_2Fx.name, skill_2Fx);
        }
        if(skill_3Fx != null){
            fxDic.Add(skill_3Fx.name, skill_3Fx);
        }
        if(Normal1Fx != null){
            fxDic.Add(Normal1Fx.name, Normal1Fx);
        }
        if(Normal2Fx != null){
            fxDic.Add(Normal2Fx.name, Normal2Fx);
        }
        if(Normal3Fx != null){
            fxDic.Add(Normal3Fx.name, Normal3Fx);
        }
        if(Normal4Fx != null){
            fxDic.Add(Normal4Fx.name, Normal4Fx);
        }
        if(Normal5Fx != null){
            fxDic.Add(Normal5Fx.name, Normal5Fx);
        }
    }

    void Update()
    {
        // float h = Input.GetAxis("Horizontal");
        // float v = Input.GetAxis("Vertical");
        // Vector2 _dir = new Vector2(h, v).normalized;
        // if(_dir != Vector2.zero){
        //     Dir = _dir;
        //     SetBlend(1);
        // }else{
        //     Dir = Vector2.zero;
        //     SetBlend(0);
        // }

        // 平滑动画
        if(currentBlend != targetBlend){
            UpdateMixBlend();
        }

        if(isMove){
            SetDir();
            if(!GetIsPauseGame()){
                SetMove();
            }
            SetCam();
        }
        if(skillMove){
            SetSkillMove();
            SetCam();
        }
    }

    public void SetCam()
    {
        if(camTrans != null){
            camTrans.position = transform.position - camOffset;
        }
    }

    private bool GetIsPauseGame(){
        if(battleManager == null){
            return false;
        }else{
            return battleManager.isPauseGame;
        }
    }

    private void SetMove()
    {
        characterController.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
    }
    private void SetSkillMove()
    {
        characterController.Move(transform.forward * Time.deltaTime * skillMoveSpeed);
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1))+ camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    public override void SetBlend(float blend){
        targetBlend = blend;
    }
    // 运动平滑
    private void UpdateMixBlend(){
        // 一帧之内完成
        if(MathF.Abs(currentBlend - targetBlend) < Constants.AcceleSpeed * Time.deltaTime){
            currentBlend = targetBlend;
        }else if(currentBlend > targetBlend){
            currentBlend -= Constants.AcceleSpeed * Time.deltaTime;
        }else{
            currentBlend += Constants.AcceleSpeed * Time.deltaTime;
        }
        animator.SetFloat("Blend", currentBlend);
    }

    public override void SetFx(string name, float destroy)
    {
        GameObject go;
        if(fxDic.TryGetValue(name, out go)){
            go.SetActive(true);
            // 一段时间后关闭特效
            timerService.AddTimeTask((int tid)=>{
                go.SetActive(false);
            }, destroy);
        }   
    }
}
