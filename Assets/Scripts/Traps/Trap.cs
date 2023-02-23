using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [SerializeField] protected int damage;

    // Start is called before the first frame update
    protected virtual void Setup()
    {
        gameObject.tag = "Trap";
    }

    protected abstract void Attack(GameObject other);

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other && other.gameObject.layer == (int)Layers.Player)
        {
            Attack(other.gameObject);
        }
    }
}
