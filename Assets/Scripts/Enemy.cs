using System.Collections;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Transform player; //for this enemy object to chase
    public float chaseSpeed;
    public float jumpForce;
    public LayerMask groundLayer;

    public float damage = 1;

    private float enemyDetectingSight = 9;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;
    //private bool isJumping;
    private float LengthFromCenterToBottom;
    private float LengthFromCenterToSide;
    private float direction;

    private float maxHealth = 5;
    private float currHealth;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
    void Update()
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

        //Player Direction?
        direction = Mathf.Sign(player.position.x - this.transform.position.x);

        //Player Above?
        //bool isPlayerAbove = Physics2D.Raycast(this.transform.position, Vector2.up, LengthFromCenterToBottom + 3f, 1 << player.gameObject.layer); // 1 << player.gameObject.layer provides the layermask of player object
        bool isPlayerAbove = player.position.y > (this.transform.position.y + LengthFromCenterToBottom);
        bool isPlayerBelow = player.position.y < (this.transform.position.y - LengthFromCenterToBottom);

        if (enemyDetectingSight > Vector3.Distance(player.position, this.transform.position))// if too far the enemy can't see player thus no movement.
        {
            rb.linearVelocityX = direction * chaseSpeed;
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
    }

    public void takeDamage(float dmg, float knockBackDistance = 0f)
    {
        currHealth -= dmg;
        StartCoroutine(FlashWhite());
        if (currHealth <= 0)
        {
            DestroyEnemy();
        }
        this.transform.position -= new Vector3(direction * knockBackDistance, 0, 0);
    }

    private IEnumerator FlashWhite()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor;
    }

    private void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }
}
