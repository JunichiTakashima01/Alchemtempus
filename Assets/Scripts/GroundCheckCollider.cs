using System;
using UnityEngine;

public class GroundCheckCollider : MonoBehaviour
{
    public static event Action<bool> OnTouchingGround;
    private int collisionObjectCount = 0;

    void OnTriggerEnter2D(Collider2D collision)
    {
        collisionObjectCount += 1;
        OnTouchingGround.Invoke(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        collisionObjectCount -= 1;
        if (collisionObjectCount == 0)
        {
            OnTouchingGround.Invoke(false);
        }
    }

}
