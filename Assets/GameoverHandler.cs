using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameoverHandler : MonoBehaviour
{
    public GameObject Player1Wins;
    public GameObject Player2Wins;


    private void OnEnable()
    {
        Player1Wins.SetActive(false);
        Player2Wins.SetActive(false);
        switch (GameManager.Game.player1Won)
        {
            case true:
                Player1Wins.SetActive(true);
                break;
            case false:
                Player2Wins.SetActive(true);
                break;
        }
    }
}
