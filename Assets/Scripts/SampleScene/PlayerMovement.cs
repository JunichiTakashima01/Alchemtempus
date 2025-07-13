using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Vector3 playerSpawnTransformPosition = new Vector3(0, 0, 0);
    private Vector3 playerBaseScale = new Vector3(1, 1, 1);
    public PlayerHealth playerHealth;
    private PlayerAttack playerAttack;

    Animator anim;

    //If the game is paused
    private bool gamePaused = false;

    //Facing
    private float facingDirection = 1f; // right is 1, left is -1

    //Movement
    public float moveSpeed = 5f;
    private float horizontalMovement;
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
    private bool isGrounded = false;
    private bool onPlatform = false;
    private bool onSolidGround = false;
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
    private Coroutine dashCoroutine = null;


    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        anim = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        boxCollider = this.GetComponent<BoxCollider2D>();
        playerAttack = GetComponent<PlayerAttack>();

        TripleJumpGem.OnTripleJumpCollected += ChangeMaxJumpsInTheAir;
        GrowLargeGem.OnGrowLargeCollected += ChangePlayerScale;
        GameController.OnGamePausedChangePauseStatus += SetGamePauseStatus;
    }

    void OnDestroy()
    {
        TripleJumpGem.OnTripleJumpCollected -= ChangeMaxJumpsInTheAir;
        GrowLargeGem.OnGrowLargeCollected -= ChangePlayerScale;
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

    // Update is called once per frame
    void Update()
    {
        if (gamePaused) return;
        //On game paused this script will be disabled and update method won't be called.
        anim.SetFloat("HorizontalVelocity", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("VerticalVelocity", rb.linearVelocity.y);
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

        MovementControl();
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    private void MovementControl()
    {
        if (isGrounded && playerAttack.GetIsAttacking())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y); //control movement

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
                anim.SetBool("isJump", true);
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
            dashCoroutine = StartCoroutine(DashCoroutine());
            DashRemaining--;

            GetComponent<PlayerAttack>().ResetAttackStatus();
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        trailRenderer.emitting = true;

        //In the first 20% of the time the dash speed is 20%
        rb.linearVelocityX = facingDirection * dashSpeed * 0.2f;
        yield return new WaitForSeconds(dashDuration * 0.2f);

        rb.linearVelocityX = facingDirection * dashSpeed;
        baseGravity = 0f;
        rb.linearVelocityY = 0f;

        yield return new WaitForSeconds(dashDuration * 0.8f);

        rb.linearVelocityX = 0f;
        baseGravity = originalGravity;

        isDashing = false;
        trailRenderer.emitting = false;

        dashCoroutine = null;

        StartCoroutine(DashCoolDown());
    }

    private IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void StopDashing()
    {
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
            rb.linearVelocityX = 0f;
            baseGravity = originalGravity;

            isDashing = false;
            trailRenderer.emitting = false;

            StartCoroutine(DashCoolDown());
        }
    }

    public void Drop(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed && !gamePaused && onPlatform && !onSolidGround)
        {
            Physics2D.IgnoreLayerCollision(6, 8, true); //Ignore collision between player and platform
        }
    }

    public void EnableCollisionWithPlatforms()
    {
        Physics2D.IgnoreLayerCollision(6, 8, false); //enable collision between player and platform
    }

    public void Parry(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            this.GetComponent<PlayerMana>().StartShield();
        }
        else if (context.canceled)
        {
            this.GetComponent<PlayerMana>().StopShield();
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
                anim.SetBool("isJump", false);
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
        if (this.transform.position.y < -100f)
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

    public void SetIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }

    public void SetOnPlatform(bool onPlatform)
    {
        this.onPlatform = onPlatform;
    }

    public void SetOnSolidGround(bool onSolidGround)
    {
        this.onSolidGround = onSolidGround;
    }

    public bool GetIsDashing()
    {
        return isDashing;
    }

    public float GetHorizontalControllerValue()
    {
        return horizontalMovement;
    }

    public float GetFacingDirection()
    {
        return facingDirection;
    }



    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.white;
    //     Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    // }
}
