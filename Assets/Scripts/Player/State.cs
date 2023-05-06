using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public abstract class State
    {
        protected readonly PlayerMovement playerMovement;

        protected State(PlayerMovement playerMovement)
        {
            this.playerMovement = playerMovement;
        }
        
        public abstract void OnEnterState();
        public abstract void OnStayState();
        public abstract void OnExitState();
    }
}

