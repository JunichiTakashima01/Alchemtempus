using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    public AttackCollisionBox attackCollisionBox;
    public GameController gameController;
    private PlayerMovement playerMovement;

    public float attackDmg = 3f;
    public float dashAttackDmg = 5f;
    public float knockBackDistance = 2f;
    public float attackMoveDistance = 0.2f;

    private Animator animator;

    private bool attacked = false;
    private bool dashAttacked = false;
    private bool isAttacking = false;
    private bool isDashAttacking = false;

    private bool canAttack = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        PauseButtonMouseDetect.OnMouseEnterUIStatus += SetCanAttack;
    }

    void OnDestroy()
    {
        PauseButtonMouseDetect.OnMouseEnterUIStatus -= SetCanAttack;
    }

    void Update()
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (attacked && !animatorStateInfo.IsName("AttackShort") && !animator.GetBool("attack"))
        {
            isAttacking = false;
            attacked = false;
        }

        if (dashAttacked && !animatorStateInfo.IsName("Dash-Attack-Short"))
        {
            isDashAttacking = false;
            dashAttacked = false;
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.started && canAttack && !gameController.getGamePausedStatus())
        {
            if (playerMovement.GetIsDashing())//dash attack
            {
                isDashAttacking = true;
                animator.SetBool("dashAttack", true);
                playerMovement.StopDashing();
                playerMovement.TurnOffGravity();
            }
            else if ((!isAttacking || attacked) && (!isDashAttacking || dashAttacked)) // either not dash attacking or after dash attack applied which means not in the middle of the dash attack
            { //normal attack
                isAttacking = true;
                animator.SetBool("attack", true);
            }
        }
    }


    public void OnAttackApply()
    {
        List<IEnemy> enemiesToAttack = attackCollisionBox.GetCollisionEnemies();
        if (enemiesToAttack != null)
        {
            List<IEnemy> copy = enemiesToAttack.ToList();
            foreach (Enemy enemy in copy)
            {
                enemy.TakeDamage(attackDmg, knockBackDistance);
            }
        }

        attacked = true;
        animator.SetBool("attack", false);

        //flash forward if player holding direction button
        if (playerMovement.GetHorizontalControllerValue() != 0)
        {
            Vector2 position = GetComponent<Transform>().position;
            GetComponent<Transform>().position = new Vector2(position.x + playerMovement.GetFacingDirection() * attackMoveDistance, position.y);
        }
    }

    public void OnDashAttackApply()
    {
        List<IEnemy> enemiesToAttack = attackCollisionBox.GetCollisionEnemies();
        if (enemiesToAttack != null)
        {
            List<IEnemy> copy = enemiesToAttack.ToList();
            foreach (Enemy enemy in copy)
            {
                enemy.TakeDamage(dashAttackDmg, knockBackDistance);
            }
        }

        dashAttacked = true;
        animator.SetBool("dashAttack", false);
    }

    private void SetCanAttack(bool mouseInPauseButton)
    {
        canAttack = !mouseInPauseButton;
    }

    public bool GetIsAttacking()
    {
        return isAttacking || isDashAttacking;
    }

    public void ResetAttackStatus()
    {
        isAttacking = false;
        attacked = false;
        animator.SetBool("attack", false);
        isDashAttacking = false;
        dashAttacked = false;
        animator.SetBool("dashAttack", false);
    }

    public void OnDashAttackFinish()
    {
        playerMovement.TurnOnGravity();
    }
}
