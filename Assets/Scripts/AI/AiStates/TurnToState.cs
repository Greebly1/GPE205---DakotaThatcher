using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToState : IState
{
    private string Name = "TurnToState";
    private BaseAiController _controller;
    private GameObject AiTargeter;
    private GameObject Target;

    public TurnToState(BaseAiController controller, GameObject AiTarget, GameObject target)
    {
        _controller = controller;
        AiTargeter = AiTarget;
        Target = target;
    }
    
    public void onBegin()
    {
        
    }

    public void onEnd()
    {
        
    }

    public string stateName()
    {
        return Name;
    }

    public void tick()
    {
        AiTargeter.transform.position = Target.transform.position;
        _controller.turnTo(AiTargeter.transform.position);
    }
}
