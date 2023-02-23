using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Spikes : Trap
{
    // Start is called before the first frame update
    void Start()
    {
        base.Setup();
    }

    protected override void Attack(GameObject other)
    {
        //TODO: temporary workaround
        other.GetComponentInParent<IDamageable>().ReceiveDamage(damage);
    }
}
