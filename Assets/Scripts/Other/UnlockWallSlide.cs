using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Other
{
    public class UnlockWallSlide : MonoBehaviour, IUsable
    {
        public void OnEnter(GameObject user)
        {
            user.GetComponent<Player.PlayerMovement>().UnlockWallSlide();
            Destroy(gameObject);
        }

        public void OnExit(GameObject user) {}
        public void Use(GameObject user) {}
    }
}

