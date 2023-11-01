using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The effect factory contains the logic for constructing any effect
//also contain its own unity properties editor for adding customizability to the effects it instantiates


[System.Serializable]
public class effectFactory
{
    public enum Effects {test, heal, speed, money};
    public Effects factoryEffect;

    public TankMovement.movementValues newMoveVars;

    public float effectLength;

    public float healAmount;

    public Effect createEffect(GameObject target)
    {
        switch (factoryEffect)
        {
            case Effects.test:
                //Debug.Log("Test effect created");
                return new Effect_Test(target, effectLength, true);

            case Effects.heal:
                //Debug.Log("Heal effect created");
                return new Effect_Heal(target, healAmount);

            case Effects.speed:
                //Debug.Log("Speed effect created");
                return new Effect_Speed(target, newMoveVars, effectLength);
            case Effects.money:
                //Debug.Log("Money effect created");
                return new Effect_Money(target, 5);

        }
        return new Effect_Test(target);
    }
}
