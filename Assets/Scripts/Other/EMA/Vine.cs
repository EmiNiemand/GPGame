using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Other.EMA
{
    public class Vine : MonoBehaviour
    {
        private GameObject player;
        private PlayerMovement playerMovement;
        
        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.FindWithTag("Player");
            playerMovement = player.GetComponent<PlayerMovement>();
        }
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (!playerMovement.CheckCurrentState(PlayerStates.Climb) && playerMovement.direction.y != 0)
                {
                    playerMovement.SetCurrentState(PlayerStates.Climb);
                }
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (!playerMovement.CheckCurrentState(PlayerStates.Climb) && playerMovement.direction.y != 0)
                {
                    playerMovement.SetCurrentState(PlayerStates.Climb);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                playerMovement.SetCurrentState(PlayerStates.Fall);
            }
        }
    }
}

