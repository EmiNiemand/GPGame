using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;
using UnityEngine.U2D;

public class Weapon : MonoBehaviour
{
    // Gives info about player's looking direction
    // as well as rotation degrees needed to orient
    // to specific direction
    enum LookingDirection
    {
        LEFT=0,
        UP=90,
        RIGHT=180,
        DOWN=270
    }

    [SerializeField] private float attackCooldown;
    [SerializeField] private int lightDamage;
    [SerializeField] private int heavyDamage;
    [SerializeField] private int knockbackForce;

    private Player.PlayerMovement playerMovement;
    private PolygonCollider2D col;
    private SpriteShapeRenderer spriteShapeRenderer;

    private bool bIsAttackOnCooldown = false;
    private AttackType currentAttack;
    private LookingDirection lookingDirection;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = transform.parent.gameObject.GetComponent<Player.PlayerMovement>();
        col = GetComponent<PolygonCollider2D>();
        spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();

        col.enabled = false;
        spriteShapeRenderer.enabled = false;
        lookingDirection = LookingDirection.RIGHT;

        knockbackForce *= Utils.KnockbackMult;
    }

    public void StartAttack(AttackType attackType)
    {
        currentAttack = attackType;

        GetLookingDirection();
        transform.rotation = Quaternion.Euler(0, 0, (float)lookingDirection);

        // Temporary workaround for heavy attacks
        //TODO: improve
        Vector3 weaponPos = new Vector3(attackType==AttackType.Light?0:0.5f, 0);
        switch (lookingDirection)
        {
            case LookingDirection.LEFT:
                transform.localPosition = weaponPos; break;
            case LookingDirection.RIGHT:
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
        if (collision != null && 
            collision.gameObject.TryGetComponent<IDamageable>(
                out IDamageable damageable))
        {
            // Temporary workaround for boar
            //TODO: improve somehow
            if(collision.GetType() == typeof(EdgeCollider2D)) return;

            int damageValue = currentAttack==AttackType.Light ? lightDamage : heavyDamage;
            int knockValue  = currentAttack==AttackType.Light ? knockbackForce/4 : knockbackForce;
            Vector2 position = playerMovement.transform.position;
            damageable.ReceiveDamage(damageValue, position, knockValue);
        }
    }


    // Helper methods
    // --------------
    private void GetLookingDirection()
    {
        Vector2 direction = playerMovement.direction;
        int lookDirection = playerMovement.lookingDirection;

        if (direction.y > 0.5)
        {
            lookingDirection = LookingDirection.UP;
        }
        else if (direction.y < -0.5 && (playerMovement.CheckCurrentState(Player.PlayerStates.Jump) || playerMovement.CheckCurrentState(Player.PlayerStates.Fall)))
        {
            lookingDirection = LookingDirection.DOWN;
        }
        else if (direction.x > 0.5 || lookDirection > 0)
        {
            lookingDirection = LookingDirection.LEFT;
        }
        else if (direction.x < -0.5 || lookDirection < 0)
        {
            lookingDirection = LookingDirection.RIGHT;
        }
    }
}
