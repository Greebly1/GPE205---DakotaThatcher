using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerController : Controller {

    private PlayerInput playerInput;
    private InputAction input_movement;
    private InputAction input_brake;
    private InputAction input_fire1;
    private TankMovement pawnMovement;
    // Start is called before the first frame update

    #region start/update
    public override void Init(Pawn possessedPawn) {
        base.Init(possessedPawn);
        pawnMovement = pawn.GetComponent<TankMovement>();
        playerInput = new PlayerInput();
        input_movement = playerInput.Player.Movement;
        input_brake = playerInput.Player.Brake;
        input_fire1 = playerInput.Player.Fire1;
        OnEnable();
    }
    
    public override void Start() {
        base.Start();   

        if (GameManager.Game != null)
        {
            GameManager.Game.players.Add(this);
        }
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();
        pawnMovement.brakeInput = (input_brake.ReadValue<float>());

        pawnMovement.throttleInput = input_movement.ReadValue<Vector2>().y;
        pawnMovement.turnInput = (input_movement.ReadValue<Vector2>().x);
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
    public void OnEnable() {
        input_movement.Enable();

        input_brake.Enable();

        input_fire1.Enable();
        input_fire1.performed += mouse1;
    }


    /// <summary>
    /// Disable all input and unbind functions to input events
    /// </summary>
    public void OnDisable()
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
}
