using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pawn : MonoBehaviour {

    #region variables


    Rigidbody rb;

    TankMovement moveComponent;

    Health healthComponent;
    #endregion

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        moveComponent = GetComponent<TankMovement>();
        if (moveComponent == null)
        {
            Debug.Log("pawn does not have a move component");
        }

        Health health = GetComponent<Health>();
        if (healthComponent == null)
        {
            Debug.Log("pawn does not have a Health component");
        }
    }

    // Start is called before the first frame update
    public virtual void Start() {
        
    }

    // Update is called once per frame
    public virtual void Update() {

    }

    public TankMovement GetMovement()
    {
        return moveComponent;
    }
}
