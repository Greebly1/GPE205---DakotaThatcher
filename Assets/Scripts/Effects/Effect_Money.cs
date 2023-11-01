using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect_Money : Effect
{
    wallet _receiverWallet;

    int _moneyAmount;
    public Effect_Money(GameObject Receiver, int amount) { 
        _receiver = Receiver;
        _receiverWallet = _receiver.GetComponent<wallet>();
        _moneyAmount = amount;
        useUndo = false;
    }

    public override void apply()
    {
        Debug.Log("money applied + " + _moneyAmount + " coins!");
        _receiverWallet.addMoney(_moneyAmount);
    }

    public override void undo()
    {
        Debug.Log("wallet should not be undone");
    }

    public override bool canApply()
    {

        Debug.Log("Can effect be applied?");
        if (_receiverWallet != null)
        {
            Debug.Log("Receiver has a Wallet component");
            if (_receiverWallet.canAddCoins(_moneyAmount))
            {
                Debug.Log("effect can be applied because receiver wallet has room for " + _moneyAmount + " more coins");
                return true;
            }
            Debug.Log("Receiver wallet is full");
        }
        Debug.Log("Effect cannot be applied");
        return false;
    }
}
