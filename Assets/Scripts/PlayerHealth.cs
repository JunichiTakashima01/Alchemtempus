using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static event Action OnPlayerZeroHealth;

    private float currHealth;
    private float maxHealth = 15;
    public HealthBarUI healthBarUI;
    public float takeDamageCoolDown = 1f; //1s invulnerable if taken damage

    private bool ableToTakeDamage = true;

    private int enemyColliding = 0;
    private List<Enemy> enemies = new List<Enemy>();

    private SpriteRenderer spriteRenderer;
    Color ogColor;
    private float flashDelay = 0.2f; //0.2s


    //private Coroutine takeDamageCDCoroutine = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currHealth = maxHealth;
        healthBarUI.setCurrAndMaxHealth(currHealth, maxHealth);

        spriteRenderer = this.GetComponent<SpriteRenderer>();
        ogColor = spriteRenderer.color;
    }

    //Update is called once per frame
    void Update()
    {
        if (ableToTakeDamage && enemyColliding > 0)
        {
            foreach (Enemy enemy in enemies)
            {
                TakeDamage(enemy.damage);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (ableToTakeDamage)
        {
            currHealth -= damage;

            if (currHealth <= 0)
            {
                currHealth = 0;
                OnPlayerZeroHealth.Invoke();
            }

            healthBarUI.SetHealthFiller(currHealth, maxHealth);

            StartCoroutine(FlashColor(Color.red));
            StartCoroutine(TakeDamageCD());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemyColliding += 1;
            enemies.Add(enemy);
            TakeDamage(enemy.damage);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemies.Remove(enemy);
            enemyColliding -= 1;
        }
    }

    private IEnumerator TakeDamageCD()
    {
        ableToTakeDamage = false;
        yield return new WaitForSeconds(takeDamageCoolDown);
        ableToTakeDamage = true;

        spriteRenderer.color = ogColor;
    }

    private IEnumerator FlashColor(Color color)
    {
        Color ogColor = spriteRenderer.color;
        spriteRenderer.color = color;
        yield return new WaitForSeconds(flashDelay);
        //turn half transparent to show invulnerability
        Color currColor = ogColor;
        currColor.a = 0.5f; // Change alpha value to half transparent
        spriteRenderer.color = currColor;
    }
}
