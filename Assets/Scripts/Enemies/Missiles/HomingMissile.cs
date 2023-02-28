using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Enemies.Missiles
{
    public class HomingMissile : Missile
    {
        private bool FollowPlayer = false;
        private void Start()
        {
            StartCoroutine(GoUp());
            StartCoroutine(DestroyAfter());
        }

        void FixedUpdate()
        {
            if (!FollowPlayer) return;
            GetComponent<Rigidbody2D>().AddForce(((Vector2)(GameObject.FindWithTag("Player").transform.position - transform.position)).normalized * speed, ForceMode2D.Impulse);
        }

        private IEnumerator GoUp()
        {
            yield return new WaitUntil(() => IsOnPosition());
            FollowPlayer = true;
        }

        private IEnumerator DestroyAfter()
        {
            yield return new WaitForSeconds(5f);
            GetComponent<Light2D>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            GetComponent<ParticleSystem>().Play();
            Destroy(gameObject, 1f);
        }

        private bool IsOnPosition()
        {
            var destination = new Vector2(transform.parent.position.x, transform.parent.position.y + 2.5f);
            transform.position = Vector2.MoveTowards(transform.position, destination, 0.01f);
            return Vector2.Distance(transform.position, destination) < 0.1f;
        }
    }
}

