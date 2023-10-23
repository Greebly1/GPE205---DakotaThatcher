using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private string stateName = "idleState";
    void IState.onBegin()
    {

    }

    void IState.onEnd()
    {

    }

    string IState.stateName()
    {
        return stateName;
    }

    void IState.tick()
    {

    }
}
