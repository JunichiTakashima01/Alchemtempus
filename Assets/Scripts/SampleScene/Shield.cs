using System.Collections;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public PlayerMana playerMana;
    public float shieldCoolDown = 1f;

    private bool canShield = true;

    private Coroutine shieldCoolDownCoroutine = null;

    void Awake()
    {
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<CircleCollider2D>().enabled = false;
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void ShieldDamage(float damage)
    {
        playerMana.ShieldDmg(damage);
    }

    // Update is called once per frame
    public bool StartShield() // return true if shielding
    {
        if (canShield)
        {
            this.GetComponent<CircleCollider2D>().enabled = true;
            this.GetComponent<SpriteRenderer>().enabled = true;
            shieldCoolDownCoroutine = StartCoroutine(ShieldCoolDownCoroutine(shieldCoolDown));

            return true;
        }
        return false;
    }

    public void StopShield()
    {
        this.GetComponent<CircleCollider2D>().enabled = false;
        this.GetComponent<SpriteRenderer>().enabled = false;
    }

    private IEnumerator ShieldCoolDownCoroutine(float shieldCoolDown)
    {
        canShield = false;
        yield return new WaitForSeconds(shieldCoolDown);
        canShield = true;
    }

    public void ResetCoolDown()
    {
        StopCoroutine(shieldCoolDownCoroutine);
        canShield = true;
    }
}
