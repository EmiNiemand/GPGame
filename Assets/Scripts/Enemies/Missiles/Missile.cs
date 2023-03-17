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
        protected bool isDestroyed = false;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                isDestroyed = true;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Light2D>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<ParticleSystem>().Play();
                Destroy(gameObject, 1f);
                col.GetComponentInParent<PlayerCombat>().ReceiveDamage(damage, transform.position, knockbackForce);
            }
            else if (col.CompareTag("Environment"))
            {
                isDestroyed = true;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Light2D>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<ParticleSystem>().Play();
                Destroy(gameObject, 1f);
            }
        }
        
        
    }
}