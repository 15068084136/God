using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : IState
{
    public void Enter(EntityBase entity, params object[] args){
        entity.currentState = AniState.hit;

        // 删除所有回调函数
        for (int i = 0; i < entity.skillMoveCBList.Count; i++)
        {
            int tid = entity.skillMoveCBList[i];
            TimerService.Instance.DeleteTask(tid);
        }
        for (int i = 0; i < entity.skillActionCBList.Count; i++)
        {
            int tid = entity.skillActionCBList[i];
            TimerService.Instance.DeleteTask(tid);
        }
    }
    public void Process(EntityBase entity, params object[] args){
        if(entity.entityType == EntityType.player){
            entity.canRlsSkill = false;
        }
        // 停止运动
        entity.SetDir(Vector2.zero);
        // 播放动画
        entity.SetAction(Constants.ActionHit);
        // 停止技能运动
        entity.GetController().skillMove = false;

        // 播放音效
        if(entity.entityType == EntityType.player){
            AudioSource charAudio = entity.GetAudioSource();
            AudioClip clip = ResService.instance.LoadAudio("ResAudio/" + Constants.assassinHit, true);
            charAudio.clip = clip;
            charAudio.Play();
        }

        TimerService.Instance.AddTimeTask((int tid)=>{
            entity.SetAction(Constants.ActionDefault);
            entity.Idle();
        }, (int)(GetHitAniLen(entity) * 1000));
    }
    public void Exit(EntityBase entity, params object[] args){
    }

    private float GetHitAniLen(EntityBase entity){
        AnimationClip[] clips = entity.GetAniClips();
        for (int i = 0; i < clips.Length; i++)
        {
            string clipName = clips[i].name;
            if(clipName.Contains("hit")||
            clipName.Contains("Hit")||
            clipName.Contains("HIT")){
                return clips[i].length;
            }
        }
        // 保护值
        return 1;
    }
}
