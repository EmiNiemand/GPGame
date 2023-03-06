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
    }

    protected override void Attack(GameObject other)
    {
        //TODO: temporary workaround
        //TODO: will only work properly if spikes are placed horizontally 
        other.GetComponentInParent<IDamageable>().ReceiveDamage(
            damage, other.transform.position - new Vector3(0, 5), knockbackForce);
    }
}
