using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [SerializeField] protected int damage;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameObject.tag = "Trap";
    }

    protected abstract void Attack();

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other && other.gameObject.CompareTag("Player"))
        {
            Attack();
        }
    }
}
