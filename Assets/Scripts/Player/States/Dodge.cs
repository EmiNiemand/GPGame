using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.States
{
    public class Dodge : State
    {
        private int dodgeDirection;
        private float dodgeTimer;
        private PlayerCombat combat;

        public Dodge(PlayerMovement playerMovement) : base(playerMovement)
        {
            combat = playerMovement.gameObject.GetComponent<PlayerCombat>();
        }

        public override void OnEnterState()
        {
            dodgeTimer = playerMovement.dodgeSpeed;
            
            playerMovement.bIsDodging = true;
            combat.SetPlayerVulnerable(false);
            
            if (playerMovement.CheckPreviousState(PlayerStates.Boost) || playerMovement.CheckPreviousState(PlayerStates.WallSlide)) 
                playerMovement.rb2D.gravityScale *= 0.2f;
            
            //TODO: change it to disabling collisions with enemy layer, remember to add enemy layer and enemy prefabs to it
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            var playerCollider = playerMovement.gameObject.GetComponentInChildren<CapsuleCollider2D>();
            
            foreach (var enemy in enemies)
            {
                Physics2D.IgnoreCollision(playerCollider, enemy.GetComponent<Collider2D>(), true);
            }
            
            dodgeDirection = playerMovement.lookingDirection;

            if (!playerMovement.CheckPreviousState(PlayerStates.WallSlide)) return;
            dodgeDirection *= -1;
            playerMovement.lookingDirection *= -1;
            
        }

        public override void OnStayState()
        {
            dodgeTimer -= Time.fixedDeltaTime;
            if (dodgeTimer > 0)
            {
                playerMovement.rb2D.AddForce(Vector2.right * (dodgeDirection * playerMovement.dodgeLength /
                                                              playerMovement.dodgeSpeed), ForceMode2D.Impulse);
                return;
            }

            var playerPosition = playerMovement.transform.position;
            var colliderHalfWidth = playerMovement.initColliderSize.x / 2;
            var colliderHalfHeight = playerMovement.initColliderSize.y / 2;
            
            playerMovement.bCanUncrouch = !Utils.ShootBoxcast(playerPosition, 
                new Vector2(colliderHalfWidth, colliderHalfHeight), Vector2.up, 
                colliderHalfHeight / 2, "Environment");
            
            playerMovement.bCanWallSlide = Utils.ShootBoxcast(playerPosition, 
                new Vector2(colliderHalfWidth, colliderHalfWidth / 2), 
                Vector2.right * playerMovement.lookingDirection, colliderHalfWidth, "Environment");
            // Force Player to crouch if there's not enough space above him
            // ------------------------------------------------------------
            if (!playerMovement.bCanUncrouch)
            {
                playerMovement.bUncrouch = true;
                playerMovement.SetCurrentState(PlayerStates.Crouch);
            }
            // From Dodge player can either Walk or Fall
            // -----------------------------------------------------------
            else if (playerMovement.bCanUncrouch && playerMovement.bIsGrounded)
                playerMovement.SetCurrentState(playerMovement.moveDirection == Vector2.zero ? PlayerStates.Idle : PlayerStates.Move);
            else playerMovement.SetCurrentState(PlayerStates.Fall);
        }

        public override void OnExitState()
        {
            playerMovement.bIsDodging = false;
            combat.SetPlayerVulnerable(true);
            
            playerMovement.rb2D.gravityScale = playerMovement.initGravityScale;
            
            //TODO: change it to disabling collisions with enemy layer, remember to add enemy layer and enemy prefabs to it
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");
            var playerCollider = playerMovement.gameObject.GetComponentInChildren<CapsuleCollider2D>();
            
            foreach (var enemy in enemies)
            {
                Physics2D.IgnoreCollision(playerCollider, enemy.GetComponent<Collider2D>(), false);
            }
            
            playerMovement.StartCoroutine(playerMovement.DodgeCooldown());
        }
    }   
}
