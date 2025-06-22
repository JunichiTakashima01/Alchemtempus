using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D collider;
    private Vector3 playerSpawnTransformPosition = new Vector3(0, 0, 0);
    private Vector3 playerBaseScale = new Vector3(1, 1, 1);

    Animator anim;

    //If the game is paused
    private bool gamePaused = false;

    //Facing
    public float facingDirection = 1f; // right is 1, left is -1

    //Movement
    public float moveSpeed = 5f;
    public float horizontalMovement;
    public bool isMoving = false;

    //Jump
    public float jumpPower = 10f;
    public int maxJumps = 2;
    private int jumpRemaining = 0;

    //Gravity
    private float originalGravity = 1.8f;
    public float baseGravity = 1.8f;
    public float fallSpeedMultiplier = 1.8f;
    public float maxFallSpeed = 18f;

    //GroundCheck
    private bool isGrounded;
    // public Transform groundCheckPos;
    // public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    // public LayerMask groundLayer;

    //Flash
    public float flashDistance = 5f;

    //Dashing
    public float dashSpeed = 20f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 0.05f;
    private bool isDashing;
    private bool canDash = true;
    public int maxDashes = 2;
    private int DashRemaining = 0;
    private TrailRenderer trailRenderer;

    //Gem Effect
    private float growLargeDuration = 5f; //unit = second
    private Coroutine growLargeCoroutine = null;

    public static event Action OnDropping;
    public static event Action<bool> OnShieldingStatus;


    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        anim = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        collider = GetComponent<BoxCollider2D>();

        TripleJumpGem.OnTripleJumpCollected += ChangeMaxJumpsInTheAir;
        GrowLargeGem.OnGrowLargeCollected += ChangePlayerScale;
        GroundCheckCollider.OnTouchingGround += SetIsGrounded;
        GameController.OnGamePausedChangePauseStatus += SetGamePauseStatus;
    }

    void OnDestroy()
    {
        TripleJumpGem.OnTripleJumpCollected -= ChangeMaxJumpsInTheAir;
        GrowLargeGem.OnGrowLargeCollected -= ChangePlayerScale;
        GroundCheckCollider.OnTouchingGround -= SetIsGrounded;
        GameController.OnGamePausedChangePauseStatus -= SetGamePauseStatus;
    }

    private void ChangeMaxJumpsInTheAir(int maxJumpsInTheAir)
    {
        maxJumps = maxJumpsInTheAir;
    }

    private void ChangePlayerScale(float scaleAdditionner)
    {
        if (growLargeCoroutine == null)
        {
            growLargeCoroutine = StartCoroutine(GrowLargeCoroutine(scaleAdditionner));
        }
        else
        {
            StopCoroutine(growLargeCoroutine);
            growLargeCoroutine = StartCoroutine(GrowLargeCoroutine(scaleAdditionner * 0.1f)); //if already grown large, grow large by a tiny bit
        }
    }

    private IEnumerator GrowLargeCoroutine(float scaleAdditionner)
    {
        if (this.transform.localScale.x < 0)
        {
            this.transform.localScale = this.transform.localScale + new Vector3(-scaleAdditionner, scaleAdditionner, 0);
        }
        else
        {
            this.transform.localScale = this.transform.localScale + new Vector3(scaleAdditionner, scaleAdditionner, 0);
        }
        
        yield return new WaitForSeconds(growLargeDuration);

        this.transform.localScale = playerBaseScale;
        growLargeCoroutine = null;
    }

    private void SetIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePaused) return;
        //On game paused this script will be disabled and update method won't be called.
        anim.SetFloat("HorizontalVelocity", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("VerticalVelocity", Mathf.Abs(rb.linearVelocity.y));
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("ground", isGrounded);
        anim.SetBool("dash", isDashing);

        GroundCheck();
        Gravity();
        CheckDropIntoAbyss();

        if (isDashing)
        {
            return; //if dashing, stop other movements
        }

        Flip();

        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y); //control movement
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        if (horizontalMovement < 0)
        {
            isMoving = true;
        }
        else if (horizontalMovement > 0)
        {
            isMoving = true;
        }
        if (horizontalMovement == 0)
        {
            isMoving = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpRemaining > 0 && !gamePaused)
        {
            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
                jumpRemaining--;
                baseGravity = originalGravity;
            }
        }
    }

    public void Gravity() //will be called in update method
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
        if (facingDirection != 0 && !gamePaused)
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
        if (DashRemaining > 0 && context.performed && canDash && !gamePaused)
        {
            StartCoroutine(DashCoroutine());
            DashRemaining--;
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        trailRenderer.emitting = true;

        rb.linearVelocityX = facingDirection * dashSpeed;
        baseGravity = 0f;
        rb.linearVelocityY = 0f;


        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocityX = 0f;
        baseGravity = originalGravity;

        isDashing = false;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed && !gamePaused)
        {
            collider.isTrigger = true;
            OnDropping.Invoke();
        }
    }

    public void Parry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnShieldingStatus.Invoke(true);
        }
        else if (context.canceled)
        {
            OnShieldingStatus.Invoke(false);
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (!collider2D.IsTouching(collider) && collider2D.CompareTag("Ground"))
        {
            collider.isTrigger = false;
        }
    }

    private void GroundCheck() //will be called in update method
    {
        //isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);

        if (isGrounded)
        {
            if (rb.linearVelocity.y <= 0)
            {
                jumpRemaining = maxJumps;
                DashRemaining = maxDashes;
            }
        }
    }



    public void Flip() //will be called in update method
    {
        if (horizontalMovement < 0)
        {
            facingDirection = -1f;
        }
        else if (horizontalMovement > 0)
        {
            facingDirection = 1f;
        }

        if ((facingDirection == -1 && this.transform.localScale.x > 0) || (facingDirection == 1 && this.transform.localScale.x < 0))
        {
            Vector3 ls = this.transform.localScale;
            ls.x *= -1f;
            this.transform.localScale = ls;
        }
    }

    private void CheckDropIntoAbyss()//will be called in update method
    {
        if (this.transform.position.y < -20f)
        {
            TeleportToSpawn();
        }
    }

    public void TeleportToSpawn()
    {
        this.transform.position = playerSpawnTransformPosition;
    }

    private void SetGamePauseStatus(bool gamePaused)
    {
        this.gamePaused = gamePaused;
        // if (gamePaused)
        // {
        //     this.GetComponent<PlayerMovement>().enabled = false; //disable update and fixedupdate when game is paused
        // }
        // else
        // {
        //     this.GetComponent<PlayerMovement>().enabled = true; //enable update and fixedupdate when game is resumed   
        // }     
    }



    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    // }
}
