using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    Animator anim;

    //Facing
    public float facingDirection = 1f; // right is 1, left is -1

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

    //Dashing
    public float dashSpeed = 20f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 0.5f;
    private bool isDashing;
    private bool canDash = true;
    private TrailRenderer trailRenderer;

    //Animator parameter
    private bool isGrounded;


    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        jumpRemaining = 0;
        anim = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
    }
    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("speed", Mathf.Abs(rb.linearVelocity.x));

        GroundCheck();

        if (isDashing)
        {
            return; //if dashing, stop other movements
        }

        Flip();
        Gravity();

        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        if (horizontalMovement < 0)
        {
            facingDirection = -1f;
        }
        else if (horizontalMovement > 0)
        {
            facingDirection = 1f;
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

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        trailRenderer.emitting = true;

        rb.linearVelocityX = facingDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocityX = 0f;

        isDashing = false;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        anim.SetBool("ground", isGrounded);

        if (isGrounded)
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
