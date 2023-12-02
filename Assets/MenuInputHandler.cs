using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInputHandler : MonoBehaviour
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
            case "Quit":
                onQuit();
                break;
            case "Play":
                onPlay();
                break;
        }
        return;
    }

    private void onQuit()
    {
        //Debug.Log("QUITTING");
        Application.Quit();  //Hopefully this actually quits the game when the project gets built
    }

    private void onPlay()
    {
        GameManager.Game.SetState(GameManager.gameState.GamePlay);
    }
}
