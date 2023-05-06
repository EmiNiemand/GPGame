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
        private bool lastAttackSuccessful = false;

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
            lastAttackSuccessful = false;

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
                case PlayerStates.LedgeClimb:
                case PlayerStates.Crouch:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            playerManager.AttackStart(attackType);
        }

        public void OnWeaponHit(Vector2 hitPosition)
        {
            lastAttackSuccessful = true;
            playerManager.OnWeaponHit(hitPosition);
            // Apply movement modifiers
            switch (attackType)
            {
                case AttackType.AttackDodge:
                    //TODO: maybe force dodge?
                    playerManager.MovementModifierApply(MovementModifier.BoostForward, 1.0f);
                    break;
                case AttackType.AttackJumpUp:
                    playerManager.MovementModifierApply(MovementModifier.BoostDown);
                    break;
                case AttackType.AttackJumpFront:
                    playerManager.MovementModifierApply(MovementModifier.BoostForward, 0.2f);
                    break;
                case AttackType.AttackJumpDown:
                    playerManager.MovementModifierApply(MovementModifier.BoostUp);
                    break;
                case AttackType.AttackBoost:
                    playerManager.MovementModifierApply(MovementModifier.BoostUp);
                    break;
                case AttackType.AttackRun:
                default:
                    break;
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

        public void AttackDamageEnd()
        {
            Debug.Log("BBBBBBBBBBBBBBBB");
            weapon.EndAttack(); 
            StartCoroutine(CooldownTime());
            
            if (lastAttackSuccessful) return;
            switch (attackType)
            {
                case AttackType.AttackDodge:
                    playerManager.MovementModifierApply(MovementModifier.Stop, 0.5f);
                    break;
                case AttackType.AttackRun:
                    playerManager.MovementModifierApply(MovementModifier.Slow, 0.5f);
                    break;
                case AttackType.AttackJumpFront:
                    playerManager.MovementModifierApply(MovementModifier.Slow, 1.0f);
                    break;
                case AttackType.AttackBoost:
                    playerManager.MovementModifierApply(MovementModifier.Slow, 0.5f);
                    break;
                case AttackType.AttackJumpUp:
                case AttackType.AttackJumpDown:
                default:
                    break;
            }
        }

        public void UpdateMovingDirection(Vector2 direction) { weapon.UpdateMovingDirection(direction); }
        public void UpdateLookingDirection(int lookingDirection) { weapon.UpdateLookingDirection(lookingDirection); }

        private bool CanAttackState()
        {
            return System.Array.BinarySearch(
                statesBlockingAttack, playerState) < 0;
        }
    }
}
