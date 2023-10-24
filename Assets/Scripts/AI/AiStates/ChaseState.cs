using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    private string stateName = "chaseState";

    private BaseAiController _controller;
    private GameObject AiTargeter;
    private GameObject targetEnemy;

    public ChaseState(BaseAiController controller, GameObject aiTargeter, GameObject TargetEnemy)
    {
        _controller = controller;
        AiTargeter = aiTargeter;
        targetEnemy = TargetEnemy;
    }

    void IState.onBegin()
    {
        _controller.throttlePower = _controller.chaseStateThrottlePower;
        _controller.turnPower = _controller.chaseStateTurnPower;
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
        //Debug.Log("Doing chase state");
        if (targetEnemy != null)
        {
            AiTargeter.transform.position = targetEnemy.transform.position;
        }
        else
        {
            Debug.LogWarning("target enemy is not set in the chase state");
        }
        
        _controller.seek(AiTargeter.transform.position, _controller.desiredAttackRange, 0.8f, 1);
    }
}