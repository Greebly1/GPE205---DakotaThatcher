using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    bool playerSpawned;
    public GameObject controller;
    [SerializeField] GameObject _pawnPrefab;
    [SerializeField] GameObject _controllerPrefab;
    void Start()
    {
        spawnController();
    }

    void Awake()
    {
        playerSpawned = false;
        
    }

    PlayerController spawnController()
    {
        Debug.Log("Spawning controller");
        if (!playerSpawned)
        {
            controller = Instantiate(_controllerPrefab, transform.position, transform.rotation);
            controller.GetComponent<PlayerController>().spawner = this;
            GameManager.Game.addPlayer(controller.GetComponent<PlayerController>());
            playerSpawned = true;
            return controller.GetComponent<PlayerController>();
        }
        else
            return null;
    }

    public TankPawn spawnPawn(PlayerController player)
    {
        Debug.Log("Spawning pawn");
        if (player.pawn == null) {
            TankPawn pawn = Instantiate(_pawnPrefab, transform.position, transform.rotation).GetComponent<TankPawn>();
            player.InitPawn(pawn);
            return pawn;
        }
        else
            return null;
    }
}
