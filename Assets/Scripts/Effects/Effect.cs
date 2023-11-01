using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Effect
{
    protected bool useUndo;

    protected float effectLength;

    protected float effectTime;

    protected string effectName;

    protected GameObject _receiver;

    //The time when this effect should end
    public float endTime { get { return effectTime + effectLength; } }

    //Whether this effect is undoable or not
    public bool UseUndo { get { return useUndo; } }

    //The global name of this effect
    public string EffectName { get { return effectName; } }


    /// <summary>
    /// Apply this effect, then set effectTime to time.current time
    /// </summary>
    public virtual void apply() {
    }

    /// <summary>
    /// Undo this effect
    /// </summary>
    public virtual void undo() {
    }


    //true if the receiver has all the necessary components for this effect to apply itself
    public virtual bool canApply() { return false; }
}
