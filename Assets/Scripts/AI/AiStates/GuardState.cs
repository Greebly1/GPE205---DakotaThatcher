using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardState : IState
{
    private string stateName = "guardState";
    private BaseAiController _controller;
    private GameObject AiTargeter;
    private bool startingGuardState;
    private bool lastTurnedRight;

    public GuardState(BaseAiController controller, GameObject AiTarget)
    {
        _controller = controller;
        AiTargeter = AiTarget;
    }
    
    void IState.onBegin()
    {
        startingGuardState = true;
        lastTurnedRight = true;
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
        float turnAmount = 170.0f;
        if (startingGuardState)
        {
            turnAmount /= 2;
        }
        //Debug.Log("Doing guard state");
        if (_controller.isFacing(AiTargeter.transform.position))
        {
            AiTargeter.transform.position = _controller.pawn.transform.position + _controller.newTurnTarget(turnAmount, lastTurnedRight);
            lastTurnedRight = !lastTurnedRight;
            startingGuardState=false;
        }
        _controller.turnTo(AiTargeter.transform.position);
    }
}
