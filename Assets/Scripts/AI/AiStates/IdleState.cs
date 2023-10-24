using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private string stateName = "idleState";

    private BaseAiController _controller;

    public IdleState(BaseAiController controller)
    {
        _controller = controller;
    }

    void IState.onBegin()
    {
        //Debug.Log("Idle state began");
    }

    void IState.onEnd()
    {
        //Debug.Log("Idle state ended");
    }

    string IState.stateName()
    {
        return stateName;
    }

    void IState.tick()
    {
        //Debug.Log("Doing idle state");
    }
}
