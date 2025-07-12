using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
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
            Debug.Log(animatorStateInfo.normalizedTime);
            
        }
    }

    public void Attack()
    {
        isAttacking = true;
        attack = true;
        animator.SetBool("attack", true);
    }

    private IEnumerator AttackMotion()
    {
        yield return new WaitForSeconds(0);
    }
}
