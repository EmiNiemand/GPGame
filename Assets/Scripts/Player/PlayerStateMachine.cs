using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private PlayerStates currentState;
        private PlayerStates previousState;
        private PlayerMovement playerMovement;
        private PlayerManager playerManager;
        
        private float jumpTime = 0.1f;
        private float jumpTimeCounter;
        //TODO: bNormalJump might be leftover variable from testing various types of jump; might be fine to delete
        private bool bNormalJump = false;
        
        private float dodgeDirection = 1;
        
        private bool bCanMove = true;
        private bool bCanWallJump = false;

        private bool bBlockState = false;

        // Start is called before the first frame update
        void Start()
        {
            jumpTimeCounter = jumpTime;
            currentState = PlayerStates.Idle;
            previousState = currentState;
            playerMovement = GetComponent<PlayerMovement>();
            playerManager = GetComponent<PlayerManager>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            OnStayState();
        }
        
          // Player states methods
        public void SetCurrentState(PlayerStates state)
        {
            if (bBlockState) return;
            previousState = currentState;
            currentState = state;
            if (currentState != previousState)
            {
                OnExitState(previousState);
                OnEnterState(currentState);
                playerManager.OnStateChange(currentState);
            }
        }
        
        #region Player State checkers and getters
        public PlayerStates GetPlayerState() { return currentState; }
        public bool CheckCurrentState(PlayerStates state) { return currentState == state; }
        public bool CheckPreviousState(PlayerStates state) { return previousState == state; }
        
        #endregion

        #region Entering, Staying in and Exiting State
        private void OnEnterState(PlayerStates state)
        {
            switch (state)
            {
                case PlayerStates.Idle: case PlayerStates.Move:
                    playerMovement.jumpCount = playerMovement.maxJumpCount;
                    break;
                case PlayerStates.Jump:
                    playerMovement.bIsGrounded = false;
                    playerMovement.rb2D.velocity = new Vector2(playerMovement.rb2D.velocity.x, 0);
                    if (bCanWallJump && !playerMovement.bIsGrounded)
                    {
                        StartCoroutine(WallSlideCooldown());
                        StartCoroutine(BlockInput());
                        playerMovement.rb2D.AddForce(new Vector2(-playerMovement.lookingDirection * 0.75f, 1.25f) * 
                                                     playerMovement.jumpForce, ForceMode2D.Impulse);
                        playerMovement.lookingDirection *= (-1);
                    }
                    else if (previousState == PlayerStates.LedgeGrab)
                    {
                        // When moveDirection is on the opposite site of lookingDirection
                        if (playerMovement.moveDirection.x * playerMovement.lookingDirection < 0)
                        {
                            StartCoroutine(WallSlideCooldown());
                            StartCoroutine(BlockInput());
                            playerMovement.rb2D.AddForce(new Vector2(-playerMovement.lookingDirection * 0.75f, 1.25f) * 
                                                         playerMovement.jumpForce, ForceMode2D.Impulse);
                            playerMovement.lookingDirection *= (-1);
                        }
                        else
                        {
                            StartCoroutine(LedgeClimb());
                        }
                    }
                    else if (previousState == PlayerStates.Boost)
                    {
                        playerMovement.rb2D.AddForce(new Vector2(playerMovement.moveDirection.x / 2, 1.25f) * 
                                                     playerMovement.jumpForce, ForceMode2D.Impulse);
                    }
                    else
                    {
                        bNormalJump = true;
                        playerMovement.bWallSlide = false;
                        playerMovement.bLedgeGrab = false;
                        playerMovement.rb2D.AddForce(Vector2.up * playerMovement.jumpForce / 2, ForceMode2D.Impulse);
                    }
                    playerMovement.jumpCount--;
                    break;
                case PlayerStates.Dodge:
                    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (var enemy in enemies)
                    {
                        Physics2D.IgnoreCollision(GetComponentInChildren<CapsuleCollider2D>(), 
                            enemy.GetComponent<Collider2D>(), true);
                    }
                    dodgeDirection = playerMovement.lookingDirection;
                    if (CheckPreviousState(PlayerStates.WallSlide))
                    {
                        dodgeDirection *= -1;
                        playerMovement.lookingDirection *= -1;
                    }
                    StartCoroutine(Dodge());
                    break;
                case PlayerStates.Boost:
                    playerMovement.rb2D.velocity = Vector2.zero;
                    StartCoroutine(Boost());
                    break;
                case PlayerStates.WallSlide:
                    playerMovement.jumpCount = playerMovement.maxJumpCount;
                    playerMovement.rb2D.gravityScale /= playerMovement.wallGravityScaleDivider;
                    bCanWallJump = true;
                    if(playerMovement.rb2D.velocity.y < 0) playerMovement.rb2D.velocity = new Vector2(playerMovement.rb2D.velocity.x, 0);
                    StartCoroutine(PushPlayerOfTheWall());
                    break;
                case PlayerStates.LedgeGrab:
                    playerMovement.jumpCount = playerMovement.maxJumpCount;
                    playerMovement.rb2D.gravityScale = 0;
                    playerMovement.rb2D.velocity = Vector2.zero;
                    bCanMove = false;
                    break;
                default: break;
            }
        }

        private void OnStayState()
        {
            // Add force to Player during horizontal movement and Dodge
            // --------------------------------------------------------
            if (currentState != PlayerStates.WallSlide && currentState != PlayerStates.Boost && currentState != PlayerStates.Crouch && bCanMove)
            {
                // Dodge "locks" Player's direction until the end of state
                if (currentState == PlayerStates.Dodge)
                {
                    playerMovement.rb2D.AddForce(new Vector2(dodgeDirection, 0) * playerMovement.moveSpeed, ForceMode2D.Impulse);
                }
                else
                {
                    playerMovement.rb2D.AddForce(playerMovement.moveDirection * playerMovement.moveSpeed, ForceMode2D.Impulse);
                }
            }

            switch (currentState)
            {
                case PlayerStates.Move:
                    // Push Player onto platform if he's stuck at the edge
                    // ---------------------------------------------------
                    if (playerMovement.previousLookingDirection == playerMovement.lookingDirection && 
                        playerMovement.previousPos == new Vector2(transform.position.x, transform.position.y) && 
                        !Utils.ShootBoxcast(playerMovement.transform.position, 
                            new Vector2(0.1f, playerMovement.initColliderSize.y / 2), 
                            Vector2.right * playerMovement.lookingDirection, 
                            playerMovement.initColliderSize.x / 2 + 0.2f, "Environment"))
                    {
                        playerMovement.rb2D.AddForce(Vector2.up * 2.5f, ForceMode2D.Impulse);
                    }
                    
                    playerMovement.previousPos = playerMovement.transform.position;
                    playerMovement.previousLookingDirection = playerMovement.lookingDirection;
                    break;
                case PlayerStates.Fall:
                    // Push Player onto platform if he's stuck at the edge
                    // ---------------------------------------------------
                    if (playerMovement.rb2D.velocity.y == 0)
                    {
                        playerMovement.rb2D.AddForce(Vector2.up * 2.5f, ForceMode2D.Impulse);
                    }
                    // Speed up player's fall to make it feel more snappy
                    // --------------------------------------------------
                    else
                    {
                        playerMovement.rb2D.AddForce(Vector2.down * playerMovement.additionalFallingForce, ForceMode2D.Impulse);
                    }
                    break;
                case PlayerStates.Jump:
                    if (bNormalJump)
                    {
                        if (jumpTimeCounter > 0 && playerMovement.bIsJumping)
                        {
                            playerMovement.rb2D.AddForce(Vector2.up * playerMovement.jumpForce / 10, ForceMode2D.Impulse);
                            jumpTimeCounter -= Time.deltaTime;
                        }
                    }
                    if (playerMovement.rb2D.velocity.y <= 0) SetCurrentState(PlayerStates.Fall);
                    break;
                case PlayerStates.Crouch:
                    playerMovement.rb2D.AddForce(playerMovement.moveDirection * playerMovement.crouchMoveSpeed, ForceMode2D.Impulse);
                    if (playerMovement.bUncrouch)
                    {
                        playerMovement.bCanUncrouch = !Utils.ShootBoxcast(playerMovement.transform.position, 
                            new Vector2(playerMovement.initColliderSize.x, playerMovement.initColliderSize.y / 2), 
                            Vector2.up, playerMovement.initColliderSize.y / 2, "Environment");
                        if (playerMovement.bCanUncrouch)
                        {
                            SetCurrentState(PlayerStates.Idle);
                        }
                    }
                    break;
                case PlayerStates.Boost:
                    if (playerMovement.direction.y <= -0.5) SetCurrentState(PlayerStates.Fall);
                    break;
                case PlayerStates.WallSlide:
                    //TODO: check if player can LedgeGrab
                    // while WallSliding player can go a little bit up,
                    // which may make it possible to reach a ledge
                    
                    // Player slided to the ground from wall or cancelled WallSlide
                    if (!playerMovement.bCanWallSlide || playerMovement.direction.y <= -0.5 || playerMovement.bIsGrounded) SetCurrentState(PlayerStates.Fall);
                    // Check whether player still collides with wall
                    playerMovement.bCanWallSlide = Utils.ShootBoxcast(playerMovement.transform.position, 
                        new Vector2(0.2f, playerMovement.initColliderSize.y / 4), Vector2.right * playerMovement.lookingDirection, 
                        playerMovement.initColliderSize.x / 2 + 0.2f, "Environment");
                    break;
                case PlayerStates.LedgeGrab:
                    // Player cancelled ledge grab
                    if(playerMovement.direction.y <= -0.5f) SetCurrentState(PlayerStates.Fall);
                    break;
                default: break;
            }
        }

        private void OnExitState(PlayerStates state)
        {
            switch (state)
            {
                case PlayerStates.Boost:
                    playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
                    break;
                case PlayerStates.WallSlide:
                    StartCoroutine(WallJumpTimer());
                    playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
                    break;
                case PlayerStates.Jump:
                    jumpTimeCounter = jumpTime;
                    bNormalJump = false;
                    playerMovement.bWallSlide = true;
                    playerMovement.bLedgeGrab = true;
                    break;
                case PlayerStates.Dodge:
                    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (var enemy in enemies)
                    {
                        Physics2D.IgnoreCollision(GetComponentInChildren<CapsuleCollider2D>(), 
                            enemy.GetComponent<Collider2D>(), false);
                    }
                    break;
                case PlayerStates.LedgeGrab:
                    playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
                    bCanMove = true;
                    break;
                default: break;
            }
        }
        #endregion

        #region States coroutines
         private IEnumerator Dodge()
        {
            playerMovement.bCanDodge = false;
            PlayerCombat combat = GetComponent<PlayerCombat>();
            combat.SetPlayerVulnerable(false);
            playerMovement.rb2D.velocity = new Vector2(dodgeDirection * playerMovement.dodgeForce, playerMovement.rb2D.velocity.y);

            // Slow down Player's fall during dashing from boost 
            if (previousState is PlayerStates.Boost) playerMovement.rb2D.gravityScale *= 0.5f; 
            yield return new WaitForSeconds(playerMovement.dodgeTime);
            if (previousState is PlayerStates.Boost) playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
            
            combat.SetPlayerVulnerable(true);
            
            // Check clearance above Player and whether he collides with wall
            // --------------------------------------------------------------
            playerMovement.bCanUncrouch = !Utils.ShootBoxcast(playerMovement.transform.position, 
                new Vector2(playerMovement.initColliderSize.x - 0.1f, playerMovement.initColliderSize.y / 2), 
                Vector2.up, playerMovement.initColliderSize.y / 2 + 0.1f, "Environment");
            playerMovement.bCanWallSlide = Utils.ShootBoxcast(playerMovement.transform.position, 
                new Vector2(0.2f, playerMovement.initColliderSize.y / 4), 
                Vector2.right * playerMovement.lookingDirection, playerMovement.initColliderSize.x / 2 + 0.2f, "Environment");
            // Force Player to crouch if there's not enough space above him
            // ------------------------------------------------------------
            if (!playerMovement.bCanUncrouch)
            {
                playerMovement.bUncrouch = true;
                SetCurrentState(PlayerStates.Crouch);
            }
            // From Dodge player can either Boost, WallSlide, Walk or Fall
            // -----------------------------------------------------------
            else if (currentState == PlayerStates.Boost) SetCurrentState(PlayerStates.Boost);
            else if (playerMovement.bIsGrounded == false && playerMovement.bCanWallSlide) SetCurrentState(PlayerStates.WallSlide);
            else if (playerMovement.bCanUncrouch && playerMovement.bIsGrounded)
                SetCurrentState(playerMovement.moveDirection == Vector2.zero ? PlayerStates.Idle : PlayerStates.Move);
            else SetCurrentState(PlayerStates.Fall);
            yield return new WaitForSeconds(playerMovement.dodgeCooldown);
            playerMovement.bCanDodge = true;
        }

        private IEnumerator Boost()
        {
            // Move player to configured height
            // --------------------------------
            var destination = (Vector2)playerMovement.transform.position + new Vector2(0, playerMovement.boostHeight);
            if (currentState == PlayerStates.Boost) playerMovement.rb2D.gravityScale = 0;
            
            yield return new WaitUntil(() => IsOnPosition(destination, PlayerStates.Boost, playerMovement.boostSpeed, 0.5f));
            yield return StartCoroutine(Wait(playerMovement.boostTime, PlayerStates.Boost));
            
            // Make Player fall if he's been boosting for too long
            // ---------------------------------------------------
            if (currentState == PlayerStates.Boost) SetCurrentState(PlayerStates.Fall);
        }

        private IEnumerator LedgeClimb()
        {
            playerMovement.bBlockMovement = true;
            bBlockState = true;
            
            // Block physics simulation during LedgeClimb to prevent bugs
            playerMovement.rb2D.constraints = RigidbodyConstraints2D.FreezeAll;
            
            // Move Player up
            // --------------
            var destination = (Vector2)playerMovement.transform.position + 
                              new Vector2(0, playerMovement.initColliderSize.y);
            yield return new WaitUntil(() => IsOnPosition(destination, PlayerStates.Jump, playerMovement.ledgeClimbSpeed, 0.1f));
            
            // Move Player to the side, above platform
            // ---------------------------------------
            destination = (Vector2)playerMovement.transform.position + 
                          new Vector2(playerMovement.initColliderSize.x * playerMovement.lookingDirection, 0);
            yield return new WaitUntil(() => IsOnPosition(destination, PlayerStates.Jump, playerMovement.ledgeClimbSpeed, 0.1f));

            // Restore physics simulation
            playerMovement.rb2D.constraints = RigidbodyConstraints2D.FreezeRotation;

            playerMovement.bBlockMovement = false;
            bBlockState = false;
        }
        
        #endregion
        
        #region Other coroutines
        private IEnumerator WallSlideCooldown()
        {
            playerMovement.bIsWallSlideOnCooldown = true;
            yield return new WaitForSeconds(0.25f);
            playerMovement.bIsWallSlideOnCooldown = false;
        }

        private IEnumerator PushPlayerOfTheWall()
        {
            yield return StartCoroutine(Wait(playerMovement.wallSlideTime, PlayerStates.WallSlide));
            StartCoroutine(WallSlideCooldown());
            playerMovement.bCanWallSlide = false;
        }

        private IEnumerator WallJumpTimer()
        {
            yield return new WaitForSeconds(playerMovement.wallJumpTime);
            bCanWallJump = false;
        }
        
        private IEnumerator BlockInput()
        {
            bCanMove = false;
            yield return new WaitForSeconds(playerMovement.blockInputTimer);
            bCanMove = true;
        }
        #endregion
        
        #region Coroutines helper methods
        private bool IsOnPosition(Vector2 destination, PlayerStates state, float speed, float precision)
        {
            var position = playerMovement.transform.position;
            
            transform.position = Vector2.MoveTowards(position, destination, speed);
            return Vector2.Distance(position, destination) < precision || currentState != state;
        }
        private IEnumerator Wait(float time, PlayerStates state)
        {
            float timePassed = 0;

            while (timePassed < time)
            {
                if (currentState != state) break;
                timePassed += Time.deltaTime;
                yield return null;
            }
        }
        #endregion
    }
}