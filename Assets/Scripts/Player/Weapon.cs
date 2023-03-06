using System.Collections;
using System.Collections.Generic;
using Enemies;
using Player;
using UnityEngine;
using UnityEngine.U2D;

public class Weapon : MonoBehaviour
{
    enum LookingDirection
    {
        Left=0,
        Up=1,
        Right=2,
        Down=3
    }

    [SerializeField] private float attackCooldown;
    [SerializeField] private int lightDamage;
    [SerializeField] private int heavyDamage;
    [SerializeField] private int knockbackForce;
    
    private PolygonCollider2D col;
    private SpriteShapeRenderer spriteShapeRenderer;
    private PlayerCombat playerCombat;

    private bool bIsAttackOnCooldown = false;
    private AttackType currentAttack;
    private LookingDirection lookingDirection;
    
    private PlayerStates playerState;
    private Vector2 playerMovingDirection;
    private int playerLookingDirection;
    

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<PolygonCollider2D>();
        spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        playerCombat = GetComponentInParent<PlayerCombat>();

        col.enabled = false;
        spriteShapeRenderer.enabled = false;
        lookingDirection = LookingDirection.Right;

        knockbackForce *= Utils.KnockbackMult;
    }

    public void StartAttack(AttackType attackType)
    {
        currentAttack = attackType;

        GetLookingDirection();
        transform.rotation = Quaternion.Euler(0, 0, (int)lookingDirection * 90);

        // Temporary workaround for heavy attacks
        //TODO: improve
        Vector3 weaponPos = new Vector3(attackType==AttackType.Light?0:0.5f, 0);
        switch (lookingDirection)
        {
            case LookingDirection.Left:
                transform.localPosition = weaponPos; break;
            case LookingDirection.Right:
                transform.localPosition = -weaponPos; break;
            default: break;
        }

        col.enabled = true;
        spriteShapeRenderer.enabled = true;
        bIsAttackOnCooldown = true;
    }

    public void EndAttack()
    {
        col.enabled = false;
        spriteShapeRenderer.enabled = false;
        bIsAttackOnCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if(collision.gameObject.TryGetComponent<IDamageable>(out var damageable))
        {
            //TODO: improve somehow
            // Temporary workaround for boar
            if (collision is EdgeCollider2D) return;

            int damageValue = currentAttack == AttackType.Light ? lightDamage : heavyDamage;
            int knockValue = currentAttack == AttackType.Light ? knockbackForce / 4 : knockbackForce;
            Vector2 position = transform.parent.transform.position;
            damageable.ReceiveDamage(damageValue, position, knockValue);
            playerCombat.OnWeaponHit(collision.ClosestPoint(transform.position));
        }
    }
    
    public void UpdateState(PlayerStates newState) { playerState = newState; }
    public void UpdateMovingDirection(Vector2 newDirection) { playerMovingDirection = newDirection; }
    public void UpdateLookingDirection(int newDirection) { playerLookingDirection = newDirection; }

    // Helper methods
    // --------------
    private void GetLookingDirection()
    {
        Vector2 direction = playerMovingDirection;
        int lookDirection = playerLookingDirection;

        if (direction.y > 0.5)
        {
            lookingDirection = LookingDirection.Up;
        }
        else if (direction.y < -0.5 && (playerState is PlayerStates.Jump or PlayerStates.Fall))
        {
            lookingDirection = LookingDirection.Down;
        }
        else if (direction.x > 0.5 || lookDirection > 0)
        {
            lookingDirection = LookingDirection.Left;
        }
        else if (direction.x < -0.5 || lookDirection < 0)
        {
            lookingDirection = LookingDirection.Right;
        }
    }
}
