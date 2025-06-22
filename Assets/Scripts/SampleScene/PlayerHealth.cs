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
    public float ShieldedDamageCoolDown = 0.4f;

    private bool ableToTakeDamage = true;
    private bool isShielding = false;

    private int enemyColliding = 0;
    private List<Enemy> enemies = new List<Enemy>();

    private SpriteRenderer spriteRenderer;
    Color ogColor;
    private float flashDelay = 0.2f; //0.2s


    //private Coroutine takeDamageCDCoroutine = null;


    void Awake()
    {
        PlayerMovement.OnDropping += ResetEnemyCollidingCount;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currHealth = maxHealth;
        healthBarUI.setCurrAndMaxHealth(currHealth, maxHealth);

        spriteRenderer = this.GetComponent<SpriteRenderer>();
        ogColor = spriteRenderer.color;
    }

    void OnDestroy()
    {
        PlayerMovement.OnDropping -= ResetEnemyCollidingCount;
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



    public void TakeDamage(float damage, float knockBackDistance = 0f, float bulletVelocity = 0f)
    {
        if (ableToTakeDamage)
        {
            if (!isShielding)
            {
                currHealth -= damage;

                if (knockBackDistance != 0f)
                {
                    float direction = Mathf.Sign(bulletVelocity);
                    this.transform.position += new Vector3(direction * knockBackDistance, 0, 0);
                }
                if (currHealth <= 0)
                {
                    currHealth = 0;
                    OnPlayerZeroHealth.Invoke();
                }

                healthBarUI.SetHealthFiller(currHealth, maxHealth);

                StartCoroutine(FlashColor(Color.red));
                StartCoroutine(TakeDamageCD(ShieldedDamageCoolDown));
            }
            else
            {
                this.GetComponent<PlayerMana>().ShieldDmg(damage);
                StartCoroutine(TakeDamageCD(takeDamageCoolDown * 0.4f));
            }
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

    private IEnumerator TakeDamageCD(float cd)
    {
        ableToTakeDamage = false;
        yield return new WaitForSeconds(cd);
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

    private void ResetEnemyCollidingCount()
    {
        enemyColliding = 0;
    }

    public void OnShielding(bool shielding)
    {
        isShielding = shielding;
    }
}
