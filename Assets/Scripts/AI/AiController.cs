using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class AiController : Controller
{
    #region variables

    public enum AIState { Idle, Guard, Return, Patrol, Chase, Attack };
    public AIState currentState;

    public GameObject AiTargeter;
    public float turningErrorMargin;
    public float distanceToStartBraking;
    public float moveToPrecision = 20;
    public float throttlePower = 1;
    public GameObject targetEnemy;

    public float defaultTurnPower = 1;
    public float defaultThrottlePower = 1;
    private float lastStateChange;
    private bool useBrakes = false;
    private float turnPower = 1;

    public float desiredAttackRange = 80;


    public float hearingRange = 50.0f;
    public float fieldOfView = 50.0f;
    public float sightRange = 200.0f;
    public Transform sensesOrigin;

    public TankMovement pawnMovement;
    private AiSenses senses;

        #region Guard Variables
    public Transform guardPost;
    private bool lastTurnedRight = true;
    public float guardStateTurnPower = 0.5f;
    private bool startingGuardState = false;
    #endregion
    #region Return Variables
    public float returnStateTurnPower = 1;
    public float returnStateThrottlePower = 1;
    public float returnStateMoveToPrecision = 20.0f;
    #endregion

    #region Patrol Variables
    public Transform[] patrolLocations;
    private int currentPatrolIndex = 0;
    private bool forwards = true;
    public float patrolStateMoveToPrecision = 15;
    public float patrolStateTurnPower = 1;
    public float patrolStateThrottlePower = 0.5f;
    public bool patrolLooping = false;
    #endregion
    #region Chase Variables
    public float chaseStateThrottlePower = 1;
    public float chaseStateTurnPower = 1;

    #endregion


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
        
        pawnMovement = pawn.GetComponent<TankMovement>();
        if (sensesOrigin != null ) senses = sensesOrigin.gameObject.AddComponent<AiSenses>();
        
        targetEnemy = GameManager.Game.player.pawn.gameObject;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        makeDecision();

        pawnMovement.targetPosition = AiTargeter.transform.position;

        senses.canSee(targetEnemy);
    }

    protected virtual void makeDecision()
    {
        switch (currentState)
        {
            case AIState.Idle:
                //Debug.Log("Idle"); //do nothing
                doIdleState();
                break;
            case AIState.Guard:
                //Debug.Log("Guard"); //Guard area
                doGuardState();
                break;
            case AIState.Return:
                //Debug.Log("Return"); //move back to guard post
                doReturnState();
                break;
            case AIState.Patrol:
                //Debug.Log("Patrol"); //Move around a designated area
                doPatrolState();
                break;
            case AIState.Chase:
                //Debug.Log("Chase"); //Move to position to fire weapon
                doChaseState();
                break;
            case AIState.Attack:
                //Debug.Log("Attack"); //fire weapon
                doAttackState();
                break;
        }
    }

    public virtual void changeState(AIState newstate)
    {
        if (currentState == newstate) return;
        switch (currentState)
        {
            case AIState.Idle:
                //Debug.Log("Idle"); //do nothing
                onBeginIdle();
                break;
            case AIState.Guard:
                //Debug.Log("Guard"); //Guard area
                onBeginGuard();
                break;
            case AIState.Return:
                //Debug.Log("Return"); //move back to guard post
                onBeginReturn();
                break;
            case AIState.Patrol:
                //Debug.Log("Patrol"); //Move around a designated area
                onBeginPatrol();
                break;
            case AIState.Chase:
                //Debug.Log("Chase"); //Move to position to fire weapon
                onBeginChase();
                break;
            case AIState.Attack:
                //Debug.Log("Attack"); //fire weapon
                onBeginAttack();
                break;
        }

        currentState = newstate;
        lastStateChange = Time.time;

        switch (currentState)
        {
            case AIState.Idle:
                //Debug.Log("Idle"); //do nothing
                onLeaveIdle();
                break;
            case AIState.Guard:
                //Debug.Log("Guard"); //Guard area
                onLeaveGuard();
                break;
            case AIState.Return:
                //Debug.Log("Return"); //move back to guard post
                onLeaveReturn();
                break;
            case AIState.Patrol:
                //Debug.Log("Patrol"); //Move around a designated area
                onLeavePatrol();
                break;
            case AIState.Chase:
                //Debug.Log("Chase"); //Move to position to fire weapon
                onLeaveChase();
                break;
            case AIState.Attack:
                //Debug.Log("Attack"); //fire weapon
                onLeaveAttack();
                break;
        }
    }

    #region State Actions
    private void doIdleState()
    {
        //Debug.Log("Doing idle state");
    }
    private void doGuardState()
    {
        float turnAmount = 170.0f;
        if (startingGuardState)
        {
            turnAmount /= 2;
        }
        //Debug.Log("Doing guard state");
        if (isFacingTarget(AiTargeter.transform.position))
        {
            AiTargeter.transform.position = pawn.transform.position + newTurnTarget(turnAmount, lastTurnedRight);
            lastTurnedRight = !lastTurnedRight;
        }
        turnTo(AiTargeter.transform.position);
    }
    private void doReturnState()
    {
        //Debug.Log("Doing return state");
        AiTargeter.transform.position = guardPost.position;
        seek(AiTargeter.transform.position, moveToPrecision);
    }
    private void doPatrolState()
    {
        
        //Debug.Log("Doing patrol state");
        if (Vector3.Distance(pawn.transform.position, patrolLocations[currentPatrolIndex].transform.position)
            < 29) {
            if (patrolLooping & currentPatrolIndex == patrolLocations.Length-1) {
                currentPatrolIndex = 0;
            } else if (patrolLooping)
            {
                currentPatrolIndex++;
            }

            if (forwards)
            {
                if (currentPatrolIndex == patrolLocations.Length - 1)
                {
                    forwards = !forwards;
                    currentPatrolIndex--;
                } else  currentPatrolIndex++;    
            } else
            {
                if (currentPatrolIndex == 0)
                {
                    forwards = !forwards;
                    currentPatrolIndex++;
                }
                else currentPatrolIndex--;
            }
            AiTargeter.transform.position = patrolLocations[currentPatrolIndex].transform.position;
        } else {
            checkBrakes();
            seek(AiTargeter.transform.position, patrolStateMoveToPrecision, patrolStateThrottlePower, 0.5f);
        }
    }
    private void doChaseState()
    {
        //Debug.Log("Doing chase state");
        AiTargeter.transform.position = targetEnemy.transform.position;
        seek(AiTargeter.transform.position, desiredAttackRange, 0.8f, 1);
    }
    private void doAttackState()
    {
        //Debug.Log("Doing attack state");
        pawn.GetComponent<Shooter_Cannon>().tryShoot();
    }
    #endregion

    #region State Initializers
    protected virtual void onBeginIdle()
    {
        //Debug.Log("Do nothing");
    }

    //Set guard state movement variables & turn 90 degrees to the right
    protected virtual void onBeginGuard()
    {
        startingGuardState = true;
        turnPower = guardStateTurnPower;
    }
    protected virtual void onBeginReturn()
    {
        turnPower = returnStateTurnPower;
        throttlePower = returnStateThrottlePower;
        moveToPrecision = returnStateMoveToPrecision;
    }
    protected virtual void onBeginPatrol()
    {
        currentPatrolIndex = 0;
    }
    protected virtual void onBeginChase()
    {
        throttlePower = chaseStateThrottlePower;
        turnPower = chaseStateTurnPower;
    }
    protected virtual void onBeginAttack()
    {
        //Debug.Log("Do nothing");
    }
    #endregion

    #region State Endings
    protected virtual void onLeaveIdle()
    {
        setDefaultMovementValues();
    }
    protected virtual void onLeaveGuard()
    {
        setDefaultMovementValues();
    }
    protected virtual void onLeaveReturn()
    {
        setDefaultMovementValues();
    }
    protected virtual void onLeavePatrol()
    {
        setDefaultMovementValues();
    }
    protected virtual void onLeaveChase()
    {
        setDefaultMovementValues();
    }
    protected virtual void onLeaveAttack()
    {
        setDefaultMovementValues();
    }
    #endregion

    #region Actions
    //Move the tank towards a target this frame
    private void seek(Vector3 targetPosition, float range = 20, float throttle = 1, float brakePower = 1)
    {
        turnTo(targetPosition);

        //accelerate forwards if it will make the movement direction closer to the targetdirection
        if (movesCloserToTarget() && !endPositionInRange(range))
        {
            pawnMovement.throttleInput = throttle;
        }
        else pawnMovement.throttleInput = 0;


        //brake if ---- this tank is going to overshoot 
        
        if (useBrakes && pawnMovement.seer.overShoots && ((AiTargeter.transform.position - (pawnMovement.seer.deltaPosition + pawn.transform.position)).magnitude > 20))
        {
            //Debug.Log("Overshooting - applying brakes");
            pawnMovement.brakeInput = brakePower;
        }
        else if ((AiTargeter.transform.position - (pawnMovement.seer.deltaPosition + pawn.transform.position)).magnitude < 20)
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
        if (Vector3.Distance(Vector3.Scale(AiTargeter.transform.position, new Vector3(1,0,1)), Vector3.Scale(pawn.transform.position, new Vector3(1, 0, 1))) < distanceToStartBraking)
        {
            useBrakes = true;
        } else useBrakes = false;
    }

    //turn tank to look in a direction
    private void turnTo(Vector3 targetPosition)
    {
        float CurrentAngle = pawnMovement.angleToTarget(targetPosition);
        //if the pawn is not facing the targetposition
        if (MathF.Abs(CurrentAngle) > turningErrorMargin)
        {
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
        }
        else pawnMovement.turnInput = 0;
    }

    //generate a new targetPosition for turning to based on an angle provided
    private Vector3 newTurnTarget(float angle, bool turnLeft = false)
    {
        Debug.Log("NewTarget!");
        if (turnLeft) {
            Debug.Log("turnleft");
            return Quaternion.Euler(0, angle, 0) * pawn.transform.forward;
        } else {
            Debug.Log("turnright");
            return Quaternion.Inverse(Quaternion.Euler(0, angle, 0)) * pawn.transform.forward;
        }
    }
    #endregion

    #region macros
    private bool endPositionInRange(float range)
    {
        return (AiTargeter.transform.position - (pawnMovement.seer.deltaPosition + pawn.transform.position)).magnitude < range;
    }

    private bool accelerateBool(Vector3 target)
    {
        bool fixVelocity = isAccelerationValid((target - pawn.transform.position).normalized, pawn.transform.forward);
        bool movesCloser = movesCloserToTarget();
        return fixVelocity && movesCloser;
    }

    private bool movesCloserToTarget()
    {
        return movesCloserToTarget(pawn.transform.position, pawn.transform.forward, AiTargeter.transform.position);
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
    private bool isFacingTarget(Vector3 target)
    {
        float CurrentAngle = pawnMovement.angleToTarget(target);
        //if the pawn is not facing the targetposition
        return MathF.Abs(CurrentAngle) < turningErrorMargin;
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
    private void setDefaultMovementValues()
    {
        throttlePower = defaultThrottlePower;
        turnPower = defaultTurnPower;
    }
    #endregion
}
