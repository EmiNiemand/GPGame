using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum AttackType { Light, Heavy }

namespace Player
{
    public class PlayerCombat : MonoBehaviour, IDamageable
    {
        public int HP { get; protected set; }
        [field: SerializeField] public int maxHP { get; protected set; }
        private Rigidbody2D rb2D;
        private Weapon weapon;
        private PlayerMovement playerMovement;
    
        private bool bIsVulnerable = true;
        private bool bIsAttacking = false;
        [SerializeField] private bool bIsCombatActivated = false;
        private PlayerStates[] statesBlockingAttack = { PlayerStates.Crouch, PlayerStates.Dodge };
    
        // UI
        private GameUI gui;

        // Start is called before the first frame update
        void Start()
        {
            playerMovement = GetComponent<PlayerMovement>();
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
            if (HP + value > maxHP) HP = maxHP;
            else HP += value;
    
            gui.UpdateHealth(HP);
        }
    
        public void ReceiveDamage(int damage, Vector2 sourcePoint=default, int knockbackForce=0)
        {
            if (!bIsVulnerable) return;
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
