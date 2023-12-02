using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ai_Guard : BaseAiController
{
    IState guardState;
    IState chaseState;
    IState attackState;
    IState returnState;

    public Transform _guardPost;
    public float maxDistanceFromPost = 200;

    public override void Start()
    {
        base.Start();

        if (_guardPost == null ) Debug.LogWarning("AI guard does not have a post");

    }

    public override void Update()
    {
        if (targetEnemy == null)
        {
            targetEnemy = GameManager.Game.player1.pawn.gameObject;

            guardState = new GuardState(this,AiTargeter);
            returnState = new ReturnState(this, AiTargeter, _guardPost);
            chaseState = new ChaseState(this, AiTargeter);
            attackState = new AttackState(this, tankGun);

            createTransition(guardState, returnState, isAtPost);
            createTransition(chaseState, guardState, seesPlayer);
            createTransition(attackState, chaseState, canAttack);
            createTransition(chaseState, attackState, tankGun.cantFire);
            createTransition(returnState, chaseState, tooFarFromPost);

            stateMachine.SetState(returnState);
        }

        stateMachine.Tick();
    }

    public bool isAtPost()
    {
        return Vector3.Distance(_guardPost.transform.position, this.pawn.transform.position) < moveToPrecision;
    }

    public bool tooFarFromPost()
    {
        return Vector3.Distance(pawn.transform.position, _guardPost.transform.position) > maxDistanceFromPost;
    }

}
