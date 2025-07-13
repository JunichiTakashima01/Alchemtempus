using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public AttackCollisionBox attackCollisionBox;

    public float attackDmg = 3;
    public float knockBackDistance = 2;

    private Animator animator;

    private bool isAttacking = false;
    private bool attack = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (attack && animatorStateInfo.IsName("AttackShort"))
        {
            if (animatorStateInfo.normalizedTime < 0.5) // to allow buffer input 
            {
                attack = false;
                animator.SetBool("attack", false);
            }
        }
        else if (isAttacking && !animatorStateInfo.IsName("AttackShort"))
        {
            isAttacking = false;
        }
    }

    public void Attack()
    {
        isAttacking = true;
        attack = true;
        animator.SetBool("attack", true);
    }

    public bool GetIsAttacking()
    {
        return isAttacking;
    }

    private IEnumerator AttackMotion()
    {
        yield return new WaitForSeconds(0);
    }

    public void OnAttackApplied()
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
    }
}
