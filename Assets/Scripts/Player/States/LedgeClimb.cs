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
            playerMovement.bBlockMovement = true;
        }

        public override void OnStayState()
        {
            bool bIsOverGround = !Utils.ShootRaycast(
                playerMovement.transform.position - new Vector3(0, playerMovement.initColliderSize.y * 0.5f, 0),
                Vector2.right * playerMovement.lookingDirection, playerMovement.initColliderSize.x, "Environment");

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
                    Vector2.down, playerMovement.initColliderSize.y, "Environment");
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
            playerMovement.bBlockMovement = false;
        }
    }
}

