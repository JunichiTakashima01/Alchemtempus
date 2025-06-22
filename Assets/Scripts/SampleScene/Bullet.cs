using System.Data.Common;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletLiveTime = 2f;
    public float bulletDamage = 1f;
    public float bulletKnockBackDistance = 0.4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DestroyBulletAfterLiveTime();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(bulletDamage, bulletKnockBackDistance);
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

}
