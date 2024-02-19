using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Chaser : BaseAiController
{
    IState chaseState;

    [SerializeField] GameObject chasetarget;

    public override void Update()
    {
        if (targetEnemy == null)
        {

            try
            {
                targetEnemy = GameManager.Game.player1.pawn.gameObject;
            }
            catch (NullReferenceException ex)
            {
                Debug.LogWarning(ex.Message);
                targetEnemy = chasetarget;
            }

            chaseState = new ChaseState(this, AiTargeter);

            stateMachine.SetState(chaseState);
        }

        stateMachine.Tick();
    }
}
