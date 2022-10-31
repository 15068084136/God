using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    Transform camTrans;
    Vector3 camOffset;
    public CharacterController characterController;
    float targetBlend;
    float currentBlend;
    public Animator animator;
    Vector2 dir;
    protected bool isMove;

    // 通过dir属性来改变isMove
    public Vector2 Dir { get => dir; set{
        if(value == Vector2.zero){
            isMove = false;
        }else{
            isMove = true;
        }
        dir = value;
    }}
    void Start()
    {
        characterController.enabled = true;
        Init();
    }

    public void Init() {
        camTrans = Camera.main.transform;
        // 摄像机偏移量
        camOffset = transform.position - camTrans.position;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 _dir = new Vector2(h, v).normalized;
        if(_dir != Vector2.zero){
            Dir = _dir;
            SetBlend(1);
        }else{
            Dir = Vector2.zero;
            SetBlend(0);
        }

        // 平滑动画
        if(currentBlend != targetBlend){
            UpdateMixBlend();
        }

        if(isMove){
            SetDir();
            SetMove();
            SetCam();
        }
    }

    public void SetCam()
    {
        if(camTrans != null){
            camTrans.position = transform.position - camOffset;
        }
    }

    private void SetMove()
    {
        characterController.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
    }

    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1))+ camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    public void SetBlend(float blend){
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
}
