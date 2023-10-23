using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private string stateName = "guardState";
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