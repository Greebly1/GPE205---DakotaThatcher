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

    public struct simulationResult
    {
        float duration;
        Vector3 changeInPosition;
        Vector3 endVelocity;
        float distance;
          public simulationResult(Vector3 change, float time, Vector3 endVelocity, float distance)
        {
            this.changeInPosition = change;
            this.duration = time;
            this.endVelocity = endVelocity;
            this.distance = distance;
        }
        public float getDuration()
        {
            return duration;
        }
        public Vector3 getChangeInPosition()
        {
            return changeInPosition;
        }
        public Vector3 getEndVelocity()
        {
            return endVelocity;
        }
        public float getDistance()
        {
            return distance;
        }
    }

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
                pawnMovement.Turn(turnPower);
                //Turn Right
            }
            else if (CurrentAngle < 0)
            {
                pawnMovement.Turn(turnPower * -1);
                //Turn left
            }
        }
        pawnMovement.setThrottle(0.4f);
           
        //accelerate forwards if it will make the movement direction closer to the targetdirection
        //brake if ---- this tank is going to overshoot 

    }
    
    #region macros

    private bool willOvershoot(simulationResult movementSim, Vector3 targetPosition, float time = 0)
    {
        //if the current distance from the target is smaller than the distance from the target to where the tank will be when it stops
        if (distanceFromTarget(targetPosition) < (targetPosition - (pawn.transform.position + movementSim.getChangeInPosition())).magnitude)
        {
            return true;
        }
        return false;
    }

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

    /// <summary>
    /// Simulate the movement (assuming 0 acceleration) until the velocity is at specified speed and cache the 
    /// resulting change in position as well with how long it took to reach
    /// </summary>
    /// <returns>simulation result containing the time until this object stops moving and how far it is moving</returns>
    private simulationResult SimulateMovement(Vector3 targetPosition, float stopSpeed = 0, float stopDistance = 10)
    {
        float secondsSimulated = 0;
        Vector3 changeInPosition = Vector3.zero;
        Vector3 tempVelocity = pawnMovement.getCurrentVelocity();
        Vector3 moveDirection = tempVelocity.normalized;
        float passiveDamp = pawnMovement.passiveDamping();
        float tempDampScalar = pawnMovement.dampingScalar;
        float tempTopSpeed = pawnMovement.topSpeed;
        float tempBaseDamping = pawnMovement.baseDamping;
        Vector3 angularDampingScalar = moveDirection * pawnMovement.angularDamping * ((Vector3.Angle(moveDirection,
            pawnMovement.getForwardVector()))/90);

        //while the speed is greater than the stopping speed, and the distance from target is greater than the end distance
        while ((tempVelocity.magnitude > stopSpeed) && (targetPosition - (pawnMovement.transform.position+changeInPosition)).magnitude > stopDistance){
            //calculate change in velocity through damping
            tempVelocity -= moveDirection * passiveDamp * (tempDampScalar * (tempVelocity.magnitude / tempTopSpeed) + tempBaseDamping);

            //calculate change in velocity through angular damping
            tempVelocity -= angularDampingScalar;

            //add velocity to the change in position
            changeInPosition += tempVelocity;

            secondsSimulated++;
        }

        return new simulationResult(changeInPosition, secondsSimulated, tempVelocity, (targetPosition - (pawnMovement.transform.position + changeInPosition)).magnitude);
    }
    #endregion
}
