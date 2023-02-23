using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerCollisions : MonoBehaviour
    {
        GameObject player;
        PlayerInteraction playerInteraction;
        List<GameObject> usables;

        void Start() {
            player = transform.parent.gameObject;

            playerInteraction = player.GetComponent<PlayerInteraction>();
            playerInteraction.onUsePressed.AddListener(OnUse);

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

        private void OnUse()
        {
            if(usables.Count == 0) return;

            usables[usables.Count-1].GetComponent<IUsable>().Use(player);
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