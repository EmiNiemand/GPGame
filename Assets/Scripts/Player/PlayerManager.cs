using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private PlayerAnimations playerAnimations;
        private GameUI gameUI;
        private PlayerCombat playerCombat;
        private PlayerSFX playerSfx;
        private PlayerStateMachine playerStateMachine;
        private PlayerMovement playerMovement;
        private PlayerCollisions playerCollisions;
        
        // Start is called before the first frame update
        void Start()
        {
            playerAnimations = GetComponentInChildren<PlayerAnimations>();
            playerSfx = GetComponent<PlayerSFX>();
            playerStateMachine = GetComponent<PlayerStateMachine>();
            playerMovement = GetComponent<PlayerMovement>();
            playerCollisions = GetComponentInChildren<PlayerCollisions>();
            playerCombat = GetComponent<PlayerCombat>();
            
            gameUI = FindObjectOfType<GameUI>();
            gameUI.Setup();
            gameUI.SetMaxHealth(playerCombat.maxHP);
        }
        
        #region Situation events
        public void OnDeath()
        {
            //make coroutine to play death animation or smth
            FindObjectOfType<GameManager>().respawnEvent.Invoke();
            gameUI.UpdateHealth(playerCombat.HP);
        }
        #endregion
        
        #region Movement events
        public void MovementFreeze(bool freeze = true)
        {
            playerMovement.BlockMovement(freeze);
        }

        public void OnLookingDirectionChange(int newDirection)
        {
            playerAnimations.UpdateLookingDirection(newDirection);
        }

        public float GetMoveSpeed()
        {
            return playerMovement.GetMoveSpeed();
        }
        #endregion
        
        #region CombatEvents
        public void AttackStart()
        {
            playerAnimations.AttackStart();
        }

        public void AttackEnd()
        {
            playerAnimations.AttackEnd();
        }
        #endregion
        
        #region Collision events
        public void Heal(int value)
        {
            if(!playerCombat.Heal(value)) return;
            
            gameUI.UpdateHealth(playerCombat.HP);
        }
        
        public void OnReceiveDamage()
        {
            gameUI.UpdateHealth(playerCombat.HP);
            playerAnimations.ShowReceiveDamage();
        }
        
        #endregion
        
        #region Player state events

        public void OnStateChange(PlayerStates newState)
        {
            playerAnimations.UpdateMovementTriggers(newState);
            playerCombat.UpdateState(newState);
            playerSfx.UpdateState(newState);
        }
        #endregion
        
        #region Animation events
        public void OnStep() { playerSfx.OnStep(); }

        public void AttackDamageStart(AttackType attackType)
        {
            playerCombat.AttackDamageStart(attackType);
            // play some sfx
        }

        public void AttackDamageEnd() { playerCombat.AttackDamageEnd(); }
        #endregion
        
        #region Input Events
        // Interaction events
        // ------------------
        public void OnUse(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                playerCollisions.OnUse();
            }
        }
        
        // Combat events
        // -------------
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!(context.started || context.canceled)) return;
            playerCombat.OnAttack(context.started);
        }

        // Movement events
        // ---------------
        public void OnMove(InputAction.CallbackContext context)
        {
            if(!playerMovement.Move(context.ReadValue<Vector2>())) return;
            
            playerAnimations.UpdateMovingDirection(playerMovement.direction.x);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!(context.started || context.canceled)) return;
            
            if(!playerMovement.Jump(context.started)) return;
            // do something if jump was successful
            //TODO: maybe move camera back a bit?
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (!(context.started || context.canceled)) return;
            playerMovement.Crouch(context.started);
            //TODO: maybe invoke event in camera
            //that after x seconds moves it down a bit
            //(Crouch() && context.started)
        }

        public void OnBoost(InputAction.CallbackContext context)
        {
            if(!context.started) return;
            playerMovement.Boost();
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            playerMovement.Dodge();
        }
        #endregion
    }  
} 
