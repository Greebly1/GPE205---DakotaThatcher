using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Speed : Effect
{
    private TankMovement _receiverMovement;

    private TankMovement.movementValues defaultMoveVars;

    private TankMovement.movementValues newMoveVars;


    public Effect_Speed(GameObject receiver, TankMovement.movementValues newMovementValues, float length = 5.0f, bool UseUndo = true)
    {
        effectName = "Speed";
        _receiver = receiver;
        effectLength = length;
        useUndo = UseUndo;
        effectTime = Time.time;
        _receiverMovement = receiver.GetComponent<TankMovement>();

        newMoveVars = newMovementValues;

        int tempIndex;
        EffectReceiver pickupReceiver = receiver.GetComponent<EffectReceiver>();
        if (pickupReceiver.isUsingEffect(effectName, out tempIndex)) // if the receiver already has a speed effect
        {
            //get the original default movement from the current speed effect
            TankMovement.movementValues defaultMovement = ((Effect_Speed)pickupReceiver.getEffectsList()[tempIndex]).getDefaultMovementValues();
            cacheDefaultMovement(defaultMovement); //cache those movement variables
        } 
        else cacheDefaultMovement(_receiverMovement); //if receiver doesn't have a speed effect cache the receivermovement's current movement values
    }

    public override void apply()
    {
        Debug.Log("Speed effect applied!");
        _receiverMovement.moveVars = newMoveVars;
        _receiverMovement.CalculateThrottlePower();
    }

    public override void undo()
    {
        Debug.Log("Speed effect undone");
        _receiverMovement.moveVars = defaultMoveVars;
        _receiverMovement.CalculateThrottlePower();
    }

    public override bool canApply()
    {
        Debug.Log("Can effect be applied?");
        if (_receiverMovement != null)
        {
            Debug.Log("effect can be applied because receiver has a movement component");
            return true;
        }
        Debug.Log("Effect cannot be applied");
        return false;
    }

    //Save a movement component's movement variables
    private void cacheDefaultMovement(TankMovement movement)
    {
        cacheDefaultMovement(movement.moveVars);
    }

    private void cacheDefaultMovement(TankMovement.movementValues movementVariables)
    {
        defaultMoveVars = movementVariables;
    }

    public TankMovement.movementValues getDefaultMovementValues ()
    {
        return defaultMoveVars;
    }
}
