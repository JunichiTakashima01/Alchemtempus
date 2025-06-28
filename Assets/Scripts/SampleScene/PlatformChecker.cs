using UnityEngine;

public class PlatformChecker : MonoBehaviour
{
    public PlayerMovement playerMovement;

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            playerMovement.EnableCollisionWithPlatforms();
        }
    }
}
