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
        senses.lostEnemy += enemyGone;
    }

    public override void Update()
    {

        stateMachine.Tick();
    }

    public void enemyGone(List<PlayerController> targets)
    {
        Debug.Log("enemygone executed" + targets);
        switch (targets.Count)
        {
            case 0:
                targetEnemy = null;
                stateMachine.SetState(idleState);
                break;
            case 1:
                targetEnemy = targets[0].pawn.gameObject;
                stateMachine.SetState(chaseState);
                break;
            case 2: //this code block should never execute
                Debug.LogWarning("The ai targeter is executing enemy gone, but it's target list has both players in it");
                targetEnemy = targets[0].pawn.gameObject;
                stateMachine.SetState(chaseState);
                break;
        }

        
    }

    public void seenEnemy(List<PlayerController> targets)
    {
        Debug.Log("seenEnemy executed" + targets);
        switch (targets.Count)
        {
            case 0: //This code block should never execute
                Debug.LogWarning("The ai targeter is executing seenEnemy, but it's target list is empty"); 
                return; //early out if no enemies
            case 1:
                targetEnemy = targets[0].pawn.gameObject;
                stateMachine.SetState(chaseState);
                break;
            case 2:
                targetEnemy = targets[0].pawn.gameObject;
                break;
        } 
    }
}
