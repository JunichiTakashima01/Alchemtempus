using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SlimeBossEnemy : Enemy
{
    public float bulletSpeed = 5f;
    public float useAbilityCD = 5f;
    private int abilityCycleCount = 0;

    //machinegun attack
    public float machinegunAttackInterval = 0.15f;
    public int machinegunAttackTimes = 10;

    //shotgun attack
    public float shotgunAttackAngleBetweenBulletsDegree = 10f; //
    public int shotGunAttackBulletNumberInHalf = 3;

    //multiple shitgunattack
    public int MultipleShotGunAttackTimes = 3;
    public float MultipleShotGunAttackInterval = 0.3f;

    //dash player ability
    public float dashSpeed = 10f;
    public float dashDistance = 30f;

    private bool canAttack = true;
    private bool canChangeFacingDirection = true;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (canAttack && PlayerDetected())
        {
            UseAbility();
            StartCoroutine(UseAbilityCD());
        }
        if (collidingPlayer)
        {
            DoDMGToPlayer(damage);
        }
        if (canChangeFacingDirection)
        {
            CheckFacingDirection();
            ChangeFacingDirection();
        }
    }

    // void SlimeBossAttackLogic()
    // {

    // }

    private IEnumerator UseAbilityCD()
    {
        canAttack = false;
        yield return new WaitForSeconds(useAbilityCD);
        canAttack = true;
    }

    private void UseAbility()
    {
        switch (abilityCycleCount)
        {
            case 0:
                MachinegunAttack();
                break;
            case 1:
                ShotGunAttack();
                break;
            case 2:
                MultipleShotGunAttack();
                break;
            case 3:
                DashPlayerAbility();
                break;
            default:
                break;
        }
        abilityCycleCount += 1;
        if (abilityCycleCount == 4)
        {
            abilityCycleCount = 0;
        }


    }

    protected void MachinegunAttack()
    {
        StartCoroutine(MachinegunAttackCoroutine());
    }

    private IEnumerator MachinegunAttackCoroutine()
    {
        for (int i = 0; i < machinegunAttackTimes; i++)
        {
            Vector2 direction = player.transform.position - this.transform.position;
            direction = direction.normalized;
            GameObject bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
            bullet.GetComponent<EnemyBullet>().SetBulletDamage(bulletDamage);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

            yield return new WaitForSeconds(machinegunAttackInterval);
        }
    }

    protected void ShotGunAttack()
    {
        Vector2 direction = player.transform.position - this.transform.position;
        direction = direction.normalized;
        float angleToPlayerRad = Mathf.Atan2(direction.y, direction.x);

        //setting the bullet point straight to the player
        GameObject bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<EnemyBullet>().SetBulletDamage(bulletDamage);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;

        float shotgunAttackAngleBetweenBulletsRadian = shotgunAttackAngleBetweenBulletsDegree * Mathf.Deg2Rad; //convert from degree to radian

        //setting other bullets
        for (int i = 0; i < shotGunAttackBulletNumberInHalf; i++)
        {
            GameObject bulletTop = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);

            Vector2 bulletTopDirection = new Vector2(Mathf.Cos(angleToPlayerRad + i * shotgunAttackAngleBetweenBulletsRadian), Mathf.Sin(angleToPlayerRad + i * shotgunAttackAngleBetweenBulletsRadian));

            bulletTop.GetComponent<EnemyBullet>().SetBulletDamage(bulletDamage);
            bulletTop.GetComponent<Rigidbody2D>().linearVelocity = bulletTopDirection * bulletSpeed;

            GameObject bulletDown = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);

            Vector2 bulletDownDirection = new Vector2(Mathf.Cos(angleToPlayerRad - i * shotgunAttackAngleBetweenBulletsRadian), Mathf.Sin(angleToPlayerRad - i * shotgunAttackAngleBetweenBulletsRadian));

            bulletDown.GetComponent<EnemyBullet>().SetBulletDamage(bulletDamage);
            bulletDown.GetComponent<Rigidbody2D>().linearVelocity = bulletDownDirection * bulletSpeed;

        }
    }

    protected void MultipleShotGunAttack()
    {
        StartCoroutine(MultipleShotGunAttackCoroutine());
    }

    private IEnumerator MultipleShotGunAttackCoroutine()
    {
        for (int i = 0; i < MultipleShotGunAttackTimes; i++)
        {
            ShotGunAttack();
            yield return new WaitForSeconds(MultipleShotGunAttackInterval);
        }
    }

    protected void DashPlayerAbility()
    {
        StartCoroutine(DashPlayerAbilityCoroutine());
    }

    private IEnumerator DashPlayerAbilityCoroutine()
    {
        canChangeFacingDirection = false;
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
        float facingDirection = direction;
        rb.linearVelocity = new Vector2(dashSpeed * facingDirection, 0);
        yield return new WaitForSeconds(dashDistance / dashSpeed);
        rb.linearVelocity = new Vector2(0, 0);
        canChangeFacingDirection = true;
    }

}
