using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverInputHandler : MonoBehaviour
{
    PlayerInput UIInput;

    private void Awake()
    {
        UIInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        UIInput.onActionTriggered += onAction;
    }

    private void OnDisable()
    {
        UIInput.onActionTriggered -= onAction;
    }

    private void onAction(InputAction.CallbackContext callback)
    {
        if (!callback.performed) return; //Early out if this is the wrong phase of the input action
        switch (callback.action.name)
        {
            case "Return":
                onReturn();
                break;
        }
        return;
    }

    private void onReturn()
    {
        GameManager.Game.SetState(GameManager.gameState.MainMenu);
    }
}
