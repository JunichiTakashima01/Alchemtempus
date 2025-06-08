using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player; //for this enemy object to chase
    public float chaseSpeed;
    public float jumpForce;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool shouldJump;
    private bool isJumping;
    private float LengthFromCenterToBottom;
    private float LengthFromCenterToSide;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();

        LengthFromCenterToBottom = this.GetComponent<BoxCollider2D>().size.y / 2f;
        LengthFromCenterToSide =  this.GetComponent<BoxCollider2D>().size.x / 2f;
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
        float direction = Mathf.Sign(player.position.x - this.transform.position.x);

        //Player Above?
        bool isPlayerAbove = Physics2D.Raycast(this.transform.position, Vector2.up, LengthFromCenterToBottom + 3f, 1 << player.gameObject.layer); // 1 << player.gameObject.layer provides the layermask of player object

        if (isGrounded)
        {
            //Chase Player
            rb.linearVelocityX = direction * chaseSpeed;

            //Jump if there is a gap ahead and no ground in front
            //else if there is plaver above and platform above

            //If ground in front (blocking enemy from moving forward)
            RaycastHit2D groundInFront = Physics2D.Raycast(this.transform.position, new Vector2(direction, 0), LengthFromCenterToSide + 0.2f, groundLayer);
            //If gap ahead
            RaycastHit2D groundAhead = Physics2D.Raycast(this.transform.position + new Vector3(direction * LengthFromCenterToSide, 0, 0), Vector2.down, LengthFromCenterToBottom + 0.05f, groundLayer);
            //If platform above
            RaycastHit2D platformAbove = Physics2D.Raycast(this.transform.position, Vector2.up, LengthFromCenterToBottom + 2f, groundLayer);

            if (!groundInFront && !groundAhead) //Indicate a gap ahead
            {
                shouldJump = true;
            }
            else if (isPlayerAbove && platformAbove)//Player at the platform above
            {
                shouldJump = true;
            }
            bool a = groundInFront;
            Debug.Log("Front " + a);
            a = groundAhead;
            Debug.Log("Ahead "+a);
            a = platformAbove;
            Debug.Log("Plat "+a);
        }

        //Decide whether to jump
        if (isGrounded && shouldJump)
        {
            shouldJump = false;
            Vector2 jumpDirection = jumpForce * (player.position - this.transform.position).normalized;

            //isJumping = true;
            //rb.AddForce(new Vector2(jumpDirection.x, jumpForce), ForceMode2D.Impulse);
            rb.linearVelocityY = jumpForce;
        }
    }
}
