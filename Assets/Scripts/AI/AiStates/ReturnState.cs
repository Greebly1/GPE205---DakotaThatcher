using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnState : IState
{
    private string stateName = "returnState";

    private BaseAiController _controller;

    private GameObject AiTargeter;

    private Transform guardPost;

    public ReturnState(BaseAiController controller, GameObject AiTarget, Transform GuardPost)
    {
        _controller = controller;
        AiTargeter = AiTarget;
        guardPost = GuardPost;
    }

    void IState.onBegin()
    {
        _controller.turnPower = _controller.returnStateTurnPower;
        _controller.throttlePower = _controller.returnStateThrottlePower;
        _controller.moveToPrecision = _controller.returnStateMoveToPrecision;
    }

    void IState.onEnd()
    {
        _controller.setDefaultMovementValues();
    }

    string IState.stateName()
    {
        return stateName;
    }

    void IState.tick()
    {
        //Debug.Log("Doing return state");
        AiTargeter.transform.position = guardPost.position;
        _controller.seek(AiTargeter.transform.position, _controller.moveToPrecision);
    }
}
