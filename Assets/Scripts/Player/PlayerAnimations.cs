using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAnimations : MonoBehaviour
    {
        private PlayerMovement playerMovement;
        private PlayerCombat playerCombat;
        private PlayerSFX playerSFX;
        private CapsuleCollider2D col2D;
        private Vector2 initColliderSize;
        private Vector3 initColliderOffset;
        private Animator playerAnimator;
        private PlayerStates currentState, previousState;
        
        // Start is called before the first frame update
        void Start()
        {
            playerMovement = transform.parent.gameObject.GetComponent<PlayerMovement>();
            playerCombat = transform.parent.gameObject.GetComponent<PlayerCombat>();
            playerSFX = transform.parent.gameObject.GetComponent<PlayerSFX>();
            
            col2D = GetComponent<CapsuleCollider2D>();
            initColliderSize = col2D.size;
            initColliderOffset = col2D.offset;
            playerAnimator = GetComponent<Animator>();
            currentState = PlayerStates.Idle;
        }

        // Update is called once per frame
        float attackTimeCounter = 0;
        void Update()
        {
            previousState = currentState;
            currentState = playerMovement.GetPlayerState();

            // Set sprite orientation
            // ----------------------
            var lookingDirection = playerMovement.lookingDirection;
            var movingDirection = playerMovement.direction.x == 0 ? lookingDirection : playerMovement.direction.x;
            transform.localScale = new Vector3(lookingDirection, 1, 1);

            // Attack animations handling
            // --------------------------
            if(playerCombat.GetIsAttacking() && attackTimeCounter < 0.4f)
            {
                if(attackTimeCounter == 0)
                    playerAnimator.SetTrigger("Attack");
                attackTimeCounter += Time.deltaTime;
            }
            else if (attackTimeCounter != 0)
            {
                if(attackTimeCounter < 0.2f)
                    playerAnimator.SetTrigger("AttackLight");
                
                if(!playerCombat.GetIsAttacking())
                    attackTimeCounter = 0;
            }


            // Movement animations handling
            // ----------------------------
            var playerSpeed = playerMovement.GetMoveSpeed();

            playerAnimator.SetBool(PlayerStates.WallSlide.ToString()+"_Reach", 
                                    lookingDirection != movingDirection && 
                                    currentState == PlayerStates.WallSlide);

            // Activate animator triggers 
            // --------------------------
            // Some triggers (Jump, Fall) stay activated for longer than action actually takes place
            if(previousState != currentState)
                playerAnimator.ResetTrigger(previousState.ToString());
            playerAnimator.SetTrigger(currentState.ToString());
            playerAnimator.SetFloat("Speed", Mathf.Abs(playerSpeed/5));
            if (playerMovement.CheckCurrentState(PlayerStates.Crouch) || 
                playerMovement.CheckCurrentState(PlayerStates.Dodge))
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

        public void ShowReceiveDamage() {
            playerAnimator.SetTrigger("ReceiveDamage");
        }

        #region Animation Events
        public void AE_MakeStep() { playerSFX.OnStep(); }
        public void AE_AttackLightDamageStart() { playerCombat.AttackLightDamageStart(); }
        public void AE_AttackHeavyDamageStart() { playerCombat.AttackHeavyDamageStart(); }
        public void AE_AttackDamageEnd() { playerCombat.AttackDamageEnd(); }

        //TODO: definitely improve
        public void AE_MovementStop() { playerMovement.rb2D.constraints = RigidbodyConstraints2D.FreezeAll; }
        public void AE_MovementStart() { playerMovement.rb2D.constraints = RigidbodyConstraints2D.FreezeRotation; }
        #endregion
    }
}

