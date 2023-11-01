using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class EffectReceiver : MonoBehaviour
{
    private List<Effect> _currentEffects = new List<Effect>(); //effects that have a timer
    private void Update()
    {
        if (_currentEffects.Count > 0)
        {
            checkAllEffectTimers();
        }
    }

    //iterates through all currentEffects, undoing them if their time has run out
    private void checkAllEffectTimers()
    {
        int currentEffectIndex = 0;
        foreach (Effect undoEffect in _currentEffects)
        {
            if (Time.time >= undoEffect.endTime)
            {
                endEffect(currentEffectIndex);
                break;
            }
            currentEffectIndex++;
        }
    }


    //Undo an effect and remove it from the _currentEffects list
    public void endEffect(int index)
    {
        _currentEffects[index].undo();

        _currentEffects.RemoveAt(index);
    }

    //Apply an effect, then add it to the _currentEffects list if it uses a timer
    public void addEffect(Effect newEffect)
    {
        if (newEffect.UseUndo)
        {
            //end the duplicate effect if this object already has that effect
            if (isUsingEffect(newEffect.EffectName, out int duplicateIndex)) endEffect(duplicateIndex);
           
            newEffect.apply();
            _currentEffects.Add(newEffect);
        }  else
        {
            newEffect.apply();
        }
    }


    public void endAllEffects()
    {
        int currentIndex = 0;
        foreach (Effect effect in _currentEffects)
        {
            endEffect(currentIndex);
            currentIndex++;
        }
    }

    public bool isUsingEffect(string effectName, out int index)
    {
        int effectIndex = 0;
        foreach (Effect undoEffect in _currentEffects)
        {
            if (undoEffect.EffectName == effectName)
            {
                index = effectIndex;
                return true;
            }
            effectIndex++;
        }
        index = 0;
        return false;
    }

    public List<Effect> getEffectsList()
    {
        return _currentEffects;
    }
}
