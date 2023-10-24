using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private string stateName = "attackState";

    private BaseAiController _controller;

    private Shooter_Cannon shooter;

    public AttackState(BaseAiController controller, Shooter_Cannon shooter)
    {
        _controller = controller;
        this.shooter = shooter;
    }

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
        shooter.tryShoot();
    }
}