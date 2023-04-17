using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Other.EMA
{
    public class LaunchShroom : MonoBehaviour
    {
        private GameObject player;
        [SerializeField] private float launchValue = 1.0f;
        [SerializeField] private float cooldown = 0.5f;
        private bool isOnCooldown = false;
    
        // Start is called before the first frame update
        void Start()
        {
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Player.PlayerMovement playerMovement = player.GetComponent<Player.PlayerMovement>();

            if (other.CompareTag("Player") && !playerMovement.bIsGrounded && !isOnCooldown)
            {
                StartCoroutine(Cooldown());
                playerMovement.rb2D.velocity = Vector2.zero;
                playerMovement.rb2D.AddForce(transform.rotation * Vector2.up * launchValue, ForceMode2D.Impulse);
            }
        }

        private IEnumerator Cooldown()
        {
            isOnCooldown = true;
            yield return new WaitForSeconds(cooldown);
            isOnCooldown = false;
        }
    }
}

