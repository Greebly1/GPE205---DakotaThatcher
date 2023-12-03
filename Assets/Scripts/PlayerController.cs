using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public enum playerState { alive, dead}


[System.Serializable]
public class PlayerController : Controller {

    private playerState state;
    private PlayerInput inputAsset;
    private TankMovement pawnMovement;

    public Action<playerState> playerStateChanged = delegate { };

    [SerializeField] Camera playerCam;
    [HideInInspector] public PlayerSpawn spawner;
    // Start is called before the first frame update

    #region start/update
    public override void InitPawn(Pawn possessedPawn) {
        base.InitPawn(possessedPawn);
        switchState(playerState.alive);
        inputAsset.SwitchCurrentActionMap("Player");
        possessedPawn.pawnDestroyed += death;

        pawnMovement = pawn.GetComponent<TankMovement>();
    }
    
    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>();
        Debug.Log("Input initialized for " + inputAsset.currentControlScheme);
        switchState(playerState.dead);
        enableInput();
    }

    private void switchState(playerState newState)
    {
        if (state != newState)
        {
            state = newState;
            playerStateChanged.Invoke(state);
        }
    }

    // Update is called once per frame
    public override void Update() {
        switch (state)
        {
            case playerState.alive:
                transform.position = pawn.transform.position;
                transform.rotation = pawn.transform.rotation;
                break;
            case playerState.dead:

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
        float brakeInput = context.action.ReadValue<float>();
        pawnMovement.brakeInput = brakeInput;
        //Debug.Log("BRAKING:" + brakeInput);
    }

    private void movement(InputAction.CallbackContext context)
    {

        Vector2 moveInput = context.action.ReadValue<Vector2>();
        pawnMovement.throttleInput = moveInput.y;
        //Debug.Log("MOVING: " + moveInput);
    }

    private void turn(InputAction.CallbackContext context )
    {
        Vector2 turnInput = context.action.ReadValue<Vector2>();
        pawnMovement.turnInput = turnInput.x;
        //Debug.Log("Turning: " + turnInput);
    }

    private void deadInput(InputAction.CallbackContext context)
    {
        Debug.Log("Dead input");
        spawner.spawnPawn(this);
    }

    #endregion

    #region enable/disable input
    /// <summary>
    /// Enable all input and bind functions to input events
    /// </summary>
    public void enableInput() {
        inputAsset.onActionTriggered += onAction;
    }

    private void onAction(InputAction.CallbackContext callback)
    {
        switch(callback.action.name)
        {
            case "Movement":
                movement(callback);
                break;
            case "Fire1":
                mouse1(callback);
                break;
            case "Brake":
                brake(callback);
                break;
            case "Turning":
                turn(callback);
                break;
            case "DeadInput":
                deadInput(callback);
                break;
        }
        return;
    }


    /// <summary>
    /// Disable all input and unbind functions to input events
    /// </summary>
    public void disableInput()
    {
        inputAsset.onActionTriggered -= onAction;
    }

    public void OnDestroy()
    {
        if (GameManager.Game != null)
        {
            GameManager.Game.players.Remove(this);
        }
        Destroy(pawn);
        Debug.Log("Destroyed player controller");
    }

    #endregion

    public override void death()
    {
        state = playerState.dead;
        inputAsset.SwitchCurrentActionMap("Dead");
    
    }
}
