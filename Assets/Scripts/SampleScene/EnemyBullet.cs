using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float bulletLiveTime = 20f;
    public float bulletDamage = 1f;
    public float bulletKnockBackDistance = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DestroyBulletAfterLiveTime();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(bulletDamage, bulletKnockBackDistance, this.GetComponent<Rigidbody2D>().linearVelocityX);
            Destroy(this.gameObject);
        }
        if (collision.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
    private void DestroyBulletAfterLiveTime()
    {
        Destroy(this.gameObject, bulletLiveTime); //Destroy after bulletLiveTime seconds
    }

    public void SetBulletDamage(float bulletDamage)
    {
        this.bulletDamage = bulletDamage;
    }
}
