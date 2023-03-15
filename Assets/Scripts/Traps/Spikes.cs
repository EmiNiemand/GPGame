using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Spikes : Trap
{
    public int knockbackForce = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        base.Setup();
        knockbackForce *= Utils.KnockbackMult;
    }

    protected override void Attack(GameObject other)
    {
        // var filter = new ContactFilter2D
        // {
        //     useLayerMask = true, layerMask = (int)Layers.Player
        // };
        // var contactPoints = new List<ContactPoint2D>();
        // GetComponent<CompositeCollider2D>().GetContacts(filter, contactPoints);
        
        //TODO: will only work on Player
        //TODO: will only work properly if spikes are placed horizontally

        //TODO: implement IDamageable in PlayerColliders
        other.GetComponentInParent<IDamageable>().ReceiveDamage(
            damage, other.transform.position - new Vector3(0, 5), knockbackForce);
    }
}
