using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Spikes : Trap
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerCombat>().ReceiveDamage(damage);
    }
}
