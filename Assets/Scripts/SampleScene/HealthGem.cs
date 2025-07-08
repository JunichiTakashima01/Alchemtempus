using System;
using UnityEngine;

public class HealthGem : Gem
{
    public static event Action<int> OnHealthGemCollected;
    private int health = 3;

    public override void Collect()
    {
        OnHealthGemCollected.Invoke(health);

        base.Collect();// Destroy this game object
    }
}