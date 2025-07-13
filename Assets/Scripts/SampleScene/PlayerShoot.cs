using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed;
    private float bulletDamage = 1f;

    private bool canShoot = true; //if mouse on UI (button) then cannot shoot.

    private bool gamePaused = false;

    void Start()
    {
        GameController.OnGamePausedChangePauseStatus += SetGamePauseStatus;
        PauseButtonMouseDetect.OnMouseEnterUIStatus += SetCanShoot;
    }

    void OnDestroy()
    {
        GameController.OnGamePausedChangePauseStatus -= SetGamePauseStatus;
        PauseButtonMouseDetect.OnMouseEnterUIStatus -= SetCanShoot;
    }

    private void SetGamePauseStatus(bool gamePaused)
    {
        this.gamePaused = gamePaused;
    }

    private void SetCanShoot(bool mouseInUI)
    {
        this.canShoot = !mouseInUI; //If mouse on button then cannot shoot
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started && !gamePaused && canShoot)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
            mousePosition.z = 0;

            Vector3 shootDirection = (mousePosition - this.transform.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().SetBulletDamage(bulletDamage);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * bulletSpeed;
        }
    }

    public float GetPlayerBulletDamage()
    {
        return bulletDamage;
    }

    public void SetPlayerBulletDamage(float bulletDamage)
    {
        this.bulletDamage = bulletDamage;
    }

    public void IncreasePlayerBulletDamageOne()
    {
        bulletDamage += 1;
    }
}
