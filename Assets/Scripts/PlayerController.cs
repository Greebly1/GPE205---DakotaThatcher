using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller {

    public KeyCode moveForwardKey;
    public KeyCode moveBackwardKey;
    public KeyCode turnLeftKey;
    public KeyCode turnRightKey;
    // Start is called before the first frame update
    public override void Start() {
        base.Start();   
    }

    // Update is called once per frame
    void Update() {
        base.Update();
        HandleInput();
    }

    /// <summary>
    /// Assigns functions to keycodes
    /// </summary>
    void HandleInput() {
        if (Input.GetKeyDown(moveForwardKey))
        {
            pawn.MoveForward();
        }
        if (Input.GetKeyDown(moveBackwardKey))
        {
            pawn.MoveBackward();
        }
        if (Input.GetKeyDown(turnLeftKey))
        {
            pawn.TurnLeft();
        }
        if (Input.GetKeyDown(turnRightKey))
        {
            pawn.TurnRight();
        }
    }

   
}
