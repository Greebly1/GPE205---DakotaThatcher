using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPawn : Pawn
{

    #region variables
    #region Properties and Constants
        public float topSpeed;
        public float stationaryTurnSpeed;
        public float secondsToTopSpeed;
            private float acceleration;
        public float secondsUntilStoppedByDamping;
            private float dampingAmount;
        public float dampingScalarForBraking;
            private float brakePower;
        [SerializeField] private AnimationCurve turnSpeedCurve;
        [SerializeField] private AnimationCurve dampingCurve;
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

    #region start & update
    //Initialize constants
    public void Awake()
    {
        acceleration = topSpeed / secondsToTopSpeed;

    }
    // Start is called before the first frame update
    public override void Start() {
        base.Start();
        rb = GetComponent<Rigidbody>();
        isBraking = false;
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();

        CalculateVelocity();

        moveTank();
    }
    #endregion

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
    }

        #region Input
        /// <summary>
        /// Moves tank across the currentVelocity vector
        /// </summary>
        public void moveTank()
        {
            rb.MovePosition(gameObject.transform.position + (Time.deltaTime * currentVelocity));
        }
    
        /// <summary>
        /// 2nd degree observer of the turning input event, takes input value and turns this tank
        /// </summary>
        /// <param name="inputAmount">inputAmount: -1 - 1 float representing left or right turning</param>
        public override void Turn(float inputAmount)
        {
            float turnAmount = (inputAmount * stationaryTurnSpeed) * turnSpeedCurve.Evaluate(percentageOfTopSpeed());
            gameObject.transform.Rotate(0f, turnAmount * Time.deltaTime, 0f, Space.World);
        }

        /// <summary>
        /// Handles playerController input for acceleration but calculating the throttle vector using this pawn's acceleration constant
        /// </summary>
        /// <param name="inputAmount">inputAmount: a 0-1 float representing amount of acceleration input</param>
        public override void setThrottle(float inputAmount)
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
        public override void setBrake(float inputValue)
            {
            if (inputValue > 0f)
            {
                isBraking = true;
            } else
            {
                isBraking = false;
            }
            brakeAmount = inputValue * dampingScalarForBraking;
        }
        #endregion

        #region Apply Variables
        /// <summary>
        /// Calculates  damping value then apply it to  velocity
        /// </summary>
        private void applyDamping()
        {
            //Calculate damping vector using public strength scalar multiplied by public scalar curve based on  movement speed
            damping = brakeVector() * dampingAmount * dampingCurve.Evaluate(percentageOfTopSpeed());
            Vector3 brakeScaledDamping = damping * brakeAmount;
            currentVelocity += damping * Time.deltaTime;
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
    private Vector3 brakeVector()
    {
        Vector3 brakevector = movementDirection() * -1;
        return brakevector;
    }
    private Vector3 movementDirection()
    {
        Vector3 movementDirection = currentVelocity.normalized;
        return movementDirection;
    }
    private float movementSpeed()
    {
        float movementSpeed = currentVelocity.magnitude;
        return movementSpeed;
    }
    private float percentageOfTopSpeed()
    {
        return movementSpeed() / topSpeed;
    }
    #endregion

}
