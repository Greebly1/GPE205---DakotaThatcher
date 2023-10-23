using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private string stateName = "patrolState";
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
