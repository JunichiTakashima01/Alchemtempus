using System.Collections;
using UnityEngine;

public class RangedEnemy : Enemy
{
    public float bulletVelocity = 3;
    public float shootCoolDown = 1.5f;

    private bool canShoot = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // Update is called once per frame
    protected override void Update()
    {
        CheckFacingDirection();
        if (canShoot && PlayerDetected())
        {
            shootAtPlayer(bulletVelocity);
            StartCoroutine(ShootCoolDown());
        }
    }

    private IEnumerator ShootCoolDown()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootCoolDown);
        canShoot = true;
    }
}
