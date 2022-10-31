using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : StateMachineBehaviour
{
    CharacterController characterController;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       characterController = animator.gameObject.GetComponent<CharacterController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.SetFloat("Blend", 0f);
       animator.SetTrigger("start");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       characterController.enabled = true;
    }
}
