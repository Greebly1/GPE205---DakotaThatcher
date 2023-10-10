using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AiController : Controller
{
    #region variables

    public TankMovement pawnMovement;

    public float turningErrorMargin;
    public enum AIState {Guard, Chase, Attack, Return};

    public AIState currentState;

    private float lastStateChange;

    public GameObject target;

    private Vector3 goalPosition;
    #endregion

    // Start is called before the first frame update
    public override void Start()
    {   
        base.Start();
        goalPosition = transform.position;
        pawnMovement = pawn.GetComponent<TankMovement>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        seek(target.transform.position);
    }

    protected void makeDecision()
    {
        Debug.Log("decision!");
    }

    public virtual void changeState(AIState newstate)
    {
        currentState = newstate;
        lastStateChange = Time.time;

    }

    //Move the tank towards a target this frame
    private void seek(Vector3 targetPosition)
    {
        float CurrentAngle = pawnMovement.angleToTarget(targetPosition);
        //if the pawn is not facing the targetposition
        if ( MathF.Abs(CurrentAngle) > turningErrorMargin)
        {
            float turnPower = 1;
            if (CurrentAngle > 0)
            {
                pawnMovement.turnInput = turnPower;
                //Turn Right
            }
            else if (CurrentAngle < 0)
            {
                pawnMovement.turnInput = turnPower * -1;
                //Turn left
            }
        } else pawnMovement.turnInput = 0;
        //pawnMovement.setThrottle(0.4f);

        //if (pawnMovement.SimulateMovement(targetPosition, 0f, 10f).)
        //accelerate forwards if it will make the movement direction closer to the targetdirection
        //brake if ---- this tank is going to overshoot 

    }
    
    #region macros
    /*
    private bool willOvershoot(simulationResult movementSim, Vector3 targetPosition, float time = 0)
    {
        //if the current distance from the target is smaller than the distance from the target to where the tank will be when it stops
        if (distanceFromTarget(targetPosition) < (targetPosition - (pawn.transform.position + movementSim.getChangeInPosition())).magnitude)
        {
            return true;
        }
        return false;
    }
    */
    /*private  bool isAccelerationValid(Vector3 goalDirection, Vector3 forwardVector)
    {

    }*/

    private Vector3 targetVector(Vector3 targetPosition)
    {
        return targetPosition - pawn.transform.position;
    }

    private Vector3 targetdirection(Vector3 targetPosition)
    {
        return targetVector(targetPosition).normalized;
    }

    private float distanceFromTarget(Vector3 targetPosition)
    {
        return targetVector(targetPosition).magnitude;
    }
    #endregion
}
