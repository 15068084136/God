using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表现实体控制器抽象基类
/// </summary>
public abstract class Controller : MonoBehaviour
{
    public Animator animator;
    Vector2 dir;
    protected bool isMove;
    protected TimerService timerService;
    public CharacterController characterController;
    public Transform hpRootTrans;
    public Transform camTrans;
    public BattleManager battleManager;

    protected Dictionary<string, GameObject> fxDic = new Dictionary<string, GameObject>();

    // 通过dir属性来改变isMove
    public Vector2 Dir { get => dir; set{
        if(value == Vector2.zero){
            isMove = false;
        }else{
            isMove = true;
        }
        dir = value;
    }}

    public bool skillMove; // 是否在技能运动
    protected float skillMoveSpeed; // 技能运动的速度

    public virtual void Init(){
        timerService = TimerService.Instance;
    }

    // 玩家覆盖这个方法，怪物直接用，因为不需要丝滑过度动画
    public virtual void SetBlend(float blend){
        animator.SetFloat("Blend", blend);
    }
    // 设置攻击动画
    public virtual void SetAction(int act){
        animator.SetInteger("action", act);
    }
    public virtual void SetFx(string name, float destroy){
        
    }
    public void SetSkillMoveState(bool move, float skillSpeed){
        skillMove = move;
        skillMoveSpeed = skillSpeed;
    }
    public virtual void SetAtkRotationLocal(Vector2 atkDir){
        float angle = Vector2.SignedAngle(atkDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    public virtual void SetAtkRotationCam(Vector2 camDir){
        float angle = Vector2.SignedAngle(camDir, new Vector2(0, 1))+ camTrans.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
}
