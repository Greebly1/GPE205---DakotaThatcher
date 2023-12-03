using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public PlayerController controller;
    public GameObject HUDCanvas;
    public GameObject DeathCanvas;
    public GameObject healthProgressBar;

    float health = 0;
    float maxHealth = 10;

    private void Start()
    {
        controller.playerStateChanged += handleStateChange;
    }

    private void Update()
    {
        if (controller.pawn != null)
        {
            Health pawnHealth = controller.pawn.gameObject.GetComponent<Health>();
            health = pawnHealth.getHealth();
            maxHealth = pawnHealth.maxHealth;
        } else
        {
            health = 0;
            
        }
        healthProgressBar.transform.localScale = new Vector3(health / maxHealth, 1, 1);
    }

    private void handleStateChange(playerState state)
    {
        switch (state)
        {
            case playerState.dead:
                DeathCanvas.SetActive(true); 
                HUDCanvas.SetActive(false);
                break;
            case playerState.alive:
                HUDCanvas.SetActive(true);
                DeathCanvas.SetActive(false);
                break;
        }
    }
}
