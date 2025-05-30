using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    Animator anim;

    //Facing
    public int facingDirection = 1; // right is 1, left is -1

    //Movement
    public float moveSpeed = 5f;
    public float horizontalMovement;

    //Jump
    public float jumpPower = 10f;
    public int maxJumps = 2;
    private int jumpRemaining;

    //Gravity
    public float baseGravity = 2f;
    public float fallSpeedMultiplier = 2f;
    public float maxFallSpeed = 18f;

    //GroundCheck
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool isOnGround = false;

    //Flash
    public float flashDistance = 5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        jumpRemaining = 0;
        anim = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        anim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));
        GroundCheck();
        Flip();
        Gravity();
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        if (horizontalMovement < 0)
        {
            facingDirection = -1;
        }
        else if (horizontalMovement > 0)
        {
            facingDirection = 1;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpRemaining > 0)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpRemaining--;
            }
        }
    }

    public void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocityY = Mathf.Max(rb.linearVelocityY, -maxFallSpeed);
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void Flash(InputAction.CallbackContext context)
    {
        if (facingDirection != 0)
        {
            if (context.performed)
            {
                Vector3 deltaPos = new Vector3(facingDirection * flashDistance, 0f);
                this.transform.position += deltaPos;
            }
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            if (rb.linearVelocity.y < 0)
            {
                jumpRemaining = maxJumps;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    public void Flip()
    {
        if ((facingDirection == -1 && this.transform.localScale.x > 0) || (facingDirection == 1 && this.transform.localScale.x < 0))
        {
            Vector3 ls = this.transform.localScale;
            ls.x *= -1f;
            this.transform.localScale = ls;
        }
    }
}
