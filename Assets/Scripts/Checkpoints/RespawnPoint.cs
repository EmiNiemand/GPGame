using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheckPoints
{
    public class RespawnPoint : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider != null && collider.CompareTag("Player"))
            {
                FindObjectOfType<GameManager>().respawnPointEvent.Invoke(gameObject);
            }
        }
    }
}

