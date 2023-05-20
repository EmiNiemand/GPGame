using System;
using System.Collections;
using System.Collections.Generic;
using Player.States;
using UnityEngine;

namespace Player
{
    public enum PlayerStates
    {
        Idle,
        Move,
        Crouch,
        Jump,
        Fall,
        WallSlide,
        Climb,
        LedgeClimb,
        Boost,
        Dodge
    }
    
    public class PlayerStateMachine : MonoBehaviour
    {
        private PlayerManager playerManager;
        private PlayerMovement playerMovement;

        private PlayerStates currentState;
        private PlayerStates previousState;

        private State activeState;

        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            playerMovement = GetComponent<PlayerMovement>();
            
            currentState = PlayerStates.Idle;
            previousState = currentState;
            activeState = new Idle(playerMovement);
        }

        private void FixedUpdate()
        {
            activeState.OnStayState();
        }

        #region Player State checkers and getters
        public PlayerStates GetPlayerState() { return currentState; }
        public bool CheckCurrentState(PlayerStates state) { return currentState == state; }
        public bool CheckPreviousState(PlayerStates state) { return previousState == state; }
        #endregion
        
        public void SetCurrentState(PlayerStates state)
        {
            if (state == currentState) return;
            previousState = currentState;
            currentState = state;
            
            activeState.OnExitState();
            activeState = CreateState(state);
            if (activeState == null)
            {
                throw new NullReferenceException();
            }
            activeState.OnEnterState();
            playerManager.OnStateChange(currentState);
            
        }
        
        State CreateState(PlayerStates state)
        {
            switch (state)
            {
                case PlayerStates.Idle:
                    return new Idle(playerMovement);
                
                case PlayerStates.Move:
                    return new Move(playerMovement);

                case PlayerStates.Crouch:
                    return new Crouch(playerMovement);
                
                case PlayerStates.Jump:
                    return new Jump(playerMovement);
                
                case PlayerStates.Fall:
                    return new Fall(playerMovement);
                
                case PlayerStates.Climb:
                    return new Climb(playerMovement);
                
                case PlayerStates.WallSlide:
                    return new WallSlide(playerMovement);
                
                case PlayerStates.LedgeClimb:
                    return new LedgeClimb(playerMovement);
                
                case PlayerStates.Boost:
                    return new Boost(playerMovement);
                
                case PlayerStates.Dodge:
                    return new Dodge(playerMovement);
            }

            return null;
        }
    }
}