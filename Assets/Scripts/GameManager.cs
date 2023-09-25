using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Variables
    /// <summary>
    /// Game Manager singleton, do not instantiate
    /// </summary>
    public static GameManager Game;


    public GameObject prefab_playerController;
    public GameObject prefab_playerPawn;
    public Transform playerSpawn;

    public PlayerController player;

    public List<PlayerController> players;
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
        players = new List<PlayerController>();
    }

    private void Start() {
        SpawnPlayer(playerSpawn);
    }

    /// <summary>
    /// Instantiates a player controller and a pawn at the given position,
    /// saves a reference to the player controller to the static variable 'player'
    /// lastly initializes the player controller by calling player.Init();
    /// </summary>
    /// <param name="spawnPoint"></param>
    public void SpawnPlayer(Transform spawnPoint) {
        GameObject playerControllerobj = Instantiate(prefab_playerController, spawnPoint.position, spawnPoint.rotation);
        GameObject playerPawnobj = Instantiate(prefab_playerPawn, playerControllerobj.transform.position, playerControllerobj.transform.rotation);

        player = playerControllerobj.GetComponent<PlayerController>();
        player.Init(playerPawnobj.GetComponent<TankPawn>());
    }

}