using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour {

    public Pawn pawn;
    
    public virtual void Init(Pawn possessedPawn) {
        pawn = possessedPawn;
        pawn.pawnDestroyed += death;
        Debug.Log("Controller initialized");
    }
    
    // Start is called before the first frame update
    public virtual void Start() {
        //Init(pawn);
    }

    // Update is called once per frame
    public virtual void Update() { }

    public void death()
    {
        Debug.Log("Destroying game object");
        Destroy(gameObject);
    }
}
