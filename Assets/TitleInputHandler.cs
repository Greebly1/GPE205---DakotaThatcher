using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TitleInputHandler : MonoBehaviour
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
        switch (callback.action.name)
        {
            case "Submit":
                onSubmit();
                break;
        }
        return;
    }

    private void onSubmit()
    {
        GameManager.Game.SetState(GameManager.gameState.MainMenu);
    }

}
