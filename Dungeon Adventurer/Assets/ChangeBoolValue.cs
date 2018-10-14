using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeBoolValue : StateMachineBehaviour {

    [SerializeField]
    private bool _onStateEnter;

    [SerializeField]
    private bool _onAnimationFinished;

    [SerializeField]
    private bool _onStateExit;

    [SerializeField]
    private string _name;

    [SerializeField]
    private bool _boolValue;

    private bool _done = false;
    private float _timeFinished;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _timeFinished = Time.time + stateInfo.length;

        if (_onStateEnter)
            animator.SetBool(_name, _boolValue);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!_done && _onAnimationFinished && Time.time >=_timeFinished)
        {
            animator.SetBool(_name, _boolValue);
            _done = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_onStateExit)
            animator.SetBool(_name, _boolValue);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}
}
