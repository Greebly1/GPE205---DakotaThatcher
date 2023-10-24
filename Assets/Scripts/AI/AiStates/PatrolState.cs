using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private string stateName = "patrolState";

    private BaseAiController _controller;
    private Transform[] patrolLocations;
    private GameObject AiTargeter;

    private int currentPatrolIndex = 0;
    private bool forwards = true;

    public PatrolState(BaseAiController controller, Transform[] patrolLocations, GameObject aiTargeter)
    {
        _controller = controller;
        this.patrolLocations = patrolLocations;
        AiTargeter = aiTargeter;
    }

    void IState.onBegin()
    {
        currentPatrolIndex = 0;
        forwards = true;
        AiTargeter.transform.position = patrolLocations[currentPatrolIndex].transform.position;
    }

    void IState.onEnd()
    {
        _controller.setDefaultMovementValues();
    }

    string IState.stateName()
    {
        return stateName;
    }

    void IState.tick()
    {
        //Debug.Log("Doing patrol state");
        if (Vector3.Distance(_controller.pawn.transform.position, patrolLocations[currentPatrolIndex].transform.position)
            < 29)
        {
            if (_controller.patrolLooping & currentPatrolIndex == patrolLocations.Length - 1)
            {
                currentPatrolIndex = 0;
            }
            else if (_controller.patrolLooping)
            {
                currentPatrolIndex++;
            }

            if (forwards)
            {
                if (currentPatrolIndex == patrolLocations.Length - 1)
                {
                    forwards = !forwards;
                    currentPatrolIndex--;
                }
                else currentPatrolIndex++;
            }
            else
            {
                if (currentPatrolIndex == 0)
                {
                    forwards = !forwards;
                    currentPatrolIndex++;
                }
                else currentPatrolIndex--;
            }
            AiTargeter.transform.position = patrolLocations[currentPatrolIndex].transform.position;
        }
        else
        {
            _controller.checkBrakes();
            _controller.seek(AiTargeter.transform.position, _controller.patrolStateMoveToPrecision, _controller.patrolStateThrottlePower, 0.5f);
        }
    }
}
