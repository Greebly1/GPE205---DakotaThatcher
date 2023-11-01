using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public effectFactory Factory;

    private void OnTriggerEnter(Collider other)
    {
        EffectReceiver receiver = other.gameObject.GetComponent<EffectReceiver>();
        if (other.gameObject.GetComponent<EffectReceiver>() != null)
        {
            Effect tempEffect = Factory.createEffect(other.gameObject);
            if (tempEffect.canApply()) {
               doPickup(receiver, tempEffect);
            }
        }
    }


    private void doPickup(EffectReceiver target, Effect newEffect)
    {
        //Debug.Log("Apply this effect from the pickup");
        target.addEffect(newEffect);
    }
}
