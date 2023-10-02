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
    // Start is called before the first frame update

    #region start/update
    public override void Init(Pawn possessedPawn) {
        base.Init(possessedPawn);
        playerInput = new PlayerInput();
        input_movement = playerInput.Player.Movement;
        input_brake = playerInput.Player.Brake;
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
        Debug.Log("Movement Values: " + input_movement.ReadValue<Vector2>());
        pawn.GetMovement().setThrottle(input_movement.ReadValue<Vector2>().y);
        pawn.GetMovement().Turn(input_movement.ReadValue<Vector2>().x);
        pawn.GetMovement().setBrake(input_brake.ReadValue<float>());
    }
    #endregion

    #region enable/disable input
    /// <summary>
    /// Enable all input and bind functions to input events
    /// </summary>
    public void OnEnable() {
        input_movement.Enable();

        input_brake.Enable();
    }


    /// <summary>
    /// Disable all input and unbind functions to input events
    /// </summary>
    public void OnDisable()
    {
        input_movement.Disable();

        input_brake.Disable();
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
