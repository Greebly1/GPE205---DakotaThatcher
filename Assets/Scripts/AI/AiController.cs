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

    public float distanceToStartBraking;

    private bool useBrakes = false;

    private Vector3 goalPosition;

    public struct simulationResult
    {
        public float timeSpan;
        public float distanceFromTarget;
        public Vector3 endVelocity;
        public Vector3 deltaPosition;
        public bool overShoots;
        public simulationResult(float time, float distance, Vector3 finalVelocity, Vector3 positionChange, bool overShootsTarget)
        {
            timeSpan = time;
            distanceFromTarget = distance;
            endVelocity = finalVelocity;
            deltaPosition = positionChange;
            overShoots = overShootsTarget;
        }
    }
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

        pawnMovement.targetPosition = target.transform.position;

        checkBrakes();

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

        //accelerate forwards if it will make the movement direction closer to the targetdirection
        if (movesCloserToTarget() && !endPositionInRange(20))
        {
            pawnMovement.throttleInput = 1;
        }
        else pawnMovement.throttleInput = 0;


        //brake if ---- this tank is going to overshoot 
        
        if (useBrakes && pawnMovement.seer.overShoots && ((target.transform.position - (pawnMovement.seer.deltaPosition + pawn.transform.position)).magnitude > 20))
        {
            //Debug.Log("Overshooting - applying brakes");
            pawnMovement.brakeInput = 1;
        }
        else if ((target.transform.position - (pawnMovement.seer.deltaPosition + pawn.transform.position)).magnitude < 20)
        {
            //Debug.Log("Not overshooting - disabling brakes and throttle");
            pawnMovement.throttleInput = 0;
            pawnMovement.brakeInput = 0;
        } else
        {
            //Debug.Log("Not overshooting - disabling brakes");
            pawnMovement.brakeInput = 0;
        } 
    }

    //decides whether to start checking to brake or not
    private void checkBrakes()
    {
        if (Vector3.Distance(Vector3.Scale(target.transform.position, new Vector3(1,0,1)), Vector3.Scale(pawn.transform.position, new Vector3(1, 0, 1))) < distanceToStartBraking)
        {
            useBrakes = true;
        } else useBrakes = false;
    }

    #region macros
    
    private bool endPositionInRange(float range)
    {
        return (target.transform.position - (pawnMovement.seer.deltaPosition + pawn.transform.position)).magnitude < range;
    }

    private bool accelerateBool(Vector3 target)
    {
        bool fixVelocity = isAccelerationValid((target - pawn.transform.position).normalized, pawn.transform.forward);
        bool movesCloser = movesCloserToTarget();
        return fixVelocity && movesCloser;
    }

    private bool movesCloserToTarget()
    {
        return movesCloserToTarget(pawn.transform.position, pawn.transform.forward, target.transform.position);
    }
    private bool movesCloserToTarget(Vector3 currentPosition, Vector3 forward, Vector3 targetPosition)
    {
        Vector3 newPosition = currentPosition + forward.normalized * 5;
        float currentDistance = (currentPosition - targetPosition).magnitude;
        float newDistance = (newPosition - targetPosition).magnitude;
        if (newDistance <= currentDistance)
        {
            return true;
        }
        return false;
    }

    private bool isAccelerationValid(Vector3 goalDirection, Vector3 forwardVector)
    {
        Vector3 newVec = pawnMovement.moveDir() + forwardVector;
        if (Mathf.Abs(Vector3.SignedAngle(newVec, goalDirection, Vector3.up)) > Mathf.Abs(Vector3.SignedAngle(pawnMovement.moveDir(), goalDirection, Vector3.up)))
        {
            return true;
        }
        return false;

    }

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
