using System;
using UnityEngine;

public class CoinGem : Gem
{
    public static event Action<int> OnCoinGemCollected;
    private int coin = 1;

    public override void Collect()
    {
        OnCoinGemCollected.Invoke(coin);

        base.Collect();// Destroy this game object
    }
}