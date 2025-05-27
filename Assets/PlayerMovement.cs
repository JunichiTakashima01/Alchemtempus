using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    public int facingDirection = 0; // right is 1, left is -1
    public float moveSpeed = 5f;

    public float jumpPower = 10f;
    public int maxJumps = 2;
    private int jumpRemaining;

    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public float horizontalMovement;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public float flashDistance = 5f;
    void Start()
    {
        jumpRemaining = 0;
    }
    // Update is called once per frame
    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        GroundCheck();
        CheckFacingDirection();
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
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
            if (rb.linearVelocity.y == 0)
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

    private void CheckFacingDirection()
    {
        if (rb.linearVelocity.x > 0)
        {
            facingDirection = 1;
        }
        else if (rb.linearVelocity.x < 0)
        {
            facingDirection = -1;
        }
        else
        {
            facingDirection = 0;
        }
    }
}
