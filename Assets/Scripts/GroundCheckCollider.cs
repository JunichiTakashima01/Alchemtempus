using System;
using UnityEngine;

public class GroundCheckCollider : MonoBehaviour
{
    public static event Action<bool> OnTouchingGround;

    void OnTriggerEnter2D(Collider2D collision)
    {
        OnTouchingGround.Invoke(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        OnTouchingGround.Invoke(false);
    }
}
