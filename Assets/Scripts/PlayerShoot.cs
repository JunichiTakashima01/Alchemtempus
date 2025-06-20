using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed;

    private bool gamePaused = false;

    void Awake()
    {
        GameController.OnGamePausedChangePauseStatus += SetGamePauseStatus;
    }

    private void SetGamePauseStatus(bool gamePaused)
    {
        this.gamePaused = gamePaused;
    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started && !gamePaused)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
            mousePosition.z = 0;

            Vector3 shootDirection = (mousePosition - this.transform.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * bulletSpeed;
        }
    }
}
