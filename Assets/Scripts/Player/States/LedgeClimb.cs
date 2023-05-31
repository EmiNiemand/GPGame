using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class LedgeClimb : State
    {
        private bool bPreviousOverGroundValue = false;
        public LedgeClimb(PlayerMovement playerMovement) : base(playerMovement) {}

        public override void OnEnterState()
        {
            playerMovement.rb2D.gravityScale = 0;
        }

        public override void OnStayState()
        {
            bool bIsOverGround = !Utils.ShootBoxcast(playerMovement.transform.position, playerMovement.initColliderSize,
                Vector2.right * playerMovement.lookingDirection, playerMovement.initColliderSize.x, (int)Layers.Environment);

            if (!bIsOverGround)
            {
                playerMovement.rb2D.AddForce(Vector2.up / playerMovement.ledgeClimbSpeed, ForceMode2D.Impulse);
            }

            if (bPreviousOverGroundValue)
            {
                playerMovement.rb2D.velocity = new Vector2(playerMovement.rb2D.velocity.x, 0);    
            }
            
            if (bIsOverGround)
            {
                playerMovement.rb2D.AddForce(Vector2.right * (playerMovement.lookingDirection / playerMovement.ledgeClimbSpeed), ForceMode2D.Impulse);
                bool bIsOnLedge = Utils.ShootRaycast(
                    playerMovement.transform.position + new Vector3(playerMovement.initColliderSize.x * -playerMovement.lookingDirection * 0.5f, 0, 0),
                    Vector2.down, playerMovement.initColliderSize.y * 1.5f, (int)Layers.Environment);
                if (bIsOnLedge)
                {
                    playerMovement.rb2D.velocity = new Vector2(0, 0);  
                    playerMovement.SetCurrentState(PlayerStates.Fall);
                }
            }

            bPreviousOverGroundValue = bIsOverGround;
        }

        public override void OnExitState()
        {
            playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
        }
    }
}

