using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAnimations : MonoBehaviour
    {
        private PlayerManager playerManager;
        private CapsuleCollider2D col2D;
        private Vector2 initColliderSize;
        private Vector3 initColliderOffset;
        private Animator playerAnimator;
        
        private PlayerStates currentState, previousState;
        private int lookingDirection;
        private float movingDirection;
        private float moveSpeed;
        
        // Start is called before the first frame update
        void Start()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            
            col2D = GetComponent<CapsuleCollider2D>();
            initColliderSize = col2D.size;
            initColliderOffset = col2D.offset;
            playerAnimator = GetComponent<Animator>();
            currentState = PlayerStates.Idle;
        }

        private float attackTimeCounter;
        //TODO: turn this into a dictionary
        private static readonly int WalkAttackHash = Animator.StringToHash("AttackWalk");
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int ReceiveDamageHash = Animator.StringToHash("ReceiveDamage");
        private static readonly int AttackLightHash = Animator.StringToHash("AttackLight");
        private static readonly int UpAttackHash = Animator.StringToHash("AttackUp");
        private static readonly int DownAttack = Animator.StringToHash("AttackDown");

        // Update is called once per frame
        void Update()
        {
            moveSpeed = playerManager.GetMoveSpeed();
            playerAnimator.SetFloat(SpeedHash, Mathf.Abs(moveSpeed/5));
        }

        public void AttackStart(Weapon.LookingDirection direction)
        {
            Debug.Log(currentState);
            
            switch (direction)
            {
                case Weapon.LookingDirection.Up when currentState is PlayerStates.Idle or PlayerStates.Move or PlayerStates.Jump or PlayerStates.Fall:
                    playerAnimator.SetTrigger(UpAttackHash); break;
                case Weapon.LookingDirection.Down when currentState is PlayerStates.Jump or PlayerStates.Fall:
                    playerAnimator.SetTrigger(DownAttack); break;
                case Weapon.LookingDirection.Left when currentState is PlayerStates.Idle or PlayerStates.Move or PlayerStates.Jump or PlayerStates.Fall:
                case Weapon.LookingDirection.Right when currentState is PlayerStates.Idle or PlayerStates.Move or PlayerStates.Jump or PlayerStates.Fall:
                    playerAnimator.SetTrigger(WalkAttackHash); break;
                default: break;
            }
            
            attackTimeCounter = Time.time;
        }

        public void AttackEnd()
        {
            var timeDelta = Time.time - attackTimeCounter;
            if(timeDelta < 0.2f || currentState is not (PlayerStates.Idle or PlayerStates.Move))
                playerAnimator.SetTrigger(AttackLightHash);
        }

        public void UpdateMovingDirection(float newMovingDirection)
        {
            movingDirection = newMovingDirection == 0 ? lookingDirection : newMovingDirection;
        }

        public void UpdateLookingDirection(int newLookingDirection)
        {
            lookingDirection = newLookingDirection;
            // Set sprite orientation
            transform.localScale = new Vector3(lookingDirection, 1, 1);
        }

        private void SetSpriteOrientation()
        {
            playerAnimator.SetBool(PlayerStates.WallSlide.ToString()+"_Reach", 
                lookingDirection != movingDirection && 
                currentState == PlayerStates.WallSlide);
        }

        public void UpdateMovementTriggers(PlayerStates newState)
        {
            previousState = currentState;
            currentState = newState;
            // Activate animator triggers 
            // --------------------------
            // Some triggers (Jump, Fall) stay activated for longer than action actually takes place
            playerAnimator.ResetTrigger(previousState.ToString());
            playerAnimator.SetTrigger(currentState.ToString());
            
            //TODO: improve this sheet
            // Workaround for movement freeze on heavy attack
            // ----------------------------------------------
            if (currentState is PlayerStates.Boost or PlayerStates.Dodge) { AE_MovementStart(); AE_AttackDamageEnd();}

            // Scale collider down when crouching
            // ----------------------------------
            if (currentState is PlayerStates.Crouch or PlayerStates.Dodge)
            {
                col2D.size = new Vector2(initColliderSize.x, initColliderSize.y / 2);
                col2D.offset = new Vector2(initColliderOffset.x, initColliderOffset.y - initColliderSize.y / 4);
            }
            else
            {
                col2D.size = initColliderSize;
                col2D.offset = initColliderOffset;
            }
        }

        public void ShowReceiveDamage() { playerAnimator.SetTrigger(ReceiveDamageHash); }

        #region Animation Events
        public void AE_MakeStep() { playerManager.OnStep(); }
        public void AE_AttackLightDamageStart() { playerManager.AttackDamageStart(AttackType.Light); }
        public void AE_AttackHeavyDamageStart() { playerManager.AttackDamageStart(AttackType.Heavy); }
        public void AE_AttackDamageEnd() { playerManager.AttackDamageEnd(); }
        
        public void AE_MovementStop() { playerManager.MovementFreeze(); }
        public void AE_MovementStart() { playerManager.MovementFreeze(false); }
        #endregion
    }
}

