using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

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
        LedgeGrab,
        Boost,
        Dodge
    }

    public class PlayerMovement : MonoBehaviour
    {
        [HideInInspector] public Rigidbody2D rb2D;
        
        [HideInInspector] public Vector2 initColliderSize;
        
        [HideInInspector] public Vector2 previousPos;

        private PlayerStateMachine stateMachine;
        
        [Header("Move")]
        public float moveSpeed;
        public float crouchMoveSpeed;
        [HideInInspector] public Vector2 moveDirection;
        [HideInInspector] public Vector2 direction;
        [HideInInspector] public int lookingDirection = 1;
        [HideInInspector] public int previousLookingDirection;
        
        [Header("Jump")]
        public float jumpForce;
        public int maxJumpCount;
        [HideInInspector] public int jumpCount;
        [HideInInspector] public bool bIsJumping = false;
        
        [Header("Fall")]
        public float additionalFallingForce;
        [HideInInspector] public float initGravityScale;
        
        [Header("Dodge")]
        public float dodgeTime;
        public float dodgeForce;
        public float dodgeCooldown;
        [HideInInspector] public bool bCanDodge = true;
        
        [Header("Boost")]
        public float boostSpeed;
        public float boostTime;
        public float boostHeight;
        [HideInInspector] public bool bCanBoost = false;
        
        [Header("Wall Slide")]
        public float wallSlideTime;
        public float wallJumpTime;
        public float wallGravityScaleDivider;
        public float blockInputTimer;
        [HideInInspector] public bool bCanWallSlide = false;
        [HideInInspector] public bool bWallSlide = true;
        [HideInInspector] public bool bIsWallSlideOnCooldown = false;

        [Header("Ledge Grab And Climb")] 
        public float ledgeClimbSpeed;
        [HideInInspector] public bool bLedgeGrab = true;
        [HideInInspector] public bool bIsOnLedge = false;
        
        [Header("Crouch")]
        [HideInInspector] public bool bCanUncrouch = false;
        [HideInInspector] public bool bUncrouch = false;
        
        [Header("Other")]
        [HideInInspector] public bool bIsGrounded = false;
        [SerializeField] private bool bIsWallSlideActivated = false;
        [SerializeField] private bool bIsBoostActivated = false;
        [HideInInspector] public bool bBlockMovement = false;
        
        void Start()
        {
            stateMachine = gameObject.AddComponent<PlayerStateMachine>();
            rb2D = GetComponent<Rigidbody2D>();
            jumpCount = maxJumpCount;
            initGravityScale = rb2D.gravityScale;
            initColliderSize = GetComponentInChildren<CapsuleCollider2D>().size;
            previousPos = transform.position;
            previousLookingDirection = lookingDirection;
        }
        
        void Update()
        {
            if (moveDirection.x < 0 && !stateMachine.CheckCurrentState(PlayerStates.WallSlide) 
                                    && !stateMachine.CheckCurrentState(PlayerStates.LedgeGrab)) lookingDirection = -1;
            else if (moveDirection.x > 0 && !stateMachine.CheckCurrentState(PlayerStates.WallSlide) 
                                         && !stateMachine.CheckCurrentState(PlayerStates.LedgeGrab)) lookingDirection = 1;

            if (!stateMachine.CheckCurrentState(PlayerStates.Jump)) {
                bIsGrounded = Utils.ShootBoxcast(transform.position, new Vector2(initColliderSize.x / 2, 0.1f), 
                    Vector2.down, initColliderSize.y / 2 + 0.25f, "Environment");
                
            }
            
            if (bIsGrounded && !stateMachine.CheckCurrentState(PlayerStates.Crouch) && 
                !stateMachine.CheckCurrentState(PlayerStates.Boost) && !stateMachine.CheckCurrentState(PlayerStates.Jump) && 
                !stateMachine.CheckCurrentState(PlayerStates.Dodge))
            {
                if (moveDirection == Vector2.zero) stateMachine.SetCurrentState(PlayerStates.Idle);
                else if (moveDirection != Vector2.zero) stateMachine.SetCurrentState(PlayerStates.Move);
            }

            else if (bIsWallSlideActivated && !bIsGrounded && !bIsWallSlideOnCooldown && 
                     !stateMachine.CheckCurrentState(PlayerStates.WallSlide) && !stateMachine.CheckCurrentState(PlayerStates.LedgeGrab))
            {
                if (moveDirection.x < -0.5 || moveDirection.x > 0.5)
                {
                    bCanWallSlide = Utils.ShootBoxcast(transform.position, new Vector2(0.2f, initColliderSize.y / 4),
                        Vector2.right * lookingDirection, initColliderSize.x / 2 + 0.2f, "Environment");
                    
                    bIsOnLedge = !Utils.ShootRaycast(new Vector2(transform.position.x, transform.position.y + 
                            initColliderSize.y / 4), new Vector2(lookingDirection, 0), 
                        initColliderSize.y / 2 + 0.4f, "Environment");
                    
                    if (bCanWallSlide && (bWallSlide || bLedgeGrab))
                    {
                        if(bIsOnLedge) stateMachine.SetCurrentState(PlayerStates.LedgeGrab);
                        else stateMachine.SetCurrentState(PlayerStates.WallSlide);
                        return;
                    }
                }
            }
            
            if (rb2D.velocity.y < 0 && !bIsGrounded && !stateMachine.CheckCurrentState(PlayerStates.WallSlide) &&
                !stateMachine.CheckCurrentState(PlayerStates.Boost) && !stateMachine.CheckCurrentState(PlayerStates.Dodge) &&
                !stateMachine.CheckCurrentState(PlayerStates.Crouch) && !stateMachine.CheckCurrentState(PlayerStates.LedgeGrab))
            {
                stateMachine.SetCurrentState(PlayerStates.Fall);
            }
        }

        // Player inputs
        public void OnMove(InputAction.CallbackContext context)
        {
            if(bBlockMovement) return;
            moveDirection = new Vector2(context.ReadValue<Vector2>().x, 0);
            direction = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if(bBlockMovement) return;
            if (context.canceled) bIsJumping = false;
            if (!(context.started && jumpCount > 0)) return;
            bIsJumping = true;
            if (stateMachine.CheckCurrentState(PlayerStates.Jump)) return;
            if (!stateMachine.CheckCurrentState(PlayerStates.Crouch))
            {
                stateMachine.SetCurrentState(PlayerStates.Jump);
                return;
            }
            bCanUncrouch = !Utils.ShootBoxcast(transform.position, new Vector2(initColliderSize.x, 
                initColliderSize.y / 2), Vector2.up, initColliderSize.y / 2 + 0.1f, "Environment");
            if (bCanUncrouch) stateMachine.SetCurrentState(PlayerStates.Jump);
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if(bBlockMovement) return;
            if (context.started && bIsGrounded)
            {
                bUncrouch = false;
                stateMachine.SetCurrentState(PlayerStates.Crouch);
            }
            else if (context.canceled) bUncrouch = true;
        }

        public void OnBoost(InputAction.CallbackContext context)
        {
            if(bBlockMovement) return;
            if (!context.started || !bIsBoostActivated) return;

            bCanBoost = !Utils.ShootBoxcast(transform.position, new Vector2(initColliderSize.x - 0.2f, 
                initColliderSize.y - 0.2f), Vector2.up, boostHeight, "Environment");
            if (bCanBoost && bIsGrounded && !stateMachine.CheckCurrentState(PlayerStates.Boost))
            {
                stateMachine.SetCurrentState(PlayerStates.Boost);
            }
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if(bBlockMovement) return;
            if (context.started && bCanDodge && !stateMachine.CheckCurrentState(PlayerStates.Jump) &&
                !stateMachine.CheckCurrentState(PlayerStates.Fall) && !stateMachine.CheckCurrentState(PlayerStates.Crouch))
            {
                stateMachine.SetCurrentState(PlayerStates.Dodge);
            }
        }

        public PlayerStates GetPlayerState()
        {
            return stateMachine.GetPlayerState();
        }
        
        public bool CheckCurrentState(PlayerStates state)
        {
            return stateMachine.CheckCurrentState(state);
        }

        public float GetMoveSpeed()
        {
            return rb2D.velocity.x;
        }

        public void UnlockWallSlide()
        {
            bIsWallSlideActivated = true;
        }

        public void UnlockBoost()
        {
            bIsBoostActivated = true;
        }
    }
}

