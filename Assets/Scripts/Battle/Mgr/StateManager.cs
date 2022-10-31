using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 状态管理器
/// </summary>
public class StateManager : MonoBehaviour
{
    private Dictionary<AniState, IState> fsm = new Dictionary<AniState, IState>();
    public void Init(){
        fsm.Add(AniState.hit, new HitState());
        fsm.Add(AniState.die, new DieState());
        fsm.Add(AniState.born, new BornState());
        fsm.Add(AniState.idle, new IdleState());
        fsm.Add(AniState.move, new MoveState());
        fsm.Add(AniState.attack, new AttackState());
    }

    // 改变状态
    public void ChangeState(EntityBase entity, AniState target, params object[] args){
        if(entity.currentState == target){
            return;
        }
        if(fsm.ContainsKey(target)){
            if(entity.currentState != AniState.none){
                fsm[entity.currentState].Exit(entity, args);
            }
            fsm[target].Enter(entity, args);
            fsm[target].Process(entity, args);
        }
    }
}
