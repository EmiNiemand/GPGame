using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheckPoints
{
    public class RespawnPoint : MonoBehaviour, IUsable
    {
        public void OnEnter(GameObject user)
        {
            FindObjectOfType<GameManager>().respawnPointEvent.Invoke(gameObject);
        }

        public void OnExit(GameObject user) { }

        public void Use(GameObject user) { }
    }
}

