using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Other.EMA
{
    public class Vine : MonoBehaviour
    {
        private GameObject player;
        private Player.PlayerMovement playerMovement;

        private float playerGravityScale;
        private bool stopMovement = false;
        
        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindWithTag("Player");
            playerMovement = player.GetComponent<Player.PlayerMovement>();
            
            Debug.Log(playerGravityScale);
        }
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            playerGravityScale = playerMovement.rb2D.gravityScale;

        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (!stopMovement && playerMovement.direction.y != 0)
                {
                    playerMovement.rb2D.gravityScale = 0;
                    playerMovement.rb2D.velocity = Vector2.zero;
                    stopMovement = true;
                }
                playerMovement.rb2D.AddForce(Vector2.up * playerMovement.direction.y * playerMovement.moveSpeed / 2, ForceMode2D.Impulse);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                stopMovement = false;
                playerMovement.rb2D.gravityScale = playerGravityScale;
            }
        }
    }
}

