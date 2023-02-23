using UnityEngine;
/// <summary>
///     Any other object can damage object implementing this interface
///</summary>

public interface IDamageable
{
    int HP { get; }
    int maxHP { get; }

    void ReceiveDamage(int damage, 
                       Vector2 sourcePoint = new Vector2(), 
                       int knockbackForce = 0);
}
