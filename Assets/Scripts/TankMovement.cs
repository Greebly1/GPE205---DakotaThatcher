using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TankMovement : MonoBehaviour
{

    #region variables

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
    private simulationResult simulation;

    #region Properties and Constants
    public float topSpeed;
    public float stationaryTurnSpeed;
    public float secondsToTopSpeed;
    private float acceleration;
    public float dampingTime;
    public float baseDamping;
    public float dampingScalar;
    public float angularDamping;
    public float dampingScalarForBraking;
    [SerializeField] private AnimationCurve turnSpeedCurve;
    //[SerializeField] private AnimationCurve dampingCurve;
    #endregion

    #region Current Frame Information
    private Vector3 damping;
    private Vector3 throttle;
    private Vector3 currentAcceleration;
    private Vector3 currentVelocity;
    private bool isBraking;
    private float brakeAmount;
    #endregion

    Rigidbody rb;
    #endregion

    private void Awake()
    {
        if (dampingTime <= 0)
        {
            dampingTime = 0.01f;
        }
        if (secondsToTopSpeed <= 0)
        {
            secondsToTopSpeed = 0.01f;
        }
        acceleration = (topSpeed / secondsToTopSpeed) + (topSpeed / dampingTime);

        rb = GetComponent<Rigidbody>();

        isBraking = false;
    }
    // Update is called once per frame
    void Update()
    {
        CalculateVelocity();

        moveTank();

        //simulation = SimulateMovement(new Vector3(0f, 100f, 0f), 0, 10);

        Debug.DrawLine(transform.position, transform.position + transform.forward * 25, Color.blue);
        Debug.DrawLine(transform.position, transform.position + getMovementDirection() * 25, Color.green);
        //Debug.DrawLine(transform.position, transform.position + simulation.getChangeInPosition() * 25, Color.red);
    }

    #region Movement

    /// <summary>
    /// Master function
    /// 1. Applies the damping to velocity
    /// 2. Applies the acceleration to velocity
    /// 3. Applies the brakes to velocity
    /// 4. Clamps velocity to range
    /// </summary>
    private void CalculateVelocity()
    {
        applyDamping();

        applyAcceleration();

        currentVelocity = Vector3.ClampMagnitude(currentVelocity, topSpeed);

        if (getMovementSpeed() < 1)
        {
            currentVelocity *= 0;
        }
    }

    #region Input
    /// <summary>
    /// Moves tank across the currentVelocity vector
    /// </summary>
    public void moveTank()
    {
       /* RaycastHit moveTest;
        if (!rb.SweepTest(getMovementDirection(), out moveTest))
        {
            Debug.Log("hitting wall");
        } */
        rb.MovePosition(gameObject.transform.position + (Time.deltaTime * currentVelocity));
    }

    /// <summary>
    /// 2nd degree observer of the turning input event, takes input value and turns this tank
    /// </summary>
    /// <param name="inputAmount">inputAmount: -1 - 1 float representing left or right turning</param>
    public void Turn(float inputAmount)
    {
        float turnAmount = (inputAmount * stationaryTurnSpeed) * turnSpeedCurve.Evaluate(getPercentageOfTopSpeed());
        gameObject.transform.Rotate(0f, turnAmount * Time.deltaTime, 0f, Space.World);
    }

    public void TurnToTarget(float inputAmount, Vector3 target)
    {
        float CurrentAngle = angleToTarget(target);
        if (CurrentAngle > 0)
        {
            Turn(inputAmount);
            //Turn Right
        } else if (CurrentAngle < 0)
        {
            Turn(inputAmount * -1);
            //Turn left
        }
    }

    /// <summary>
    /// Handles playerController input for acceleration but calculating the throttle vector using this pawn's acceleration constant
    /// </summary>
    /// <param name="inputAmount">inputAmount: a 0-1 float representing amount of acceleration input</param>
    public void setThrottle(float inputAmount)
    {
        float ismoving = Mathf.Ceil(Mathf.Abs(inputAmount));
        float direction = Mathf.Ceil(inputAmount) + Mathf.Floor(inputAmount);
        throttle = gameObject.transform.forward * (acceleration * inputAmount);
        if (isBraking)
        {
            throttle = gameObject.transform.forward * 0;
        }
    }

    /// <summary>
    /// Takes player input and sets the braking amount and the braking boolean
    /// </summary>
    /// <param name="inputValue"> float inpit thing </param>
    public void setBrake(float inputValue)
    {
        if (inputValue > 0f)
        {
            isBraking = true;
        }
        else
        {
            isBraking = false;
        }
        brakeAmount = inputValue;
    }
    #endregion

    #region Apply Variables
    /// <summary>
    /// Calculates  damping value then apply it to  velocity
    /// </summary>
    private void applyDamping()
    {
        // >1 scalar
        float brakeDamping = 1 + (brakeAmount * dampingScalarForBraking);

        //The angle difference between the movement direction and the forward vector
        float angularity = Vector3.Angle(getMovementDirection(), gameObject.transform.forward);
        //a 0-1 scalar based on how close the angularity is to 90 (a percent of 90)
        float angularDampingScalar = angularity / 90;
        currentVelocity -= getMovementDirection() * (passiveDamping() * brakeDamping * dampingScalarFormula()) * Time.deltaTime;
        currentVelocity -= getMovementDirection() * angularDamping * angularDampingScalar * Time.deltaTime;
    }

    /// <summary>
    /// Calculate's acceleration and applies it to velocity
    /// </summary>
    private void applyAcceleration()
    {
        //Calculate acceleration
        currentAcceleration = throttle;
        //Apply acceleration to the velocity
        currentVelocity += currentAcceleration * Time.deltaTime;
    }
    /// <summary>
    /// Applies brakes to velocity using a scalar value
    /// </summary>
    #endregion
    #endregion


    #region Macros
    public float passiveDamping()
    {
        float damping = topSpeed / dampingTime;
        return damping;
    }
    public Vector3 getCurrentVelocity()
    {
        return currentVelocity;
    }
    public Vector3 getBrakeVector()
    {
        Vector3 brakevector = getMovementDirection() * -1;
        return brakevector;
    }
    public Vector3 getForwardVector()
    {
        return gameObject.transform.forward;
    }
    public Vector3 getMovementDirection()
    {

        Vector3 movementDirection = currentVelocity.normalized;
        return movementDirection;
    }
    public float getMovementSpeed()
    {
        float movementSpeed = currentVelocity.magnitude;
        return movementSpeed;
    }
    public float getPercentageOfTopSpeed()
    {
        return getMovementSpeed() / topSpeed;
    }

    public float dampingScalarFormula()
    {
        return (dampingScalar * getPercentageOfTopSpeed()) + baseDamping;
    }
    public float angleToTarget(Vector3 targetPosition)
    {
        return angleToTarget(transform.position, targetPosition, transform.forward);
    }
    public float angleToTarget(Vector3 startPosition, Vector3 targetPosition, Vector3 forward)
    {
        Vector3 directionToTarget = (startPosition - targetPosition).normalized;

        Vector3 adjustmentVector = new Vector3(1f, 0f, 1f);

        Vector3 adjustedDirection = Vector3.Scale(directionToTarget, adjustmentVector);
        Vector3 adjustedForward = Vector3.Scale(forward, adjustmentVector) * -1;

        return Vector3.SignedAngle(adjustedForward, adjustedDirection, Vector3.up);
    }

    public simulationResult SimulateMovement(Vector3 targetPosition, float stopSpeed = 0, float stopDistance = 10)
    {
        float secondsSimulated = 0;
        Vector3 changeInPosition = Vector3.zero;
        Vector3 tempVelocity = getCurrentVelocity();
        Vector3 moveDirection = tempVelocity.normalized;
        float passiveDamp = passiveDamping();
        float tempDampScalar = dampingScalar;
        float tempTopSpeed = topSpeed;
        float tempBaseDamping = baseDamping;
        Vector3 angularDampingScalar = moveDirection * angularDamping * ((Vector3.Angle(moveDirection,
            getForwardVector())) / 90);

        //while the speed is greater than the stopping speed, and the distance from target is greater than the end distance
        while ((tempVelocity.magnitude > stopSpeed) && (targetPosition - (transform.position + changeInPosition)).magnitude > stopDistance)
        {
            //calculate change in velocity through damping
            tempVelocity -= moveDirection * passiveDamp * (tempDampScalar * (tempVelocity.magnitude / tempTopSpeed) + tempBaseDamping);

            //calculate change in velocity through angular damping
            tempVelocity -= angularDampingScalar;

            //add velocity to the change in position
            changeInPosition += tempVelocity;

            secondsSimulated++;
        }

        return new simulationResult(changeInPosition, secondsSimulated, tempVelocity, (targetPosition - (transform.position + changeInPosition)).magnitude);
    }

    #endregion
}
