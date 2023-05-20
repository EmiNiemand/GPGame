using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class WallSlide : State
    {
        private float gravityScale;
        private float wallSlideTimer;
        
        public WallSlide(PlayerMovement playerMovement) : base(playerMovement) {}

        public override void OnEnterState()
        {
            wallSlideTimer = playerMovement.wallSlideTime;
            playerMovement.jumpCount = playerMovement.maxJumpCount;

            var playerPosition = playerMovement.transform.position;
            
            playerMovement.bIsOnLedge = !Utils.ShootRaycast(new Vector2(playerPosition.x, 
                     playerPosition.y + playerMovement.initColliderSize.y / 4), new Vector2(playerMovement.lookingDirection, 1), 
                playerMovement.initColliderSize.y / 2 + 0.5f, (int)Layers.Environment);
            
            if (playerMovement.bIsOnLedge)
            {
                playerMovement.rb2D.velocity = new Vector2(playerMovement.rb2D.velocity.x, 0);
                playerMovement.rb2D.gravityScale = 0;
            }
            else
            {
                gravityScale = playerMovement.rb2D.gravityScale / playerMovement.wallGravityScaleDivider;

                if (playerMovement.rb2D.velocity.y < 0)
                {
                    playerMovement.rb2D.velocity = new Vector2(playerMovement.rb2D.velocity.x, 0);
                    playerMovement.rb2D.gravityScale = gravityScale;
                }
            }
            
        }

        public override void OnStayState()
        {
            if (playerMovement.rb2D.velocity.y < 0)
            {
                playerMovement.rb2D.gravityScale = gravityScale;
            }
            
            wallSlideTimer -= Time.fixedDeltaTime;
            if (wallSlideTimer <= 0)
            {
                playerMovement.bCanWallSlide = false;
            }
            
            // Player slided to the ground from wall or cancelled WallSlide
            if (!playerMovement.bCanWallSlide || playerMovement.direction.y <= -0.5 || playerMovement.bIsGrounded) 
                playerMovement.SetCurrentState(PlayerStates.Fall);
            
            playerMovement.rb2D.AddForce(Vector2.right * playerMovement.lookingDirection, ForceMode2D.Impulse);

            var colliderHalfWidth = playerMovement.initColliderSize.x * 0.5f;
            var colliderHalfHeight = playerMovement.initColliderSize.y * 0.5f;
            
            // Check whether player still collides with wall
            playerMovement.bCanWallSlide = Utils.ShootBoxcast(playerMovement.transform.position + 
                 new Vector3(0, colliderHalfHeight * 0.5f, 0), new Vector2(colliderHalfWidth, colliderHalfHeight * 0.5f), 
                 Vector2.right * playerMovement.lookingDirection, colliderHalfWidth + 0.3f, (int)Layers.Environment) && 
                                           Utils.ShootBoxcast(playerMovement.transform.position - 
                 new Vector3(0, colliderHalfHeight * 0.5f, 0), new Vector2(colliderHalfWidth, colliderHalfHeight* 0.5f), 
                 Vector2.right * playerMovement.lookingDirection, colliderHalfWidth + 0.3f, (int)Layers.Environment);
        }

        public override void OnExitState()
        {
            playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
            
            playerMovement.StartCoroutine(playerMovement.WallSlideCooldown());
        }
    }
}

