using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Scout : BaseAiController
{
    IState patrolState;
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

            patrolState = new PatrolState(this, patrolLocations, AiTargeter);
            chaseState = new ChaseState(this, AiTargeter, targetEnemy);
            attackState = new AttackState(this, tankGun);

            createTransition(chaseState, patrolState, seesPlayer);
            createTransition(attackState, chaseState, canAttack);
            createTransition(chaseState, attackState, tankGun.cantFire);

            stateMachine.SetState(patrolState);
        }

        stateMachine.Tick();
    }
}
