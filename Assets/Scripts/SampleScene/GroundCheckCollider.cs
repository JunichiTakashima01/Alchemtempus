using System;
using UnityEngine;

public class GroundCheckCollider : MonoBehaviour
{

    public PlayerMovement playerMovement;
    private int collisionObjectCount = 0;


    void OnTriggerEnter2D(Collider2D collision)
    {
        collisionObjectCount += 1;
        playerMovement.SetIsGrounded(true);
        if (collision.CompareTag("Platform"))
        {
            playerMovement.SetOnPlatform(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        collisionObjectCount -= 1;
        if (collisionObjectCount == 0)
        {
            playerMovement.SetIsGrounded(false);
        }
        if (collision.CompareTag("Platform"))
        {
            playerMovement.SetOnPlatform(false);
        }
    }

}
