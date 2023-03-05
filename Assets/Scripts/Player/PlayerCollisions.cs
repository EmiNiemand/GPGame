using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerCollisions : MonoBehaviour
    {
        private GameObject player;
        // Needed to pass info about received damage
        private PlayerManager playerManager;
        private List<GameObject> usables;

        void Start() {
            player = transform.parent.gameObject;
            playerManager = GetComponentInParent<PlayerManager>();
            usables = new List<GameObject>();
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // Object implementing IUsable interface
            // -------------------------------------
            if(other.gameObject.layer == (int)Layers.Usable)
            {
                usables.Add(other.gameObject);
                other.GetComponent<IUsable>().OnEnter(player);
            }
        }

        public void OnUse()
        {
            if(usables.Count == 0) return;

            usables[^1].GetComponent<IUsable>().Use(player);
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.gameObject.layer == (int)Layers.Usable &&
               usables.Remove(other.gameObject))
            {
                other.gameObject.GetComponent<IUsable>().OnExit(player);
            }
        }
    }
}