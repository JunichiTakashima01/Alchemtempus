using System.Collections;
using UnityEngine;

public class BasicEnemy : Enemy, IEnemy
{
    protected override void Update()
    {
        base.Update();
        // IsGrounded();
        // CheckFacingDirection();
        // if (PlayerDetected())
        // {
        //     Move();
        //     DecideJump();
        // }
        // if (collidingPlayer)
        // {
        //     DoDMGToPlayer(damage);
        // }
    }
}