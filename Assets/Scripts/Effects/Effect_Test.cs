using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Test : Effect
{

    public Effect_Test(GameObject receiver, float length = 5.0f, bool UseUndo = false)
    {
        effectName = "Test";
        _receiver = receiver;
        effectLength = length;
        useUndo = UseUndo;
        effectTime = Time.time;
    }

    public override void apply()
    {
        Debug.Log("Effect applied");
    }

    public override void undo()
    {
        Debug.Log("Effect undone");
    }

    public override bool canApply()
    {
        Debug.Log("Can effect be applied?");
        if (_receiver.GetComponent<Health>() != null)
        {
            Debug.Log("effect can be applied because receiver has a health component");
            return true;
        }
        Debug.Log("Effect cannot be applied");
        return false;
    }
}
