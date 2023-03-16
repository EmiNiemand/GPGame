using UnityEngine;
/// <summary>
///     Any other object can damage object implementing this interface
///     <br/><br/>
///     ! IMPORTANT ! <br/>
///     You need to multiply <b>knockbackForce</b> of an object by <b>Utils.KnockbackMult</b>.
///     Only then you get reasonable numbers to scale knockback by (~10 is good).
///</summary>
public interface IDamageable
{
    // REMOVED FOR NOW
    // e.g.: some objects may be just one-hit destructable,
    // playerCollisions class doesn't use them as anything meaningful;
    // Maybe move them to another interface?
    // [because these might be useful when implementing
    // automated HP bar addition to object]
    
    // int HP { get; }
    // int maxHP { get; }
    
    //TODO: might need to add some enum for damage type
    // Let's say that some wall can be only destructed by 
    // charging boar or fire, then we need to determine
    // source of damage
    void ReceiveDamage(int damage, 
                       Vector2 sourcePoint = default, 
                       int knockbackForce = 0);
}
