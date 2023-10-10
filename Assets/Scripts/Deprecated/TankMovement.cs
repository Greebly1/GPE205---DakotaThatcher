using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TankMovementDeprecated : MonoBehaviour
{

    #region variables

    public struct simulationResult
    {
        float duration;
        Vector3 changeInPosition;
        Vector3 endVelocity;
        public simulationResult(Vector3 change, float time, Vector3 endVelocity)
        {
            this.changeInPosition = change;
            this.duration = time;
            this.endVelocity = endVelocity;
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
    public float angularDampingPower;
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
        

        //simulation = SimulateMovement(1);
        Debug.Log(currentVelocity);
        Debug.Log(currentVelocity.magnitude);

        Debug.DrawLine(transform.position, transform.position + currentVelocity, Color.red);
        //Debug.DrawLine(transform.position, transform.position + getMovementDirection() * (passiveDamping() * dampingScalarFormula()), Color.black);
        Debug.DrawLine(transform.position, transform.position + transform.forward * 25, Color.blue);
        Debug.DrawLine(transform.position, transform.position + getMovementDirection() * 25, Color.green);
    }

    private void FixedUpdate()
    {
        CalculateVelocity(Time.fixedDeltaTime);

        moveTank(Time.fixedDeltaTime);

        CalculateVelocity(Time.fixedDeltaTime);
    }

    #region Movement

    /// <summary>
    /// Master function
    /// 1. Applies the damping to velocity
    /// 2. Applies the acceleration to velocity
    /// 3. Applies the brakes to velocity
    /// 4. Clamps velocity to range
    /// </summary>
    private void CalculateVelocity(float timeStep)
    {
        applyDamping(timeStep);

        applyAcceleration(timeStep);

        currentVelocity = Vector3.ClampMagnitude(currentVelocity, topSpeed);
    }

    #region Input
    /// <summary>
    /// Moves tank across the currentVelocity vector
    /// </summary>
    public void moveTank(float timeStep)
    {
       /* RaycastHit moveTest;
        if (!rb.SweepTest(getMovementDirection(), out moveTest))
        {
            Debug.Log("hitting wall");
        } */
        //rb.MovePosition(gameObject.transform.position + (Time.deltaTime * currentVelocity));
        gameObject.transform.position += (currentVelocity * timeStep);
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
    private void applyDamping(float timeStep)
    {
        // >1 scalar
        float brakeDamping = 1 + (brakeAmount * dampingScalarForBraking);

        currentVelocity -= getMovementDirection() * calculateDamping(timeStep, getMovementSpeed()) * brakeDamping * 0.5f;
        currentVelocity -= getMovementDirection() * calculateAngularDamping(timeStep, angularDampingPower) * 0.5f;
    }

    public float calculateDamping(float timeStep, float speed)
    {
        return (passiveDamping() * dampingScalarFormula()) * timeStep;
    }

    public float calculateAngularDamping(float timeStep, float angularDampingScalar)
    {
        return (Vector3.Angle(getMovementDirection(), gameObject.transform.forward) / 90) * angularDampingScalar * timeStep;
    }

    /// <summary>
    /// Calculate's acceleration and applies it to velocity
    /// </summary>
    private void applyAcceleration(float timeStep)
    {
        //Calculate acceleration
        currentAcceleration = throttle;
        //Apply acceleration to the velocity
        currentVelocity += currentAcceleration * timeStep * 0.5f;
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

    public simulationResult SimulateMovement(float length = 1)
    {
        Vector3 tempVelocity = currentVelocity;
        Vector3 positionalChange = new Vector3(0f, 0f, 0f);

        float damp = passiveDamping();
        Vector3 tempDir = getMovementDirection();

        tempVelocity -= tempDir * damp * (dampingScalar * (tempVelocity.magnitude / topSpeed) + baseDamping) * length;

        //tempVelocity -= tempDir * angularDampingPower * calculateAngularDamping() * length;

        if (tempVelocity.magnitude < 1)
        {
            tempVelocity *= 0;
        }

        positionalChange += tempVelocity;

        Debug.Log(positionalChange);

        return new simulationResult(positionalChange, length, tempVelocity);
    }

    #endregion
}
