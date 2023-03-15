using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerManager : MonoBehaviour
    {
        private PlayerAnimations animations;
        private PlayerCombat combat;
        private PlayerSFX sfx;
        private PlayerStateMachine stateMachine;
        private PlayerMovement movement;
        private PlayerCollisions collisions;
        private PlayerCameraEffects cameraEffects;
        
        private GamepadHaptics gamepadHaptics;
        private GameUI gameUI;
        private PauseMenuUI pauseMenuUI;
        
        // Start is called before the first frame update
        void Start()
        {
            animations = GetComponentInChildren<PlayerAnimations>();
            sfx = GetComponent<PlayerSFX>();
            stateMachine = GetComponent<PlayerStateMachine>();
            movement = GetComponent<PlayerMovement>();
            collisions = GetComponentInChildren<PlayerCollisions>();
            combat = GetComponent<PlayerCombat>();
            cameraEffects = GetComponent<PlayerCameraEffects>();
            
            gamepadHaptics = GetComponent<GamepadHaptics>();
            gamepadHaptics.SetPlayerWidth(
                collisions.gameObject.GetComponent<Collider2D>()
                    .bounds.size.x);

            gameUI = FindObjectOfType<GameUI>();
            gameUI.Setup();
            gameUI.SetMaxHealth(combat.maxHP);

            pauseMenuUI = GameObject.Find("_PauseMenuManager").GetComponent<PauseMenuUI>();
        }
        
        #region Situation Events
        public void OnDeath()
        {
            //make coroutine to play death animation or smth
            FindObjectOfType<GameManager>().respawnEvent.Invoke();
            gameUI.UpdateHealth(combat.HP);
        }
        #endregion
        
        #region Movement Events
        public void MovementFreeze(bool freeze = true)
        {
            movement.BlockMovement(freeze);
        }

        public void OnLookingDirectionChange(int newDirection)
        {
            animations.UpdateLookingDirection(newDirection);
            combat.UpdateLookingDirection(newDirection);
        }

        public float GetMoveSpeed()
        {
            return movement.GetMoveSpeed();
        }
        #endregion
        
        #region Combat Events
        public void AttackStart()
        {
            animations.AttackStart();
        }

        public void AttackEnd()
        {
            animations.AttackEnd();
        }
        #endregion
        
        #region Collision Events
        public void Heal(int value)
        {
            if(!combat.Heal(value)) return;
            
            gameUI.UpdateHealth(combat.HP);
        }
        
        public void OnReceiveDamage(Vector2 sourcePoint)
        {
            gameUI.UpdateHealth(combat.HP);
            gamepadHaptics.ReceivedDamage(sourcePoint);
            animations.ShowReceiveDamage();
            sfx.PlayCombatSound(CombatSoundType.Hurt);
            cameraEffects.PlayEffect(CameraSingularEffect.Shake);
        }
        
        public void OnWeaponHit(Vector2 hitPosition)
        {
            gamepadHaptics.SuccessfullyAttacked(hitPosition);
            sfx.PlayCombatSound(CombatSoundType.Hit);
        }
        #endregion
        
        #region Player State Events

        public void OnStateChange(PlayerStates newState)
        {
            animations.UpdateMovementTriggers(newState);
            combat.UpdateState(newState);
            sfx.UpdateState(newState);
        }
        #endregion
        
        #region Animation Events
        public void OnStep() { sfx.OnStep(); }

        public void AttackDamageStart(AttackType attackType)
        {
            combat.AttackDamageStart(attackType);
            //TODO: this probably won't be accurate, but let's try
            sfx.PlayCombatSound(CombatSoundType.Swing);
        }

        public void AttackDamageEnd() { combat.AttackDamageEnd(); }
        #endregion
        
        #region Input Events
        // Interaction events
        // ------------------
        public void OnUse(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            collisions.OnUse();
        }
        
        // Combat events
        // -------------
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!(context.started || context.canceled)) return;
            combat.OnAttack(context.started);
        }
        
        // UI events
        // ---------
        public void OnPauseUnpauseGame(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            if(pauseMenuUI.PauseUnpause())
                gamepadHaptics.Pause();
            else
                gamepadHaptics.Resume();
        }

        // Movement events
        // ---------------
        public void OnMove(InputAction.CallbackContext context)
        {
            //TODO
            if(!movement.Move(context.ReadValue<Vector2>())) return;
            
            animations.UpdateMovingDirection(movement.direction.x);
            combat.UpdateMovingDirection(movement.direction);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!(context.started || context.canceled)) return;
            
            if(!movement.Jump(context.started)) return;
            // do something if jump was successful
            //TODO: maybe move camera back a bit?
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (!(context.started || context.canceled)) return;
            movement.Crouch(context.started);
            //TODO: maybe invoke event in camera
            //that after x seconds moves it down a bit
            //(Crouch() && context.started)
        }

        public void OnBoost(InputAction.CallbackContext context)
        {
            if(!context.started) return;
            movement.Boost();
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            movement.Dodge();
        }
        #endregion
    }  
} 
