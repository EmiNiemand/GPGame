using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackType { Light, Heavy }

namespace Player
{
    public class PlayerCombat : MonoBehaviour, IDamageable
    {
        [field: SerializeField] public int maxHP { get; protected set; }
        public int HP { get; protected set; }
        public float invincibilityTime = 1.0f;

        private Rigidbody2D rb2D;
        private Weapon weapon;
        private PlayerManager playerManager;

        [SerializeField] private bool bIsCombatActivated = false;
        private bool bIsVulnerable = true;
        private bool bIsInvincible = false;
        private bool bIsAttacking = false;
        private PlayerStates playerState;
        private readonly PlayerStates[] statesBlockingAttack = { PlayerStates.Crouch, PlayerStates.Dodge };

        // Start is called before the first frame update
        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            weapon = GetComponentInChildren<Weapon>();
            rb2D = GetComponent<Rigidbody2D>();

            HP = maxHP;
        }
    
        public void OnAttack(bool actionStarted)
        {
            if (actionStarted && 
                bIsCombatActivated &&
                CanAttackState())
                playerManager.AttackStart();
            else if (!actionStarted) 
                playerManager.AttackEnd();
        }

        public void OnWeaponHit(Vector2 hitPosition)
        {
            playerManager.OnWeaponHit(hitPosition);
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

            HP -= damage;
            
            if (HP <= 0)
            {
                OnDeath();
            }
            else if(knockbackForce != 0)
            {
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
    
        private void OnDeath()
        {
            HP = maxHP;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            
            playerManager.OnDeath();
        }
    
        public void SetPlayerVulnerable(bool bVulnerability)
        {
            bIsVulnerable = bVulnerability;
        }
    
        public bool GetIsAttacking() { return bIsAttacking; }
        public void UnlockCombat() { bIsCombatActivated = true; }

        public void UpdateState(PlayerStates newState)
        {
            playerState = newState;
            weapon.UpdateState(newState);
        }
        
        // Pass-through methods called from animations
        // -------------------------------------------
        public void AttackDamageStart(AttackType attackType) { weapon.StartAttack(attackType); }
        public void AttackDamageEnd() { weapon.EndAttack(); }

        public void UpdateMovingDirection(Vector2 direction) { weapon.UpdateMovingDirection(direction); }
        public void UpdateLookingDirection(int lookingDirection) { weapon.UpdateLookingDirection(lookingDirection); }

        private bool CanAttackState()
        {
            return System.Array.BinarySearch(
                statesBlockingAttack, playerState) < 0;
        }
    }
}
