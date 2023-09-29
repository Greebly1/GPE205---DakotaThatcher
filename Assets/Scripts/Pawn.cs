using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour {

    // Start is called before the first frame update
    public virtual void Start() {
        
    }

    // Update is called once per frame
    public virtual void Update() {

    }

    public abstract void setThrottle(float amouunt);
    public abstract void Turn(float amount);
    public abstract void setBrake(float inputValue);
}
