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
        
        private float jumpTime = 0.1f;
        private float jumpTimeCounter;
        private bool bNormalJump = false;
        
        private float dodgeDirection = 1;
        
        private bool bCanMove = true;
        private bool bCanWallJump = false;
        

        // Start is called before the first frame update
        void Start()
        {
            jumpTimeCounter = jumpTime;
            currentState = PlayerStates.Idle;
            previousState = currentState;
            playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            OnStayState();
        }
        
          // Player states methods
        public void SetCurrentState(PlayerStates state)
        {
            previousState = currentState;
            currentState = state;
            if (currentState != previousState)
            {
                OnExitState(previousState);
                OnEnterState(currentState);
            }
        }
        public PlayerStates GetPlayerState()
        {
            return currentState;
        }

        public bool CheckCurrentState(PlayerStates state)
        {
            return currentState == state;
        }

        public bool CheckPreviousState(PlayerStates state)
        {
            return previousState == state;
        }

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
                    else if (previousState == PlayerStates.Boost)
                    {
                        playerMovement.rb2D.AddForce(new Vector2(playerMovement.moveDirection.x / 2, 1.25f) * 
                                                     playerMovement.jumpForce, ForceMode2D.Impulse);
                    }
                    else
                    {
                        bNormalJump = true;
                        playerMovement.bWallSlide = false;
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
                default: break;
            }
        }

        private void OnStayState()
        {
            if (currentState != PlayerStates.WallSlide && currentState != PlayerStates.Boost && currentState != PlayerStates.Crouch && bCanMove)
            {
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
                    if (playerMovement.rb2D.velocity.y == 0)
                    {
                        playerMovement.rb2D.AddForce(Vector2.up * 2.5f, ForceMode2D.Impulse);
                    }
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
                            if (jumpTimeCounter <= jumpTime / 2) playerMovement.bWallSlide = true; 
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
                    if (!playerMovement.bCanWallSlide || playerMovement.direction.y <= -0.5 || playerMovement.bIsGrounded) SetCurrentState(PlayerStates.Fall);
                    playerMovement.bCanWallSlide = Utils.ShootBoxcast(playerMovement.transform.position, 
                        new Vector2(0.2f, playerMovement.initColliderSize.y / 4), Vector2.right * playerMovement.lookingDirection, 
                        playerMovement.initColliderSize.x / 2 + 0.2f, "Environment");
                    break;
                default: break;
            }
        }

        private void OnExitState(PlayerStates state)
        {
            switch (state)
            {
                case PlayerStates.Boost:
                    playerMovement.stick.enabled = false;
                    playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
                    break;
                case PlayerStates.WallSlide:
                    StartCoroutine(WallJumpTimer());
                    playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
                    break;
                case PlayerStates.Jump:
                    jumpTimeCounter = jumpTime;
                    bNormalJump = false;
                    break;
                case PlayerStates.Dodge:
                    var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    foreach (var enemy in enemies)
                    {
                        Physics2D.IgnoreCollision(GetComponentInChildren<CapsuleCollider2D>(), 
                            enemy.GetComponent<Collider2D>(), false);
                    }
                    break;
                default: break;
            }
        }
        
         private IEnumerator Dodge()
        {
            playerMovement.bCanDodge = false;
            PlayerCombat combat = GetComponent<PlayerCombat>();
            combat.SetPlayerVulnerable(false);
            playerMovement.rb2D.velocity = new Vector2(dodgeDirection * playerMovement.dodgeForce, playerMovement.rb2D.velocity.y);
            yield return new WaitForSeconds(playerMovement.dodgeTime);
            combat.SetPlayerVulnerable(true);
            playerMovement.bCanUncrouch = !Utils.ShootBoxcast(playerMovement.transform.position, 
                new Vector2(playerMovement.initColliderSize.x - 0.1f, playerMovement.initColliderSize.y / 2), 
                Vector2.up, playerMovement.initColliderSize.y / 2 + 0.1f, "Environment");
            playerMovement.bCanWallSlide = Utils.ShootBoxcast(playerMovement.transform.position, 
                new Vector2(0.2f, playerMovement.initColliderSize.y / 4), 
                Vector2.right * playerMovement.lookingDirection, playerMovement.initColliderSize.x / 2 + 0.2f, "Environment");
            if (!playerMovement.bCanUncrouch)
            {
                playerMovement.bUncrouch = true;
                SetCurrentState(PlayerStates.Crouch);
            }
            else if (currentState == PlayerStates.Boost) SetCurrentState(PlayerStates.Boost);
            else if (playerMovement.bIsGrounded == false && playerMovement.bCanWallSlide) SetCurrentState(PlayerStates.WallSlide);
            else if (playerMovement.bCanUncrouch && playerMovement.bIsGrounded && playerMovement.moveDirection == Vector2.zero) SetCurrentState(PlayerStates.Idle);
            else if (playerMovement.bCanUncrouch && playerMovement.bIsGrounded && playerMovement.moveDirection != Vector2.zero) SetCurrentState(PlayerStates.Move);
            else SetCurrentState(PlayerStates.Fall);
            yield return new WaitForSeconds(playerMovement.dodgeCooldown);
            playerMovement.bCanDodge = true;
        }

        private IEnumerator Boost()
        {
            var destination = new Vector2(playerMovement.transform.position.x, playerMovement.transform.position.y + playerMovement.boostHeight);
            yield return new WaitUntil(() => IsOnPosition(destination));
            if (currentState == PlayerStates.Boost)
            {
                playerMovement.stick.enabled = true;
                playerMovement.rb2D.gravityScale = 0;
            }
            yield return StartCoroutine(Wait(playerMovement.boostTime, PlayerStates.Boost));
            if (currentState == PlayerStates.Boost) SetCurrentState(PlayerStates.Fall);
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

        private bool IsOnPosition(Vector2 destination)
        {
            transform.position = Vector2.MoveTowards(playerMovement.transform.position, destination, playerMovement.boostSpeed);
            return Vector2.Distance(playerMovement.transform.position, destination) < 0.5f || currentState != PlayerStates.Boost;
        }

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
    }
}