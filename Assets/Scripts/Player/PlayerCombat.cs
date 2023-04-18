using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackStrength { Light=1, Medium, Heavy }

public enum AttackType
{
    AttackDodge, 
    AttackRun, 
    AttackJumpUp, AttackJumpFront, AttackJumpDown,
    AttackBoost
}

namespace Player
{
    public class PlayerCombat : MonoBehaviour, IDamageable
    {
        [field: SerializeField] public int maxHP { get; protected set; }
        public int HP { get; protected set; }
        public float invincibilityTime = 1.0f;
        public float cooldownTime = 1.0f;

        private Rigidbody2D rb2D;
        private Weapon weapon;
        private PlayerManager playerManager;

        [SerializeField] private bool bIsCombatActivated = false;
        private bool bIsVulnerable = true;
        private bool bIsInvincible = false;
        private bool bIsAttacking = false;
        private bool bIsOnCooldown = false;
        private PlayerStates playerState;
        private readonly PlayerStates[] statesBlockingAttack = { PlayerStates.Crouch };
        private AttackType attackType;
        private AttackStrength attackStrength;

        // Start is called before the first frame update
        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            weapon = GetComponentInChildren<Weapon>();
            rb2D = GetComponent<Rigidbody2D>();

            HP = maxHP;
        }
    
        public void OnAttack()
        {
            if (!bIsCombatActivated || bIsOnCooldown) return;
            bIsOnCooldown = true;

            switch (playerState)
            {
                case PlayerStates.Idle or PlayerStates.Move:
                    attackType = AttackType.AttackRun;
                    attackStrength = AttackStrength.Light;
                    break;
                case PlayerStates.Jump or PlayerStates.Fall:
                    switch (weapon.GetLookingDirection())
                    {
                        case Weapon.LookingDirection.Up:
                            attackType = AttackType.AttackJumpUp;
                            attackStrength = AttackStrength.Light;
                            break;
                        case Weapon.LookingDirection.Left or Weapon.LookingDirection.Right:
                            attackType = AttackType.AttackJumpFront;
                            attackStrength = AttackStrength.Medium;
                            break;
                        case Weapon.LookingDirection.Down:
                            attackType = AttackType.AttackJumpDown;
                            attackStrength = AttackStrength.Medium;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case PlayerStates.Boost:
                    attackType = AttackType.AttackBoost;
                    attackStrength = AttackStrength.Heavy;
                    break;
                case PlayerStates.Dodge:
                    attackType = AttackType.AttackDodge;
                    attackStrength = AttackStrength.Heavy;
                    break;
                // Not implemented yet
                case PlayerStates.WallSlide:
                case PlayerStates.LedgeGrab:
                case PlayerStates.Crouch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            playerManager.AttackStart(attackType);
        }

        public void OnWeaponHit(Vector2 hitPosition)
        {
            playerManager.OnWeaponHit(hitPosition);
            // Apply movement modifiers
            switch (attackType)
            {
                case AttackType.AttackDodge:
                    break;
                case AttackType.AttackRun:
                    break;
                case AttackType.AttackJumpUp:
                    break;
                case AttackType.AttackJumpFront:
                    break;
                case AttackType.AttackJumpDown:
                    break;
                case AttackType.AttackBoost:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    
        public bool Heal(int value)
        {
            HP += value;
            if (HP > maxHP) HP = maxHP;
            
            return true;
        }
    
        public void ReceiveDamage(int damage, Vector2 sourcePoint=default, int knockbackForce=0)
        {
            if (!bIsVulnerable || bIsInvincible) return;
            StartCoroutine(InvincibilityTime());

            // Debug.Log("Player position: " + transform.position + "\t\tSource point: " + sourcePoint + "\t\tKnockback: " + knockbackForce);

            HP -= damage;
            
            if (HP <= 0) { OnDeath(); return; }
            
            if(knockbackForce != 0)
            {
                rb2D.velocity = Vector2.zero;
                Vector2 knockbackVector = (Vector2)transform.position - sourcePoint;
                rb2D.AddForce(knockbackVector.normalized * knockbackForce);
            }
            playerManager.OnReceiveDamage(sourcePoint);
        }

        private IEnumerator InvincibilityTime()
        {
            bIsInvincible = true;
            yield return new WaitForSeconds(invincibilityTime);
            bIsInvincible = false;
        }

        //TODO: find better solution
        private bool cooldownRunning = false;
        private IEnumerator CooldownTime()
        {
            if (cooldownRunning) yield break;
            cooldownRunning = true;
            yield return new WaitForSeconds(cooldownTime);
            bIsOnCooldown = false;
            cooldownRunning = false;
        }
    
        private void OnDeath()
        {
            HP = maxHP;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            
            playerManager.OnDeath();
        }
    
        public void SetPlayerVulnerable(bool bVulnerability) { bIsVulnerable = bVulnerability; }
        public void UnlockCombat() { bIsCombatActivated = true; }

        public void UpdateState(PlayerStates newState)
        {
            playerState = newState;
            weapon.UpdateState(newState);
        }
        
        // Pass-through methods called from animations
        // -------------------------------------------
        public void AttackDamageStart() { weapon.StartAttack(attackStrength); }
        public void AttackDamageEnd() { weapon.EndAttack(); StartCoroutine(CooldownTime()); }

        public void UpdateMovingDirection(Vector2 direction) { weapon.UpdateMovingDirection(direction); }
        public void UpdateLookingDirection(int lookingDirection) { weapon.UpdateLookingDirection(lookingDirection); }

        private bool CanAttackState()
        {
            return System.Array.BinarySearch(
                statesBlockingAttack, playerState) < 0;
        }
    }
}
