using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class Move : State
    {
        public Move(PlayerMovement playerMovement) : base(playerMovement) {}

        public override void OnEnterState()
        {
            playerMovement.jumpCount = playerMovement.maxJumpCount;
        }

        public override void OnStayState()
        {
            //TODO: Implement pushing player on the platform if he's stuck on the ledge

            if (playerMovement.bBlockMovement) return;
            playerMovement.rb2D.AddForce(playerMovement.moveDirection * playerMovement.moveSpeed, ForceMode2D.Impulse);
        }

        public override void OnExitState() {}
    }
}

