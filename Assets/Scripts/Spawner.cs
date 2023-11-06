using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnPrefab;

    private GameObject currentObject;

    [SerializeField] private int spawnCount = 3;
    private int currentSpawnCount;

    [SerializeField] private bool endless = false;


    [SerializeField] private float timeBetweenSpawns = 3.0f;

    private float nextSpawnTime;
    public enum spawnerState { spawned, spawntimer }

    private spawnerState _currentState;

    private void Awake()
    {
        currentSpawnCount = spawnCount;
        spawn();
    }


    /// <summary>
    /// if this spawner can spawn something, spawn the gameobject and set the state to spawned
    /// </summary>
    private void spawn()
    {
        if (currentSpawnCount > 0 || endless)
        {
            currentObject = Instantiate(spawnPrefab, this.gameObject.transform.position, this.gameObject.transform.rotation);
            _currentState = spawnerState.spawned;
            currentSpawnCount--;
        }  else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        switch(_currentState)
        {
            case spawnerState.spawned:
                checkObject();
                break;
            case spawnerState.spawntimer:
                checkTimer();
                break;
        }
    }

    private void checkTimer()
    {
        if (Time.time >= nextSpawnTime)
        {
            spawn();
        }
    }

    private void checkObject()
    {
        if (currentObject == null)
        {
            _currentState = spawnerState.spawntimer;
            nextSpawnTime = Time.time + timeBetweenSpawns;
        }
    }
}
