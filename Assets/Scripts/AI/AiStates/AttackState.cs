using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private string stateName = "attackState";
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