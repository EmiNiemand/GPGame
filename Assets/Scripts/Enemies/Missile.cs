using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Enemies
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private int damage;
        [SerializeField] private int knockbackForce;
        [SerializeField] private float speed;
        private void FixedUpdate()
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(GetComponentInParent<Enemy>().lookingDirection * speed, 0), ForceMode2D.Impulse);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col) return;
            if (col.CompareTag("Player"))
            {
                col.gameObject.GetComponentInParent<PlayerCombat>().ReceiveDamage(damage, transform.position, knockbackForce);
                GetComponent<Light2D>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<ParticleSystem>().Play();
                Destroy(gameObject, 1f);
            }
            else if (col.CompareTag("Environment"))
            {
                GetComponent<Light2D>().enabled = false;
                GetComponent<CircleCollider2D>().enabled = false;
                GetComponent<ParticleSystem>().Play();
                Destroy(gameObject, 1f);
            }
        }
        
        
    }
}