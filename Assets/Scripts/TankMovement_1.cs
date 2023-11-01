using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TankMovement : MonoBehaviour
{

    [System.Serializable]
    public struct movementValues
    {
        public float topSpeed;
        public float secondsToTopSpeed;
        public float dampingScale;
        public float frictionMultiplier;
        public float turnSpeed;
        public float baseDamping;
        public float angularDamping;

        public movementValues(float TopSpeed = 80, float SecondsToTopSpeed = 2, float DampingScale = 1,
            float FrictionMultiplier = 1.7f, float TurnSpeed = 120, float BaseDamping = 5, float AngularDamping = 1)
        {
            topSpeed = TopSpeed;
            secondsToTopSpeed = SecondsToTopSpeed;
            dampingScale = DampingScale;
            frictionMultiplier = FrictionMultiplier;
            turnSpeed = TurnSpeed;
            baseDamping = BaseDamping;
            angularDamping = AngularDamping;
        }
    }

    public movementValues moveVars;

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
    public simulationResult seer;


    //TEMPORARY WORKAROUND VARS
    //Used for movement simulation pathfinding
    [HideInInspector] public Vector3 targetPosition = Vector3.zero;

    //cached references
    Rigidbody rb;

    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        CalculateThrottlePower();
    }

    public void CalculateThrottlePower() {throttlePower = (moveVars.topSpeed / moveVars.secondsToTopSpeed); }

    private void Update()
    {
        seer = performSimulation(0.06f, 0f, targetPosition);

        //Debug.Log("TargetPosition = " + (seer.deltaPosition + transform.position));
        //Debug.Log("Damping = " + CalculateDamping(Time.deltaTime).magnitude);
        //Debug.Log("AngularDamping = " + CalculateAngularDamping(Time.deltaTime).magnitude);
        //Debug.Log("Angularity Left:" +angularity(Vector3.forward, Vector3.left));
        //Debug.Log("Angularity Back:" + angularity(Vector3.forward, Vector3.back));

        Debug.DrawLine(transform.position, transform.position + seer.deltaPosition, Color.green);
        //Debug.DrawLine(transform.position, transform.position + moveDir()*25, Color.red);
        //Debug.DrawLine(transform.position, transform.position + transform.forward * 25, Color.blue);
    }

    private void FixedUpdate()
    {
        Vector3 throttleVector = CalculateThrottle(Time.fixedDeltaTime);
        Vector3 dampingVector = CalculateDamping(Time.fixedDeltaTime);
        Vector3 angularDampingVector = CalculateAngularDamping(Time.fixedDeltaTime);
        acceleration = throttleVector + dampingVector + angularDampingVector;
        //Debug.Log("amounts" + angularDampingVector.normalized + dampingVector.normalized);

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
            tempVelocity = Vector3.ClampMagnitude(tempVelocity, moveVars.topSpeed);
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
        return CalculateDamping(timeStep, moveDir(), moveVars.dampingScale, moveVars.frictionMultiplier, speed(), moveVars.baseDamping, CalculateBrakePower());
    }
    public Vector3 CalculateDamping(float timeStep, Vector3 direction, float dampingPower, float frictionPower, float currentSpeed, float baseFriction, float brakePower)
    {
        return moveDir(direction)* -1 * dampingPower * linearFormula(frictionPower, currentSpeed, baseFriction) * timeStep * brakePower;
    }

    private Vector3 CalculateAngularDamping(float timeStep) {
        return CalculateAngularDamping(timeStep, moveDir(), transform.forward, moveVars.angularDamping, speed());
}
    public Vector3 CalculateAngularDamping(float timeStep, Vector3 direction, Vector3 forward, float angularDampingPower, float currentSpeed)
    {
        return moveDir(direction) * -1 * angularity(direction, forward) * linearFormula(1, currentSpeed, 0) * angularDampingPower * timeStep;
    }

    #endregion

    #region turning

    public float CalculateTurnPower()
    {
        return CalculateTurnPower(turnInput, moveVars.turnSpeed, velocity, moveVars.topSpeed);
    }

    public float CalculateTurnPower(float turn, float maxTurnSpeed, Vector3 tempVelocity, float maxSpeed)
    {
        return turnAmount(turn) * maxTurnSpeed * turnSpeedCurve.Evaluate(speedPercent(tempVelocity,maxSpeed));
    }

    private void applyTurning(float timeStep)
    {
        applyTurning(timeStep, CalculateTurnPower(), turnDir());
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
        if (tempVelocity.magnitude < 0.1f) return Vector3.zero;
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
        return speedPercent(speed(velocity), moveVars.topSpeed);
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
        if (Mathf.Abs(Vector3.SignedAngle(forward, direction, Vector3.up)) > 90) return Mathf.Abs(Vector3.SignedAngle(forward*-1, direction, Vector3.up))/90;
        return Mathf.Abs(Vector3.SignedAngle(forward, direction, Vector3.up))/90;

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

    public void TurnToTarget(float inputAmount, Vector3 target)
    {
        float CurrentAngle = angleToTarget(target);
        if (CurrentAngle > 0)
        {
            turnInput = inputAmount;
            //Turn Right
        }
        else if (CurrentAngle < 0)
        {
            turnInput = -inputAmount;
            //Turn left
        }
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
        return linearFormula(moveVars.frictionMultiplier, speed(), moveVars.baseDamping);
    }

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

    public simulationResult performSimulation(float timeStep, float endVelocity, Vector3 target)
    {
        return performSimulation(velocity, transform.forward, transform.position, target, moveVars.angularDamping, moveVars.baseDamping, moveVars.dampingScale, moveVars.frictionMultiplier, timeStep, endVelocity, 8);
    }

    public simulationResult performSimulation(Vector3 startVelocity, Vector3 startForward, Vector3 currentPosition, Vector3 targetPosition, float angularDampingStrength, float baseFriction, 
        float dampingPower, float frictionPower, float timeStep = 1f, float endVelocity = 0.1f, float length = 10f)
    {
        Vector3 tempVelocity = startVelocity;
        Vector3 movement = Vector3.zero;
        Vector3 moveDir = tempVelocity.normalized;
        float duration = 0f;
        bool overShot = false;

        float DistanceFromTarget = (targetPosition - currentPosition).magnitude;

        while (tempVelocity.magnitude > endVelocity && duration < length)
        {
            Vector3 dampingVector = CalculateDamping(timeStep, moveDir, dampingPower, frictionPower, tempVelocity.magnitude, baseFriction, 1);
            Vector3 angularDampingVector = CalculateAngularDamping(timeStep, moveDir, startForward, angularDampingStrength, tempVelocity.magnitude);
            Vector3 accel = dampingVector + angularDampingVector;
            Vector3 targetVelocity;
            

            if ((tempVelocity + accel).magnitude >= tempVelocity.magnitude) break;

            applyAcceleration(0.5f, accel, ref tempVelocity, true);
            movement += tempVelocity * timeStep;
            applyAcceleration(0.5f, accel, ref tempVelocity, true);

            if (DistanceFromTarget > (targetPosition - (currentPosition + movement)).magnitude ) {
                DistanceFromTarget = (targetPosition - (currentPosition + movement)).magnitude;
            } else
            {
                targetVelocity = tempVelocity;
                overShot = true;
            }

            duration += timeStep;
        }

        return new simulationResult(duration, DistanceFromTarget, tempVelocity, movement, overShot);
    }
}
