using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class Fall : State
    {
        public Fall(PlayerMovement playerMovement) : base(playerMovement) {}

        public override void OnEnterState() {}

        public override void OnStayState()
        {
            //TODO: Implement pushing player on the ledge if he's stuck
            playerMovement.rb2D.AddForce(Vector2.down * playerMovement.additionalFallingForce, ForceMode2D.Impulse);
            
            if (playerMovement.bBlockMovement) return;
            playerMovement.rb2D.AddForce(playerMovement.moveDirection * playerMovement.moveSpeed, ForceMode2D.Impulse);
        }

        public override void OnExitState() {}
    }
}

