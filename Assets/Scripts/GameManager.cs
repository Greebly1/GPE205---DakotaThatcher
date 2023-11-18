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

    public PlayerController player;

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
        state = startState;
        
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
        Debug.Log(state);
        SetState(state);
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
            Destroy(player.pawn.gameObject);
            Destroy(player.gameObject);
        }
        Destroy(player.pawn.gameObject);
        Destroy(player.gameObject);
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



    /// <summary>
    /// Instantiates a player controller and a pawn at the given position,
    /// saves a reference to the player controller to the static variable 'player'
    /// lastly initializes the player controller by calling player.Init();
    /// </summary>
    /// <param name="spawnPoint"></param>
    public void SpawnPlayer(Transform spawnPoint, int playerID) {
        GameObject playerControllerobj;
        Debug.Log(playerID);
        if (players.Count >= playerID)
        {
            playerControllerobj = Instantiate(prefab_playerController, spawnPoint.position, spawnPoint.rotation);
            player = playerControllerobj.GetComponent<PlayerController>();
        } else { playerControllerobj = players[playerID].gameObject; }
        
        GameObject playerPawnobj = Instantiate(prefab_playerPawn, playerControllerobj.transform.position, playerControllerobj.transform.rotation);
        GameObject playerCameraobj = Instantiate(prefab_playerCamera, spawnPoint.position, spawnPoint.rotation);


        
        player.Init(playerPawnobj.GetComponent<TankPawn>(), playerID);
        playerCameraobj.GetComponent<cameraFollow>().focus = playerPawnobj;

        players.Add(playerControllerobj.GetComponent<PlayerController>());
    }


    public enum gameState
    {
        GamePlay,
        TitleScreen,
        MainMenu,
        GameOver
    }
}