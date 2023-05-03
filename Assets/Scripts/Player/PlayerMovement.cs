using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [HideInInspector] public Rigidbody2D rb2D;
        [HideInInspector] public Vector2 initColliderSize;

        public PlayerStateMachine stateMachine;
        private PlayerManager playerManager;

        [Header("Move")] public float moveSpeed;
        public float crouchMoveSpeed;
        [HideInInspector] public Vector2 moveDirection;
        [HideInInspector] public Vector2 direction;
        private int lookingDirectionValue = 1;

        [HideInInspector]
        public int lookingDirection
        {
            get => lookingDirectionValue;
            set
            {
                lookingDirectionValue = value;
                playerManager.OnLookingDirectionChange(lookingDirectionValue);
            }
        }
        
        [Header("Jump")] public float jumpForce;
        public int maxJumpCount;
        public float normalJumpTime;
        [HideInInspector] public int jumpCount;
        [HideInInspector] public bool bIsJumping = false;

        [Header("Fall")] public float additionalFallingForce;
        [HideInInspector] public float initGravityScale;

        [Header("Dodge")] public float dodgeSpeed;
        public float dodgeLength;
        public float dodgeCooldown;
        [HideInInspector] public bool bIsDodging = false;
        [HideInInspector] public bool bIsDodgeOnCooldown = false;

        [Header("Boost")] public float boostSpeed;
        public float boostTime;
        public float boostHeight;
        [HideInInspector] public bool bCanBoost = false;

        [Header("Wall Slide")] public float wallSlideTime;
        public float wallGravityScaleDivider;
        public float wallSlideCooldown;
        [HideInInspector] public bool bCanWallSlide = false;
        [HideInInspector] public bool bIsWallSlideOnCooldown = false;

        [Header("Ledge Grab And Climb")] public float ledgeClimbSpeed;
        [HideInInspector] public bool bIsOnLedge = false;

        [Header("Crouch")] [HideInInspector] public bool bCanUncrouch = false;
        [HideInInspector] public bool bUncrouch = false;

        [Header("Other")] [HideInInspector] public bool bIsGrounded = false;
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
        }

        void Update()
        {
            if (moveDirection.x < 0 && !CheckCurrentState(PlayerStates.WallSlide)
                                    && !CheckCurrentState(PlayerStates.LedgeClimb)) lookingDirection = -1;
            else if (moveDirection.x > 0 && !CheckCurrentState(PlayerStates.WallSlide)
                                         && !CheckCurrentState(PlayerStates.LedgeClimb)) lookingDirection = 1;

            bIsGrounded = Utils.ShootBoxcast(transform.position, new Vector2(initColliderSize.x / 2, 0.1f),
                Vector2.down, initColliderSize.y / 2 + 0.25f, "Environment");

            if (bIsGrounded && !CheckCurrentState(PlayerStates.Crouch) &&
                !CheckCurrentState(PlayerStates.Boost) && !CheckCurrentState(PlayerStates.Jump) &&
                !CheckCurrentState(PlayerStates.Dodge) && !CheckCurrentState(PlayerStates.LedgeClimb))
            {
                if (moveDirection == Vector2.zero) SetCurrentState(PlayerStates.Idle);
                else if (moveDirection != Vector2.zero) SetCurrentState(PlayerStates.Move);
            }

            else if (bIsWallSlideActivated && !bIsGrounded && !bIsWallSlideOnCooldown &&
                     !CheckCurrentState(PlayerStates.WallSlide) && !CheckCurrentState(PlayerStates.LedgeClimb))
            {
                if (moveDirection.x < -0.5 || moveDirection.x > 0.5)
                {
                    var colliderHalfWidth = initColliderSize.x * 0.5f;
                    var colliderHalfHeight = initColliderSize.y * 0.5f;

                    bCanWallSlide = Utils.ShootBoxcast(
                                        transform.position + new Vector3(0, colliderHalfHeight * 0.5f, 0),
                                        new Vector2(colliderHalfWidth, colliderHalfHeight * 0.5f),
                                        Vector2.right * lookingDirection,
                                        colliderHalfWidth + 0.1f, "Environment") &&
                                    Utils.ShootBoxcast(
                                        transform.position - new Vector3(0, colliderHalfHeight * 0.5f, 0),
                                        new Vector2(colliderHalfWidth, colliderHalfHeight * 0.5f),
                                        Vector2.right * lookingDirection,
                                        colliderHalfWidth + 0.1f, "Environment");

                    if (bCanWallSlide)
                    {
                        stateMachine.SetCurrentState(PlayerStates.WallSlide);
                        return;
                    }
                }
            }

            if (rb2D.velocity.y < 0 && !bIsGrounded && !stateMachine.CheckCurrentState(PlayerStates.WallSlide) &&
                !stateMachine.CheckCurrentState(PlayerStates.Boost) &&
                !stateMachine.CheckCurrentState(PlayerStates.Dodge) &&
                !stateMachine.CheckCurrentState(PlayerStates.Crouch) &&
                !stateMachine.CheckCurrentState(PlayerStates.LedgeClimb))
            {
                stateMachine.SetCurrentState(PlayerStates.Fall);
            }
        }

        // Player input handlers
        // return true if action performed successfully
        public bool Move(Vector2 inputVector)
        {
            if (bBlockMovement) return false;

            moveDirection = new Vector2(inputVector.x, 0);
            direction = inputVector;
            return true;
        }

        public bool Jump(bool actionStarted)
        {
            if (bBlockMovement) return false;

            if (!actionStarted) bIsJumping = false;
            if (!(actionStarted && jumpCount > 0)) return false;

            bIsJumping = true;

            if (stateMachine.CheckCurrentState(PlayerStates.Jump)) return false;

            if (stateMachine.CheckCurrentState(PlayerStates.Crouch))
            {
                // Player can go directly from crouching to jumping
                // if he has enough room above him
                var colliderHalfWidth = initColliderSize.x / 2;
                var colliderHalfHeight = initColliderSize.y / 2;

                bCanUncrouch = !Utils.ShootBoxcast(transform.position, new Vector2(colliderHalfWidth,
                    colliderHalfHeight), Vector2.up, colliderHalfHeight / 2, "Environment");

                if (!bCanUncrouch) return false;
                stateMachine.SetCurrentState(PlayerStates.Jump);
                return true;
            }

            if (stateMachine.CheckCurrentState(PlayerStates.WallSlide) && bIsOnLedge)
            {
                int sign = moveDirection.x>0 ? 1:-1;
                if (sign * (int)Mathf.Ceil(Mathf.Abs(moveDirection.x)) != lookingDirection && moveDirection.x != 0)
                {
                    stateMachine.SetCurrentState(PlayerStates.Jump);
                    return true;
                }
                stateMachine.SetCurrentState(PlayerStates.LedgeClimb);
                return true;
            }

            stateMachine.SetCurrentState(PlayerStates.Jump);
            return true;
        }

        public bool Crouch(bool actionStarted)
        {
            if (bBlockMovement) return false;

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
            if (bBlockMovement) return false;
            if (!bIsBoostActivated) return false;
            if (!bIsGrounded) return false;
            if (CheckCurrentState(PlayerStates.Boost)) return false;

            var colliderHalfWidth = initColliderSize.x / 2;
            var colliderHalfHeight = initColliderSize.y / 2;

            // Check if there's enough place upwards to boost
            bCanBoost = !Utils.ShootBoxcast(transform.position, new Vector2(colliderHalfWidth,
                colliderHalfHeight), Vector2.up, boostHeight, "Environment");

            if (bCanBoost)
            {
                stateMachine.SetCurrentState(PlayerStates.Boost);
            }

            return true;
        }

        public bool Dodge()
        {
            if (bBlockMovement) return false;

            if (bIsDodging || bIsDodgeOnCooldown || stateMachine.CheckCurrentState(PlayerStates.Jump) ||
                stateMachine.CheckCurrentState(PlayerStates.Fall) ||
                stateMachine.CheckCurrentState(PlayerStates.Crouch)) return false;

            stateMachine.SetCurrentState(PlayerStates.Dodge);
            return true;
        }

        public void SetCurrentState(PlayerStates state)
        {
            stateMachine.SetCurrentState(state);
        }

        public bool CheckCurrentState(PlayerStates state)
        {
            return stateMachine.CheckCurrentState(state);
        }

        public bool CheckPreviousState(PlayerStates state)
        {
            return stateMachine.CheckPreviousState(state);
        }

        public float GetMoveSpeed()
        {
            return rb2D.velocity.x;
        }

        public void BlockMovement(bool block = true)
        {
            // if (bBlockMovement == block) return;
            // bBlockMovement = block;
            rb2D.constraints = block ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.FreezeRotation;
        }

        public void UnlockWallSlide()
        {
            bIsWallSlideActivated = true;
        }

        public void UnlockBoost()
        {
            bIsBoostActivated = true;
        }

        // TODO: find better method to do this
        public IEnumerator DodgeCooldown()
        {
            bIsDodgeOnCooldown = true;
            yield return new WaitForSeconds(dodgeCooldown);
            bIsDodgeOnCooldown = false;
        }
        
        public IEnumerator WallSlideCooldown()
        {
            bIsWallSlideOnCooldown = true;
            yield return new WaitForSeconds(wallSlideCooldown);
            bIsWallSlideOnCooldown = false;
        }
    }
}

