using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour {

    public Pawn pawn;

    public virtual void InitPawn(Pawn possessedPawn)
    {
        pawn = possessedPawn;
        Debug.Log("pawn set");
    }
    
    // Start is called before the first frame update
    public virtual void Start() {
        //Init(pawn);
    }

    // Update is called once per frame
    public virtual void Update() { }

    public virtual void death()
    {
        Debug.Log("Destroying game object");
        Destroy(gameObject);
    }
}
