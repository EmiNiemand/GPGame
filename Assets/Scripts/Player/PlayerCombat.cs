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
        private PlayerMovement playerMovement;

        //TODO: it shouldn't be here, move to PlayerManager
        private PlayerAnimations playerAnimations;

        [SerializeField] private bool bIsCombatActivated = false;
        private bool bIsVulnerable = true;
        private bool bIsInvincible = false;
        private bool bIsAttacking = false;
        private PlayerStates[] statesBlockingAttack = { PlayerStates.Crouch, PlayerStates.Dodge };
    
        // External references, probably need to improve that
        private GameUI gui;

        // Start is called before the first frame update
        void Start()
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerAnimations = GetComponentInChildren<PlayerAnimations>();
            weapon = GetComponentInChildren<Weapon>();
            rb2D = GetComponent<Rigidbody2D>();

            HP = maxHP;
            
            gui = FindObjectOfType<GameUI>();
            gui.Setup();
            gui.SetMaxHealth(maxHP);
        }
    
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started && 
                bIsCombatActivated &&
                canAttackState())
                    bIsAttacking = true;
            else if (context.canceled) 
                bIsAttacking = false;
        }
    
        public void Heal(int value)
        {
            HP += value;
            if (HP > maxHP) HP = maxHP;
    
            gui.UpdateHealth(HP);
        }
    
        public void ReceiveDamage(int damage, Vector2 sourcePoint=default, int knockbackForce=0)
        {
            if (!bIsVulnerable || bIsInvincible) return;
            StartCoroutine(InvincibilityTime());

            HP -= damage;
            
            if (HP <= 0)
            {
                OnDeath();
                return;
            }

            if(knockbackForce != 0)
            {
                Vector2 knockbackVector = (Vector2)transform.position - sourcePoint;
                rb2D.AddForce(knockbackVector.normalized * knockbackForce);
            }
            
            gui.UpdateHealth(HP);
            playerAnimations.ShowReceiveDamage();
        }

        private IEnumerator InvincibilityTime()
        {
            bIsInvincible = true;
            yield return new WaitForSeconds(invincibilityTime);
            bIsInvincible = false;
        }
    
        private void OnDeath()
        {
            //make coroutine to play death animation or smth
            HP = maxHP;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            FindObjectOfType<GameManager>().respawnEvent.Invoke();
            gui.UpdateHealth(HP);
        }
    
        public void SetPlayerVulnerable(bool bVulnerability)
        {
            bIsVulnerable = bVulnerability;
        }
    
        public bool GetIsAttacking() { return bIsAttacking; }
        public void UnlockCombat() { bIsCombatActivated = true; }

        // Pass-through methods called from animations
        // -------------------------------------------
        public void AttackLightDamageStart() { weapon.StartAttack(AttackType.Light); }
        public void AttackHeavyDamageStart() { weapon.StartAttack(AttackType.Heavy); }
        public void AttackDamageEnd() { weapon.EndAttack(); }

        private bool canAttackState()
        {
            return System.Array.BinarySearch(
                statesBlockingAttack, 
                playerMovement.GetPlayerState()) < 0;
        }
    }
}
