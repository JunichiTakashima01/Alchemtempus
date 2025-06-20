using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private float currHealth;
    private float maxHealth = 15;
    public HealthBarUI healthBarUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currHealth = maxHealth;
        healthBarUI.setCurrAndMaxHealth(currHealth, maxHealth);

    }

    // Update is called once per frame
    // void Update()
    // {

    // }

    public void TakeDamage(float damage)
    {
        currHealth -= damage;

        if (currHealth <= 0)
        {
            currHealth = 0;
            //player pause
        }

        healthBarUI.SetHealthFiller(currHealth, maxHealth);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            TakeDamage(enemy.damage);
        }
    }
}
