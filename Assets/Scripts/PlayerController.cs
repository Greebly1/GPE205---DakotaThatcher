using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public enum playerState { alive, dead}


[System.Serializable]
public class PlayerController : Controller {

    public int playerID { get; private set; }

    private playerState state;
    private PlayerInput playerInput;
    private InputAction input_movement;
    private InputAction input_brake;
    private InputAction input_fire1;
    private TankMovement pawnMovement;
    // Start is called before the first frame update

    #region start/update
    public override void Init(Pawn possessedPawn, int ID) {
        base.Init(possessedPawn, ID);
        state = playerState.alive;
        pawnMovement = pawn.GetComponent<TankMovement>();
        playerInput = new PlayerInput();
        input_movement = playerInput.Player.Movement;
        input_brake = playerInput.Player.Brake;
        input_fire1 = playerInput.Player.Fire1;
        playerID = ID;
        enableInput();
    }
    
    public override void Start() {
        base.Start();   

    }

    // Update is called once per frame
    public override void Update() {
        switch (state)
        {
            case playerState.alive:
                base.Update();
                pawnMovement.brakeInput = (input_brake.ReadValue<float>());
                pawnMovement.throttleInput = input_movement.ReadValue<Vector2>().y;
                pawnMovement.turnInput = (input_movement.ReadValue<Vector2>().x);
                break;
            case playerState.dead:
                base.Update();
                break;
        }
       
    }

    private void mouse1(InputAction.CallbackContext context)
    {
        Shooter_Cannon cannon = pawn.GetComponent<Shooter_Cannon>();
        cannon.tryShoot();
    }

    private void brake(InputAction.CallbackContext context)
    {
       // pawnMovement.setBrake(input_brake.ReadValue<float>());
    }

    private void movement(InputAction.CallbackContext context)
    {
        //pawnMovement.setThrottle(input_movement.ReadValue<Vector2>().y);
        //pawnMovement.Turn(input_movement.ReadValue<Vector2>().x);
    }

    #endregion

    #region enable/disable input
    /// <summary>
    /// Enable all input and bind functions to input events
    /// </summary>
    public void enableInput() {
        input_movement.Enable();

        input_brake.Enable();

        input_fire1.Enable();
        input_fire1.performed += mouse1;
    }


    /// <summary>
    /// Disable all input and unbind functions to input events
    /// </summary>
    public void disableInput()
    {
        input_movement.Disable();

        input_brake.Disable();

        input_fire1.Disable();
        input_fire1.performed -= mouse1;
    }

    public void OnDestroy()
    {
        if (GameManager.Game != null)
        {
            GameManager.Game.players.Remove(this);
        }
    }

    #endregion

    public override void death()
    {
        state = playerState.dead;
        disableInput();
    
    }
}
