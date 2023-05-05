using System;
using System.Collections;
using System.ComponentModel;
using UI;
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
    
    public enum MovementModifier { None, Slow, Stop, BoostUp, BoostDown, BoostForward }

    public class PlayerMovement : MonoBehaviour
    {
        //TODO: when player slides up on the wall too high up, he might leave WallSlide and Fall (and doesn't check if he can grab ledge)
        

        [HideInInspector] public Rigidbody2D rb2D;
        
        [HideInInspector] public Vector2 initColliderSize;
        
        [HideInInspector] public Vector2 previousPos;

        private PlayerStateMachine stateMachine;
        private PlayerManager playerManager;
        
        [Header("Move")]
        public float moveSpeed;
        public float crouchMoveSpeed;
        [HideInInspector] public Vector2 moveDirection;
        [HideInInspector] public Vector2 direction;
        private int lookingDirectionValue = 1;
        [HideInInspector] public int lookingDirection
        {
            get => lookingDirectionValue;
            set
            {
                lookingDirectionValue = value;
                playerManager.OnLookingDirectionChange(lookingDirectionValue);
            }
        }
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
            playerManager = GetComponent<PlayerManager>();
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

        // Player input handlers
        // return true if action performed successfully
        public bool Move(Vector2 inputVector)
        {
            if(bBlockMovement) return false;
            
            moveDirection = new Vector2(inputVector.x, 0);
            direction = inputVector;
            return true;
        }

        public bool Jump(bool actionStarted)
        {
            if(bBlockMovement) return false;
            
            if (!actionStarted) bIsJumping = false;
            if (!(actionStarted && jumpCount > 0)) return false;
            
            bIsJumping = true;
            
            if (stateMachine.CheckCurrentState(PlayerStates.Jump)) return false;
            if (!stateMachine.CheckCurrentState(PlayerStates.Crouch))
            {
                stateMachine.SetCurrentState(PlayerStates.Jump);
                return true;
            }
            // Player can go directly from crouching to jumping
            // if he has enough room above him
            bCanUncrouch = !Utils.ShootBoxcast(transform.position, new Vector2(initColliderSize.x, 
                initColliderSize.y / 2), Vector2.up, initColliderSize.y / 2 + 0.1f, "Environment");
            
            if (!bCanUncrouch) return false;
            stateMachine.SetCurrentState(PlayerStates.Jump);
            return true;
        }

        public bool Crouch(bool actionStarted)
        {
            if(bBlockMovement) return false;
            
            if (actionStarted && bIsGrounded)
            {
                bUncrouch = false;
                stateMachine.SetCurrentState(PlayerStates.Crouch);
            }
            else if (!actionStarted) bUncrouch = true;

            return true;
        }

        public bool Boost()
        {
            if(bBlockMovement) return false;
            if (!bIsBoostActivated) return false;
            
            // Check if there's enough place upwards to boost
            bCanBoost = !Utils.ShootBoxcast(transform.position, new Vector2(initColliderSize.x - 0.2f, 
                initColliderSize.y - 0.2f), Vector2.up, boostHeight, "Environment");
            if (bCanBoost && bIsGrounded && !stateMachine.CheckCurrentState(PlayerStates.Boost))
            {
                stateMachine.SetCurrentState(PlayerStates.Boost);
            }

            return true;
        }

        public bool Dodge()
        {
            if(bBlockMovement) return false;
            if (!bCanDodge || stateMachine.CheckCurrentState(PlayerStates.Jump) ||
                stateMachine.CheckCurrentState(PlayerStates.Fall) ||
                stateMachine.CheckCurrentState(PlayerStates.Crouch)) return false;
            
            stateMachine.SetCurrentState(PlayerStates.Dodge);
            return true;
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

        public void ActivateModifier(MovementModifier modifier, float time=0)
        {
            switch (modifier)
            {
                case MovementModifier.Slow:
                    StartCoroutine(ModifierSlow(time));
                    break;
                case MovementModifier.Stop:
                    StartCoroutine(ModifierStop(time));
                    break;
                case MovementModifier.BoostUp:
                    rb2D.velocity *= new Vector2(1, 0);
                    rb2D.AddForce(new Vector2(0, 50), ForceMode2D.Impulse);
                    break;
                case MovementModifier.BoostForward:
                    rb2D.velocity *= new Vector2(0, 1);
                    rb2D.AddForce(new Vector2(20*lookingDirection, 0), ForceMode2D.Impulse);
                    bCanDodge = true;
                    break;
                case MovementModifier.BoostDown:
                    rb2D.velocity *= new Vector2(1, 0);
                    rb2D.AddForce(new Vector2(0, -10), ForceMode2D.Impulse);
                    break; 
                case MovementModifier.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(modifier), modifier, null);
            }
        }

        private IEnumerator ModifierSlow(float time)
        {
            var initSpeed = moveSpeed;
            moveSpeed /= 2;
            yield return new WaitForSeconds(time);
            moveSpeed = initSpeed;
        }
        
        private IEnumerator ModifierStop(float time)
        {
            var initConstrains = rb2D.constraints;
            rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            yield return new WaitForSeconds(time);
            rb2D.constraints = initConstrains;
        }

        public void UnlockWallSlide() { bIsWallSlideActivated = true; }

        public void UnlockBoost() { bIsBoostActivated = true; }
    }
}

