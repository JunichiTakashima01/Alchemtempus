using System.Collections;
using UnityEngine;

public class BasicEnemy : Enemy
{
    protected override void Update()
    {
        IsGrounded();
        CheckFacingDirection();
        if (PlayerDetected())
        {
            Move();
            DecideJump();
        }
    }
}