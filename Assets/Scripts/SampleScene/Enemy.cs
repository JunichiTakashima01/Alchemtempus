using System;
using System.Collections;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Transform player; //for this enemy object to chase

    public GameObject bulletPrefab;
    public float chaseSpeed;
    public float jumpForce;
    public LayerMask groundLayer;

    public float damage = 1;
    public float bulletDamage = 3;

    public float detectingSight = 9;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;
    //private bool isJumping;
    private float LengthFromCenterToBottom;
    private float LengthFromCenterToSide;
    private float direction;

    protected bool collidingPlayer = false;

    public float maxHealth = 15;
    private float currHealth;
    public bool immuneToKnockBack = false;

    
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public static event Action OnEnemyKilled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        player = GameObject.Find("Player").transform;

        rb = this.GetComponent<Rigidbody2D>();

        LengthFromCenterToBottom = this.GetComponent<BoxCollider2D>().size.y / 2f;
        LengthFromCenterToSide = this.GetComponent<BoxCollider2D>().size.x / 2f;

        currHealth = maxHealth;
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        IsGrounded();
        CheckFacingDirection();
        if (PlayerDetected())
        {
            Move();
            DecideJump();
        }
        if (collidingPlayer)
        {
            DoDMGToPlayer(damage);
        }
    }

    protected void Move()
    {
        rb.linearVelocityX = direction * chaseSpeed;
    }

    protected void DecideJump()
    {
        //Player Above?
        //bool isPlayerAbove = Physics2D.Raycast(this.transform.position, Vector2.up, LengthFromCenterToBottom + 3f, 1 << player.gameObject.layer); // 1 << player.gameObject.layer provides the layermask of player object
        bool isPlayerAbove = player.position.y > (this.transform.position.y + LengthFromCenterToBottom);
        bool isPlayerBelow = player.position.y < (this.transform.position.y - LengthFromCenterToBottom);

        if (isGrounded)
        {

            //Jump if there is a gap ahead and no ground in front
            //else if there is plaver above and platform above

            //If ground in front (blocking enemy from moving forward)
            bool groundInFront = Physics2D.Raycast(this.transform.position, new Vector2(direction, 0), LengthFromCenterToSide + 0.2f, groundLayer) || Physics2D.Raycast(this.transform.position + new Vector3(0, LengthFromCenterToBottom * 0.5f, 0), new Vector2(direction, 0), LengthFromCenterToSide + 0.2f, groundLayer);
            //If gap ahead
            RaycastHit2D groundAhead = Physics2D.Raycast(this.transform.position + new Vector3(direction * LengthFromCenterToSide, 0, 0), Vector2.down, LengthFromCenterToBottom + 0.05f, groundLayer);
            //If platform above directly
            RaycastHit2D platformAbove = Physics2D.Raycast(this.transform.position, Vector2.up, LengthFromCenterToBottom + 2f, groundLayer);

            if (!isPlayerBelow && !groundInFront && !groundAhead) //Indicate a gap ahead
            {
                shouldJump = true;
            }
            else if (!isPlayerBelow && groundInFront)//obstacle in front
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove)//Player at the platform above
            {
                shouldJump = true;
            }
        }

        //Decide whether to jump
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            Vector2 jumpDirection = jumpForce * (player.position - this.transform.position).normalized;

            //isJumping = true;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            //rb.linearVelocityY = jumpForce;
        }
    }

    protected void IsGrounded()
    {
        //If moving upward then it is not grounded
        if (rb.linearVelocityY > 0)
        {
            isGrounded = false;
        }
        else
        {
            //Is Grounded?
            isGrounded = Physics2D.Raycast(this.transform.position, Vector2.down, LengthFromCenterToBottom + 0.05f, groundLayer);
            //Check both side if touching ground
            isGrounded = isGrounded || Physics2D.Raycast(this.transform.position + new Vector3(LengthFromCenterToSide, 0, 0), Vector2.down, LengthFromCenterToBottom + 0.05f, groundLayer);
            isGrounded = isGrounded || Physics2D.Raycast(this.transform.position + new Vector3(-LengthFromCenterToSide, 0, 0), Vector2.down, LengthFromCenterToBottom + 0.05f, groundLayer);
        }
    }

    protected void CheckFacingDirection()
    {
        //Player Direction?
        direction = Mathf.Sign(player.position.x - this.transform.position.x);
    }

    protected void shootAtPlayer(float velocity)
    {
        Vector3 aimPosition = player.position;

        Vector3 shootDirection = (aimPosition - this.transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, this.transform.position, Quaternion.identity);
        bullet.GetComponent<EnemyBullet>().SetBulletDamage(bulletDamage);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * velocity;
    }

    public bool PlayerDetected()
    {
        return detectingSight > Vector3.Distance(player.position, this.transform.position);// if too far the enemy can't see player thus no movement.
    }

    public void TakeDamage(float dmg, float knockBackDistance = 0f)
    {
        currHealth -= dmg;
        StartCoroutine(FlasPink());
        if (currHealth <= 0)
        {
            DestroyEnemy();
        }
        if (!immuneToKnockBack)
        {
            this.transform.position -= new Vector3(direction * knockBackDistance, 0, 0);
        }
    }

    private IEnumerator FlasPink()
    {
        spriteRenderer.color = Color.lightPink;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }

    private void DestroyEnemy()
    {
        OnEnemyKilled.Invoke();
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            collidingPlayer = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            collidingPlayer = false;
        }
    }

    protected virtual void DoDMGToPlayer(float dmg)
    {
        player.GetComponent<PlayerHealth>().TakeDamage(dmg);
    }

    protected void ChangeFacingDirection()
    {
        if (direction == -1 && this.transform.localScale.x < 0)
        {
            Flip();
        }
        else if (direction == 1 && this.transform.localScale.x > 0)
        {
            Flip();
        }
    }

    protected void Flip() //flip enemy facing direction
    {
        this.transform.localScale = new Vector3(-1 * this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
    }
}
