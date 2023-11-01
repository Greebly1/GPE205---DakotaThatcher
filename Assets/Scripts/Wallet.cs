using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The wallet is an object that contains the values for currency as well as the methods and logic for changing those values

public class wallet : MonoBehaviour
{
    public int maxCoins = 100;

    [SerializeField] private int currentCoins = 0;

    public int CurrentCoins { get { return currentCoins; } }

    //TODO:
    //Create methods for spending currency in an item shop/ upgrade shop


    public void addMoney(int amount)
    {
        currentCoins += amount;
        clampWallet();
    }

    private void clampWallet()
    {
        Mathf.Clamp(currentCoins, 0, maxCoins);
    }

    public bool canAddCoins(int amount)
    {
        if (currentCoins + amount > maxCoins)
        {
            return false;
        }
        else return true;
    }
}
