using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : Controller
{
    #region variables

    public TankMovement pawnMovement;
    public enum AIState {Guard, Chase, Attack, Return};

    public AIState currentState;

    private float lastStateChange;

    public GameObject target;

    private Vector3 goalPosition;
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

        makeDecision();
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
        //rotate towards targetposition if necessary
        //accelerate forwards or backwards if it will make the movement direction closer to the targetdirection
        //brake if ---- this tank is going to overshoot 

    }
    #region macros

    private bool willOvershoot()
    {

    }

    private  bool isAccelerationValid(Vector3 goalDirection, Vector3 forwardVector)
    {

    }

    private Vector3 targetVector(Vector3 targetPosition)
    {
        return targetPosition - transform.position;
    }

    private Vector3 targetdirection(Vector3 targetPosition)
    {
        return targetVector(targetPosition).normalized;
    }

    private float distanceFromTarget(Vector3 targetPosition)
    {
        return targetVector(targetPosition).magnitude;
    }

    private Vector3 endPosition()
    {

    }
    #endregion
}
