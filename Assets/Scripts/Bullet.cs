using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float bulletLiveTime = 2f;
    private float bulletDamage = 1f;
    private float bulletKnockBackDistance = 0.4f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DestroyBullet();
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.takeDamage(bulletDamage, bulletKnockBackDistance);
            Destroy(this.gameObject);
        }
    }


        private void DestroyBullet()
    {
        Destroy(this.gameObject, bulletLiveTime); //Destroy after bulletLiveTime seconds
    }

}
