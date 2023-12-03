using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Variables
    /// <summary>
    /// Game Manager singleton, do not instantiate
    /// </summary>
    public static GameManager Game;
    public gameState state;
    [SerializeField] private gameState startState;

    [SerializeField] private GameObject titleState;
    [SerializeField] private GameObject menuState;
    [SerializeField] private GameObject gameplayState;
    [SerializeField] private GameObject gameoverState;

    [SerializeField] private MapGenerator generator;



    public GameObject prefab_playerController;
    public GameObject prefab_playerPawn;
    public GameObject prefab_playerCamera;

    public PlayerController player1;
    public PlayerController player2;

    public List<PlayerController> players;
    public List<BaseAiController> enemyAIs;
    #endregion

    // Awake called before the game starts
    void Awake() {
        // if there is no instance of this game manager
        if(Game == null) {
            Game = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        players = new List<PlayerController> ();
       
    }

    private void Update()
    {
        //Debug.Log("FPS last frame = " + 1f / Time.deltaTime);
        
        if (Input.GetKey(KeyCode.Alpha1)) { SetState(gameState.TitleScreen); }
        if (Input.GetKey(KeyCode.Alpha2)) { SetState(gameState.MainMenu); }
        if (Input.GetKey(KeyCode.Alpha3)) { SetState(gameState.GamePlay); }
        if (Input.GetKey(KeyCode.Alpha4)) { SetState(gameState.GameOver); }
        

    }

    private void Start() {
        SetState(startState);
    }

    public void SetState(gameState newState)
    {
        if (state != newState)
        {
            CheckStateEnds(state);
            ClearStates();
            state = newState; 
            CheckStateBegins(state);
        }
    }

    private void ClearStates()
    {
        titleState.SetActive(false);
        gameoverState.SetActive(false);
        menuState.SetActive(false);
        gameplayState.SetActive(false);
    }

    private void CheckStateBegins(gameState state)
    {
        switch (state)
        {
            case gameState.GamePlay: 
                BeginStateGameplay();
                return;

            case gameState.GameOver: 
                BeginStateGameOver();
                return;

            case gameState.TitleScreen: 
                BeginStateTitleScreen();
                return;

            case gameState.MainMenu: 
                BeginStateMainMenu();
                return;
        }
    }

    private void CheckStateEnds(gameState state)
    {
        switch (state)
        {
            case gameState.GamePlay: 
                EndStateGameplay();
                return;

            case gameState.GameOver: 
                EndStateGameOver();
                return;

            case gameState.TitleScreen: 
                EndStateTitleScreen();
                return;

            case gameState.MainMenu: 
                EndStateMainMenu();
                return;
        }
    }

    #region beginGameStateMethods
    private void BeginStateGameplay()
    {
        Debug.Log("begin gameplay state");
        gameplayState.SetActive(true);
        generator.Generate();
    }
    private void BeginStateGameOver()
    {
        Debug.Log("begin gameover state");
        gameoverState.SetActive(true);
    }
    private void BeginStateTitleScreen()
    {
        Debug.Log("begin titlescreen state");
        titleState.SetActive(true);
    }
    private void BeginStateMainMenu()
    {
        Debug.Log("begin mainmenu state");
        menuState.SetActive(true);
    }
    #endregion

    #region endGameStateMethods
    private void EndStateGameplay()
    {
        Debug.Log("End gameplay state");
        generator.destroyMap();
        foreach (BaseAiController enemy in enemyAIs)
        {
            Destroy(enemy.pawn.gameObject);
        }
        foreach (PlayerController player in players)
        {
            if(player.pawn != null) Destroy(player.pawn.gameObject);
            Destroy(player.gameObject);
        }
        enemyAIs = new List<BaseAiController>();
    }

    private void EndStateGameOver()
    {
        Debug.Log("End gameover state");
    }
    private void EndStateTitleScreen()
    {
        Debug.Log("End titlescreen state");
    }
    private void EndStateMainMenu()
    {
        Debug.Log("End mainmenu state");
    }
    #endregion

    public PlayerController addPlayer(PlayerController player)
    {
        if (player1 == null)
        {
            player1 = player;
            players.Add(player);
            return player;
        } else if (player2 == null)
        {
            player2 = player;
            players.Add(player2);
            return player;
        } else
        {
            Destroy(player.gameObject);
            return null;
        }
    }

    public enum gameState
    {
        GamePlay,
        TitleScreen,
        MainMenu,
        GameOver
    }
}