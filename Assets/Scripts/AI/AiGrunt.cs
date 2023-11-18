using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGrunt : BaseAiController
{
    IState idleState;
    IState chaseState;
    IState attackState;

    public override void Start()
    {
        base.Start();

        idleState = new IdleState(this);
        chaseState = new ChaseState(this, this.AiTargeter);
        attackState = new AttackState(this, this.tankGun);

        createTransition(attackState, chaseState, canAttack);
        createTransition(chaseState, attackState, tankGun.cantFire);

        stateMachine.SetState(idleState);

        senses.sawEnemy += seenEnemy;
    }

    public override void Update()
    {

        stateMachine.Tick();
    }

    public void seenEnemy(GameObject target)
    {
        targetEnemy = target;
        stateMachine.SetState(chaseState);
    }
}
