using System;
using UnityEngine;

public class GrowLargeGem : Gem
{
    public static event Action<float> OnGrowLargeCollected;
    private float scaleAdditionner = 1.5f;

    public override void Collect()
    {
        OnGrowLargeCollected.Invoke(scaleAdditionner);

        base.Collect();// Destroy this game object
    }
}
