using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
            mousePosition.z = 0;

            Vector3 shootDirection = (mousePosition - this.transform.position).normalized;

            GameObject bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * bulletSpeed;


        }
    }
}
