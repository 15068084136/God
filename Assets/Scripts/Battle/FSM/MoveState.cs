using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : IState
{
    public void Enter(EntityBase entity, params object[] args){
        entity.currentState = AniState.move;
    }
    public void Process(EntityBase entity, params object[] args){
        entity.SetBlend(Constants.BlendMove);
    }
    public void Exit(EntityBase entity, params object[] args){
    }
}
