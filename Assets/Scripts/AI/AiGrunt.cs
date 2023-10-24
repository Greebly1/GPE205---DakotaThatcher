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
    }

    public override void Update()
    {
        if (targetEnemy == null)
        {
            targetEnemy = GameManager.Game.player.pawn.gameObject;

            idleState = new IdleState(this);
            chaseState = new ChaseState(this, this.AiTargeter, this.targetEnemy);
            attackState = new AttackState(this, this.tankGun);

            createTransition(chaseState, idleState, seesPlayer);
            createTransition(attackState, chaseState, canAttack);
            createTransition(chaseState, attackState, tankGun.cantFire);

            stateMachine.SetState(idleState);
        }

        stateMachine.Tick();
    }
}
