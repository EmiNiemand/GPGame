using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class Boost : State
    {
        private float boostTimer = 0;
        private float hoverTimer = 0;
        public Boost(PlayerMovement playerMovement) : base(playerMovement) {}

        public override void OnEnterState()
        {
            if (playerMovement.CheckPreviousState(PlayerStates.Dodge))
            {
                playerMovement.rb2D.AddForce(new Vector2(playerMovement.lookingDirection * playerMovement.dodgeLength / playerMovement.dodgeSpeed, 
                    playerMovement.boostHeight / playerMovement.boostSpeed) * 3.5f, ForceMode2D.Impulse);
                playerMovement.SetCurrentState(PlayerStates.Fall);
                return;
            }
            boostTimer = playerMovement.boostTime;
            hoverTimer = playerMovement.boostSpeed;
            playerMovement.rb2D.velocity = Vector2.zero;
            playerMovement.rb2D.gravityScale = 0;
        }

        public override void OnStayState()
        {
            if (playerMovement.CheckPreviousState(PlayerStates.Dodge)) return;
            boostTimer -= Time.fixedDeltaTime;
            hoverTimer -= Time.fixedDeltaTime;
            if (playerMovement.direction.y <= -0.5 || boostTimer <= 0) playerMovement.SetCurrentState(PlayerStates.Fall);
            if (hoverTimer <= 0)
            {
                playerMovement.rb2D.velocity = Vector2.zero;
                return;
            }
            playerMovement.rb2D.AddForce(Vector2.up * (playerMovement.boostHeight / playerMovement.boostSpeed), ForceMode2D.Impulse);
        }

        public override void OnExitState()
        {
            playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
        }
    }
}

