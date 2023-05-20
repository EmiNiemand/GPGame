using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class Climb : State
    {
        private const float climbSpeed = 1.0f;
        public Climb(PlayerMovement playerMovement) : base(playerMovement) {}
        
        public override void OnEnterState()
        {
            playerMovement.jumpCount = playerMovement.maxJumpCount;
            playerMovement.rb2D.gravityScale = 0;
            playerMovement.rb2D.velocity = Vector2.zero;
        }

        public override void OnStayState()
        {
            if (playerMovement.bBlockMovement) return;
            playerMovement.rb2D.AddForce(playerMovement.moveDirection * playerMovement.moveSpeed, ForceMode2D.Impulse);
            playerMovement.rb2D.AddForce(Vector2.up * (playerMovement.direction.y * climbSpeed), ForceMode2D.Impulse);
        }

        public override void OnExitState()
        {
            playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
        }
    }
}

