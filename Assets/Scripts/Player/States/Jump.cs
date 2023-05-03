using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Player.States
{
    public class Jump : State
    {
        private float jumpTimeCounter;
        private bool bNormalJump;

        public Jump(PlayerMovement playerMovement) : base(playerMovement)
        {
            jumpTimeCounter = playerMovement.normalJumpTime;
        }

        public override void OnEnterState()
        {
            playerMovement.jumpCount--;
            playerMovement.rb2D.velocity = new Vector2(playerMovement.rb2D.velocity.x, 0);
            
            if (playerMovement.CheckPreviousState(PlayerStates.WallSlide))
            {
                // TODO: check values
                playerMovement.rb2D.AddForce(new Vector2(-playerMovement.lookingDirection * 0.75f, 1.25f) * playerMovement.jumpForce, ForceMode2D.Impulse);
                playerMovement.lookingDirection *= (-1);
            }
            else if (playerMovement.CheckPreviousState(PlayerStates.Boost))
            {
                // TODO: check values
                playerMovement.rb2D.AddForce(new Vector2(playerMovement.moveDirection.x * 0.5f, 1.25f) * 
                                             playerMovement.jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                playerMovement.rb2D.AddForce(Vector2.up * (playerMovement.jumpForce / 2), ForceMode2D.Impulse);
                bNormalJump = true;
            }
        }

        public override void OnStayState()
        {
            if (bNormalJump)
            {
                if (jumpTimeCounter >= 0 && playerMovement.bIsJumping)
                {
                    playerMovement.rb2D.AddForce(Vector2.up * (playerMovement.jumpForce / 2 * Time.fixedDeltaTime / 
                                                               playerMovement.normalJumpTime), ForceMode2D.Impulse);
                    jumpTimeCounter -= Time.fixedDeltaTime;
                }
            }
            
            if (playerMovement.rb2D.velocity.y <= 0)
            {
                playerMovement.SetCurrentState(PlayerStates.Fall);
                return;
            }
            
            if (!playerMovement.bBlockMovement)
            {
                playerMovement.rb2D.AddForce(playerMovement.moveDirection * playerMovement.moveSpeed, ForceMode2D.Impulse);
            }
        }

        public override void OnExitState() {}
    }
}

