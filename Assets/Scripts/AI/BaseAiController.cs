using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAiController : Controller
{
    public GameObject AiTargeter;
    public float turningErrorMargin;
    public float distanceToStartBraking;
    public float moveToPrecision = 20;
    public float throttlePower = 1;

    public GameObject targetEnemy;

    public float defaultTurnPower = 1;
    public float defaultThrottlePower = 1;
    private bool useBrakes = false;
    public float turnPower = 1;

    public float desiredAttackRange = 80;

    public float hearingRange = 50.0f;
    public float fieldOfView = 50.0f;
    public float sightRange = 200.0f;
    public Transform sensesOrigin;

    public TankMovement pawnMovement;
    public AiSenses senses;

    public Shooter_Cannon tankGun;

    public AiStateMachine stateMachine;

    

    #region Return Variables
    public float returnStateTurnPower = 1;
    public float returnStateThrottlePower = 1;
    public float returnStateMoveToPrecision = 20.0f;
    #endregion

    #region Patrol Variables
    public Transform[] patrolLocations;
    public float patrolStateMoveToPrecision = 15;
    public float patrolStateTurnPower = 1;
    public float patrolStateThrottlePower = 0.5f;
    public bool patrolLooping = false;
    #endregion

    #region Chase Variables
    public float chaseStateThrottlePower = 1;
    public float chaseStateTurnPower = 1;

    #endregion

    public override void Init(Pawn possessedPawn, int id)
    {
        base.Init(possessedPawn, id);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        AiTargeter = Instantiate(AiTargeter);

        stateMachine = new AiStateMachine();

        pawnMovement = pawn.GetComponent<TankMovement>();
        if (sensesOrigin != null) senses = sensesOrigin.gameObject.AddComponent<AiSenses>();

        //targetEnemy = GameManager.Game.player.pawn.gameObject;

        tankGun = pawn.GetComponent<Shooter_Cannon>();

        GameManager.Game.enemyAIs.Add(this);
    }

    public void OnDestroy()
    {
        GameManager.Game.enemyAIs.Remove(this);
        Destroy(AiTargeter );
    }


    public void createTransition(IState to, IState from, Func<bool> Condition)
    {
        stateMachine.AddTransition(from, to, Condition);
    }

    public void createGlobalTransition(IState to, Func<bool> Condition)
    {
        stateMachine.AddAnyTransition(to, Condition);
    }

    #region Actions
    //Move the tank towards a target this frame
    public void seek(Vector3 targetPosition, float range = 20, float throttle = 1, float brakePower = 1)
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
        }
        else
        {
            //Debug.Log("Not overshooting - disabling brakes");
            pawnMovement.brakeInput = 0;
        }
    }

    //decides whether to start checking to brake or not
    public void checkBrakes()
    {
        if (Vector3.Distance(Vector3.Scale(AiTargeter.transform.position, new Vector3(1, 0, 1)), Vector3.Scale(pawn.transform.position, new Vector3(1, 0, 1))) < distanceToStartBraking)
        {
            useBrakes = true;
        }
        else useBrakes = false;
    }

    //turn tank to look in a direction
    public void turnTo(Vector3 targetPosition)
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
    public Vector3 newTurnTarget(float angle, bool turnLeft = false)
    {
        Debug.Log("NewTarget!");
        if (turnLeft)
        {
            Debug.Log("turnleft");
            return Quaternion.Euler(0, angle, 0) * pawn.transform.forward;
        }
        else
        {
            Debug.Log("turnright");
            return Quaternion.Inverse(Quaternion.Euler(0, angle, 0)) * pawn.transform.forward;
        }
    }
    #endregion


    #region Transition Booleans

    public bool seesPlayer()
    {
        foreach (PlayerController enemy in GameManager.Game.players)
        {
            if (senses.canSee(enemy.gameObject))
            {
                targetEnemy = enemy.gameObject;
                return true;
            }
        }
        targetEnemy = null;
        return false;
    }

    public bool isFacing(Vector3 target)
    {
        float CurrentAngle = pawnMovement.angleToTarget(target);
        //if the pawn is not facing the targetposition
        return MathF.Abs(CurrentAngle) < turningErrorMargin;
    }

    public bool inFireRange(Vector3 target)
    {
        if(Vector3.Distance(this.pawn.transform.position, target) <= desiredAttackRange)
        {
            return true;
        }
        return false;
    }

    public bool canAttack()
    {
        if (targetEnemy == null) { return false; }
        bool inRange = inFireRange(targetEnemy.transform.position); //target is within firing range       
        bool canfire = tankGun.canFire(); //the pawn's gun can fire
        bool aiming_at_target = isFacing(targetEnemy.transform.position); //this pawn is aiming at the target

        return inRange && canfire && aiming_at_target;
    }

    public bool isAt(Vector3 position)
    {
        return Vector3.Distance(position, this.pawn.transform.position) < moveToPrecision;
    }

    #endregion

    #region macros
    public bool endPositionInRange(float range)
    {
        return (AiTargeter.transform.position - (pawnMovement.seer.deltaPosition + pawn.transform.position)).magnitude < range;
    }

    public bool accelerateBool(Vector3 target)
    {
        bool fixVelocity = isAccelerationValid((target - pawn.transform.position).normalized, pawn.transform.forward);
        bool movesCloser = movesCloserToTarget();
        return fixVelocity && movesCloser;
    }

    public bool movesCloserToTarget()
    {
        return movesCloserToTarget(pawn.transform.position, pawn.transform.forward, AiTargeter.transform.position);
    }
    public bool movesCloserToTarget(Vector3 currentPosition, Vector3 forward, Vector3 targetPosition)
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
    
    public bool isAccelerationValid(Vector3 goalDirection, Vector3 forwardVector)
    {
        Vector3 newVec = pawnMovement.moveDir() + forwardVector;
        if (Mathf.Abs(Vector3.SignedAngle(newVec, goalDirection, Vector3.up)) > Mathf.Abs(Vector3.SignedAngle(pawnMovement.moveDir(), goalDirection, Vector3.up)))
        {
            return true;
        }
        return false;

    }
    public void setDefaultMovementValues()
    {
        throttlePower = defaultThrottlePower;
        turnPower = defaultTurnPower;
    }
    #endregion
}
