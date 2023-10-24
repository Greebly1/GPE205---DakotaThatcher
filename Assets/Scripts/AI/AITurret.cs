using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITurret : BaseAiController
{
    IState turnToState;
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

            turnToState = new TurnToState(this, this.AiTargeter, this.targetEnemy);
            attackState = new AttackState(this, this.tankGun);

            createTransition(attackState, turnToState, canAttack);
            createTransition(turnToState, attackState, tankGun.cantFire);

            stateMachine.SetState(turnToState);
        }

        stateMachine.Tick();
    }
}
