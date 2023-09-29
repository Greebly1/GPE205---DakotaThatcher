using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPawn : Pawn
{

    #region variables
    public float acceleration;
    public float topSpeed;
    public float dampingAmount;
    public float stationaryTurnSpeed;
    public float brakePower;
    [SerializeField] private AnimationCurve turnSpeedCurve;
    [SerializeField] private AnimationCurve accelerationCurve;
    [SerializeField] private AnimationCurve dampingCurve;
    private Vector3 damping;
    private Vector3 throttle;
    private Vector3 currentAcceleration;
    private Vector3 currentVelocity;
    private bool isBraking;
    private float brakeAmount;
    Rigidbody rb;
    #endregion

    #region start & update
    // Start is called before the first frame update
    public override void Start() {
        base.Start();
        rb = GetComponent<Rigidbody>();
        isBraking = false;
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();
        //Debug.Log("current speed = " + currentVelocity + ", current acceleration = " + currentAcceleration);
        CalculateMovement();
        moveTank();
    }
    #endregion

    #region movement
    //Calculate the throttle
    public override void setThrottle(float amount) {
        float ismoving = Mathf.Ceil(Mathf.Abs(amount));
        float direction = Mathf.Ceil(amount) + Mathf.Floor(amount);
        throttle = gameObject.transform.forward * (acceleration * amount);
        if (isBraking)
        {
            throttle = gameObject.transform.forward * 0;
        }
    }

    private void CalculateMovement()
    {
        Vector3 brakevector = (currentVelocity * -1).normalized;
        damping = brakevector * dampingAmount * dampingCurve.Evaluate(currentVelocity.magnitude);
        currentVelocity += damping * Time.deltaTime;

        currentAcceleration = throttle;

        currentVelocity += currentAcceleration * Time.deltaTime;

        currentVelocity *= Mathf.Abs(1-(brakeAmount * (brakePower/1000)));

        // * accelerationCurve.Evaluate(currentVelocity.magnitude)
        //* dampingCurve.Evaluate(currentVelocity.magnitude));


        if (currentVelocity.magnitude > topSpeed)
        {
            currentVelocity = currentVelocity.normalized * topSpeed;
        }
    }

    public void moveTank()
    {
        rb.MovePosition(gameObject.transform.position + (Time.deltaTime * currentVelocity));
    }

    public override void Turn(float amount)
    {
        float turnAmount = (amount * stationaryTurnSpeed) * turnSpeedCurve.Evaluate(currentVelocity.magnitude/topSpeed);
        gameObject.transform.Rotate(0f, turnAmount * Time.deltaTime, 0f, Space.World);
    }

    public override void setBrake(float inputValue)
    {
        if (inputValue > 0f)
        {
            isBraking = true;
        } else
        {
            isBraking = false;
        }
        brakeAmount = inputValue;
        Debug.Log(brakeAmount);
    }
    #endregion
}
