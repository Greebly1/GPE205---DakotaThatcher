using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Variables
    /// <summary>
    /// Game Manager singleton, do not instantiate
    /// </summary>
    public static GameManager instance;


    public GameObject playerController;
    public Transform playerSpawn;

    #endregion

    // Awake called before the game starts
    void Awake() {
        // if there is no instance of this game manager
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        SpawnPlayer(playerSpawn);
    }

    public void SpawnPlayer(Transform position) {
        
    }

}
