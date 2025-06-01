using System;
using UnityEngine;

public class TripleJumpGem : Gem
{
    public static event Action<int> OnTripleJumpCollected;
    private int AirJumpTimes = 3;

    public override void Collect()
    {
        OnTripleJumpCollected.Invoke(AirJumpTimes);

        base.Collect();// Destroy this game object
    }
}
