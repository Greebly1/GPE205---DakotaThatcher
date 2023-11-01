using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Heal : Effect
{
    private float _healAmount;
    private Health _receiverHealth;

    public Effect_Heal(GameObject receiver, float healAmount = 50)
    {
        effectName = "Heal";
        _receiver = receiver;
        _receiverHealth = receiver.GetComponent<Health>();
        effectLength = 0f ;
        useUndo = false;
        effectTime = Time.time;
        _healAmount = healAmount;
    }

    public override void apply()
    {
        Debug.Log("HealApplied + " + _healAmount + " hp!");
        _receiverHealth.heal(_healAmount);
    }

    public override void undo()
    {
        Debug.Log("Heal should not be undone");
    }

    public override bool canApply()
    {

        Debug.Log("Can effect be applied?");
        if (_receiverHealth != null)
        {
            Debug.Log("Receiver has a health component");
            if (_receiverHealth.getHealth() != _receiverHealth.maxHealth)
            {
                Debug.Log("effect can be applied because receiver health is not max");
                return true;
            }
            Debug.Log("Receiver is at max health");
        }
        Debug.Log("Effect cannot be applied");
        return false;
    }
}