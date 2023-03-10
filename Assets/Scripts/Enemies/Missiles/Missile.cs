using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Enemies.Missiles
{
    public abstract class Missile : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private int knockbackForce;
        [SerializeField] protected float speed;

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                col.gameObject.GetComponentInParent<PlayerCombat>().ReceiveDamage(damage, transform.position, knockbackForce);
                GetComponent<Light2D>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<ParticleSystem>().Play();
                Destroy(gameObject, 1f);
            }
            else if (col.gameObject.CompareTag("Environment"))
            {
                GetComponent<Light2D>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<ParticleSystem>().Play();
                Destroy(gameObject, 1f);
            }
        }
        
        
    }
}