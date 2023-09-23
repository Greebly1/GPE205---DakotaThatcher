using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller {

    private PlayerInput playerInput;
    private InputAction input_movement;
    // Start is called before the first frame update

    public override void Init(Pawn possessedPawn) {
        base.Init(possessedPawn);
        playerInput = new PlayerInput();
        input_movement = playerInput.Player.Movement;
        OnEnable();
    }
    public override void Start() {
        base.Start();   
    }

    // Update is called once per frame
    public override void Update() {
        base.Update();
        Debug.Log("Movement Values: " + input_movement.ReadValue<Vector2>());
    }

    /// <summary>
    /// Enable all input and bind functions to input events
    /// </summary>
    public void OnEnable() {
        input_movement.Enable();
    }

    /// <summary>
    /// Disable all input and unbind functions to input events
    /// </summary>
    public void OnDisable()
    {
        input_movement.Disable();
    }
}
