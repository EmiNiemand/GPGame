using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class Crouch : State
    {
        public Crouch(PlayerMovement playerMovement) : base(playerMovement) {}

        public override void OnEnterState() {}

        public override void OnStayState()
        {
            if (playerMovement.bUncrouch)
            {
                var colliderHalfWidth = playerMovement.initColliderSize.x / 2;
                var colliderHalfHeight = playerMovement.initColliderSize.y / 2;
                
                playerMovement.bCanUncrouch = !Utils.ShootBoxcast(playerMovement.transform.position, 
                    new Vector2(colliderHalfWidth, colliderHalfHeight),  Vector2.up, colliderHalfHeight / 2, "Environment");
                
                if (playerMovement.bCanUncrouch)
                {
                    playerMovement.SetCurrentState(PlayerStates.Idle);
                }
            }
            
            if (playerMovement.bBlockMovement) return;
            playerMovement.rb2D.AddForce(playerMovement.moveDirection * playerMovement.crouchMoveSpeed, ForceMode2D.Impulse);
        }

        public override void OnExitState() {}
    }
}

