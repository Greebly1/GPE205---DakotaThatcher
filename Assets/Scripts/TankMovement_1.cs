using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TankMovement : MonoBehaviour
{
    //inspector variables & constants
    public float topSpeed = 80f;
    public float secondsToTopSpeed = 2f;
    public float dampingScale = 1f;
    public float frictionMultiplier = 1.7f;
    public float turnSpeed;
    public float baseDamping = 5f;
    public float angularDamping = 1f;
    [SerializeField] private AnimationCurve turnSpeedCurve;
    public float brakePower;
    private float throttlePower;

    //current values
    private Vector3 velocity;
    private Vector3 acceleration;
    [HideInInspector] public float throttleInput = 0;
    [HideInInspector] public float turnInput = 0;
    [HideInInspector] public float brakeInput = 0;
    private bool isGrounded = true;
    simulationResult seer;

    //cached references
    Rigidbody rb;

    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        throttlePower = (topSpeed / secondsToTopSpeed);
    }

    private void Update()
    {
        seer = performSimulation(velocity, angularDamping, baseDamping, dampingScale, frictionMultiplier, 0.1f, 0f, 100);
        Debug.Log("Tank is headed for " + (transform.position + seer.deltaPosition));
        //Debug.Log("Simulation lasted" + seer.timeSpan);
       // Debug.Log("Speed = " + speed());
        Debug.DrawLine(transform.position, transform.position + velocity, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.forward * 25, Color.blue);
    }

    private void FixedUpdate()
    {
        Vector3 throttleVector = CalculateThrottle(Time.fixedDeltaTime);
        Vector3 dampingVector = CalculateDamping(Time.fixedDeltaTime);
        Vector3 angularDampingVector = CalculateAngularDamping(Time.fixedDeltaTime);
        acceleration = throttleVector + dampingVector + angularDampingVector;

        applyAcceleration();
        transform.position += velocity * Time.fixedDeltaTime;
        applyTurning(Time.fixedDeltaTime);
        applyAcceleration();
    }
    #region acceleration
    private void applyAcceleration() {
        applyAcceleration(0.5f, acceleration, ref velocity, isGrounded);
    }
    public void applyAcceleration(float timePartition, Vector3 accelerationAmount, ref Vector3 tempVelocity, bool onGround)
    {
        if(onGround)
        {
            tempVelocity += accelerationAmount * timePartition;
            tempVelocity = Vector3.ClampMagnitude(tempVelocity, topSpeed);
            if (speed(tempVelocity) < 0.1)
            {
                tempVelocity *= 0;
            }
        }
    }

    private Vector3 CalculateThrottle(float timeStep)
    {
        return CalculateThrottle(timeStep, throttlePower, throttleInput, transform.forward, isBraking());
    }
    public Vector3 CalculateThrottle(float timeStep, float power, float input, Vector3 forward, bool braking)
    {
        if (braking) return Vector3.zero;
        return forward * power * input * timeStep;
    }

    private Vector3 CalculateDamping(float timeStep)
    {
        return CalculateDamping(timeStep, moveDir(), dampingScale, frictionMultiplier, speed(), baseDamping, CalculateBrakePower());
    }
    public Vector3 CalculateDamping(float timeStep, Vector3 direction, float dampingPower, float frictionPower, float currentSpeed, float baseFriction, float brakePower)
    {
        return moveDir(direction)* -1 * dampingPower * linearFormula(frictionPower, currentSpeed, baseFriction) * timeStep * brakePower;
    }

    private Vector3 CalculateAngularDamping(float timeStep) {
        return CalculateAngularDamping(timeStep, moveDir(), transform.forward, angularDamping);
}
    public Vector3 CalculateAngularDamping(float timeStep, Vector3 direction, Vector3 forward, float angularDampingPower)
    {
        return direction * -1 * angularity(direction, forward) * angularDampingPower * timeStep;
    }

    #endregion

    #region turning

    public float CalculateTurnPower()
    {
        return CalculateTurnPower(turnInput, turnSpeed, velocity, topSpeed);
    }

    public float CalculateTurnPower(float turn, float maxTurnSpeed, Vector3 tempVelocity, float maxSpeed)
    {
        return turnAmount(turn) * maxTurnSpeed * turnSpeedCurve.Evaluate(speedPercent(tempVelocity,maxSpeed));
    }

    private void applyTurning(float timeStep)
    {
        applyTurning(Time.fixedDeltaTime, CalculateTurnPower(), turnDir());
    }
    private void applyTurning(float timeStep, float turnPower, int direction)
    {
        transform.Rotate(0f, turnPower * direction * timeStep, 0f, Space.Self);
    }

    #endregion
    //-----Macros-----
    #region velocity
    public Vector3 moveDir()
    {
        return moveDir(velocity);
    }
    public Vector3 moveDir(Vector3 tempVelocity)
    {
        return tempVelocity.normalized;
    }
    public float speed()
    {
        return speed(velocity);
    }
    public float speed(Vector3 tempVelocity)
    {
        return tempVelocity.magnitude;
    }

    public float speedPercent()
    {
        return speedPercent(speed(velocity), topSpeed);
    }
    public float speedPercent(Vector3 tempVelocity, float maxSpeed)
    {
        return speedPercent(speed(tempVelocity), maxSpeed);
    }
    public float speedPercent(float speed, float maxSpeed)
    {
        return speed/maxSpeed;
    }
    public float angularity()
    {
        return angularity(moveDir(), transform.forward);
    }
    public float angularity(Vector3 direction, Vector3 forward)
    {
        return Vector3.Angle(direction, forward) / 90;

    }
    #endregion

    #region turning
    public int turnDir()
    {
        return turnDir(turnInput);
    }
    public int turnDir(float turn)
    {
        if (turn < 0)
        {
            return -1;
        }
        else return 1;
    }
    public float turnAmount()
    {
        return turnAmount(turnInput);
    }
    public float turnAmount(float turn)
    {
        return Mathf.Abs(turn);
    }
    public bool isTurningRight()
    {
        return isTurningRight(turnInput);
    }
    public bool isTurningRight(float turn)
    {
        if (turnDir(turn) > 0)
        {
            return true;
        }
        else return false;
    }
    public bool isTurningLeft()
    {
        return !isTurningRight(turnInput);
    }
    public bool isTurningLeft(float turn)
    {
        return !isTurningRight(turn);
    }

    #endregion

    #region braking
    public bool isBraking()
    {
        if (brakeInput != 0)
        {
            return true;
        } return false;
    }

    public float CalculateBrakePower()
    {
        return CalculateBrakePower(brakeInput, brakePower);
    }
    public float CalculateBrakePower(float brakes, float brakeScalar)
    {
        return 1 + (brakes * brakeScalar);
    }

    #endregion

    public float linearFormula(float slope, float x, float intercept)
    {
        return slope * x + intercept;
    }
    public float friction()
    {
        return linearFormula(frictionMultiplier, speed(), baseDamping);
    }

    public struct simulationResult
    {
        public float timeSpan;
        public Vector3 endVelocity;
        public Vector3 deltaPosition;
        public simulationResult(float time, Vector3 finalVelocity, Vector3 positionChange)
        {
            timeSpan = time;
            endVelocity = finalVelocity;
            deltaPosition = positionChange;
        }
    }

    public simulationResult performSimulation(Vector3 startVelocity, float angularDampingStrength, float baseFriction, 
        float dampingPower, float frictionPower, float timeStep = 1f, float endVelocity = 0.1f, float length = 10f)
    {
        Vector3 tempVelocity = startVelocity;
        Vector3 movement = Vector3.zero;
        Vector3 moveDir = tempVelocity.normalized;
        float duration = 0f;

        while (tempVelocity.magnitude > endVelocity && duration < length)
        {

            Vector3 dampingVector = CalculateDamping(timeStep, moveDir, dampingPower, frictionPower, tempVelocity.magnitude, baseFriction, 1);
            Vector3 angularDampingVector = CalculateAngularDamping(timeStep, moveDir, transform.forward, angularDampingStrength);
            Vector3 accel = dampingVector + angularDampingVector;

            applyAcceleration(0.5f, accel, ref tempVelocity, true);
            movement += tempVelocity * timeStep;
            applyAcceleration(0.5f, accel, ref tempVelocity, true);

            duration += timeStep;
        }

        return new simulationResult(duration, tempVelocity, movement);
    }
}
