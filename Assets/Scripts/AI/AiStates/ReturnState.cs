using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnState : IState
{
    private string stateName = "returnState";
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
