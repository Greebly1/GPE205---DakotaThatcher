using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : Controller
{
    #region variables

    public enum AIState {Guard, Chase, Flee, Attack};

    public AIState currentState;

    private float lastStateChange;

    public GameObject target;
    #endregion

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
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
}
