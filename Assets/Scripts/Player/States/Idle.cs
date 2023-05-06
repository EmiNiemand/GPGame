using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class Idle : State
    {
        public Idle(PlayerMovement playerMovement) : base(playerMovement) {}

        public override void OnEnterState()
        {
            playerMovement.jumpCount = playerMovement.maxJumpCount;
        }

        public override void OnStayState() {}

        public override void OnExitState() {}
    }
}

